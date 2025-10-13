﻿using FerPROJ.DBHelper.Base;
using FerPROJ.DBHelper.DBExtensions;
using FerPROJ.DBHelper.Entity.Users;
using FerPROJ.Design.Class;
using FerPROJ.Design.Interface;
using FerPROJ.Design.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.Repository {
    public partial class UserRepository : BaseRepository<BaseDbContext, UserModel, User, Guid>, IModelViewAsync<UserModel> {
        public UserRepository() {
        }

        public UserRepository(BaseDbContext ts) : base(ts) {
        }

        public async Task<IEnumerable<UserModel>> GetViewAsync(string searchText = "", DateTime? dateFrom = null, DateTime? dateTo = null, int dataLimit = 100) {
            var query = await GetAllWithSearchAsync(searchText, dateFrom, dateTo, dataLimit);

            var result = await query.SelectListAsync(async c => {

                var model = c.ToDestination<UserModel>();

                await Task.CompletedTask;

                return model;

            });

            return result;
        }
        public async Task<bool> CheckCredentialsAsync(UserModel model) {

            model.Password = CEncryptionManager.EncryptText(model.Password);

            var entity = await GetByPredicateAsync(c => c.UserName == model.UserName && c.Password == model.Password);

            if (entity == null) {
                CDialogManager.Warning("Invalid username or password.");
                return false;
            }

            CAppConstants.USERNAME = entity.UserName;
            CAppConstants.USER_ID = entity.Id;

            return true;

        }
    }
}

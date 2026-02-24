using FerPROJ.DBHelper.DBCrud;
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
    public partial class UserRepository : BaseRepository<BaseDbContext, UserModel, User, Guid> {
        public UserRepository() {
        }

        public UserRepository(BaseDbContext ts) : base(ts) {
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
            CAppConstants.NAME = entity.Name;

            return true;

        }
        public async Task<bool> CheckCredentialsAsync() {

            var username = CConfigurationManager.GetValue("username", "rememberme");

            var entity = await GetByPredicateAsync(c => c.UserName == username);

            if (entity == null) {
                CDialogManager.Warning("Invalid username or password.");
                return false;
            }

            CAppConstants.USERNAME = entity.UserName;
            CAppConstants.USER_ID = entity.Id;
            CAppConstants.NAME = entity.Name;

            return true;

        }

    }
}

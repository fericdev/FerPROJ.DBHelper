using FerPROJ.DBHelper.Base;
using FerPROJ.DBHelper.DBExtensions;
using FerPROJ.DBHelper.Helper;
using FerPROJ.Design.Class;
using FerPROJ.Design.Interface;
using FerPROJ.Design.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.Entity.Users {
    public partial class UserMigrations : IDbContextMigration<BaseDbContext> {
        public void Dispose() {
            throw new NotImplementedException();
        }

        public async Task RunMigrationAsync(BaseDbContext dbContext) {
            await DBHelpers.CreateOrUpdateTableOfEntityAsync<User>(dbContext);
            var passowrd = CEncryptionManager.EncryptText("adminpassword");

            if (!dbContext.Users.Any(u => u.UserName == "adminusername")) {
                await dbContext.SaveDTOAndCommitAsync<UserModel, User>(
                    new UserModel {
                        UserName = "adminusername",
                        Name = "System Administrator",
                        Password = passowrd,
                        UserRole = CBaseEnums.Role.Administrator.ToString(),
                    }
                );
            }
        }
    }
}

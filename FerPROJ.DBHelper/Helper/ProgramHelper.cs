using FerPROJ.DBHelper.DBCache;
using FerPROJ.DBHelper.Forms;
using FerPROJ.DBHelper.Repository;
using FerPROJ.Design.Class;
using FerPROJ.Design.Forms;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.Helper {
    public static class ProgramHelper {
        public static bool Initialize<DbContext>(string[] args, Assembly assembly, string systemName) {
            //
            CAssembly.SetAssembly<DbContext>(assembly);

            // Backup
            DbContextHelper.BackupDatabaseAsync(false).RunTask();

            // Check if any arguments were passed to avoid "Index out of range"
            if (args.GetIndexValue<bool>()) {
                DbContextHelper.RunDatabaseMigrationAsync().RunTaskAndForget();
            }

            try {
                var isLoggedIn = false;
                if (CConfigurationManager.IsLoginSkipped()) {
                    isLoggedIn = new UserRepository().CheckCredentialsAsync().RunTask();
                }

                if (!isLoggedIn) {
                    isLoggedIn = CFormLayer.ManageAsync<FrmLogin>(parameters: c => c.SystemName = systemName).RunTask();
                }

                if (isLoggedIn) {
                    LoadCacheAsync();
                }

                return isLoggedIn;
            }
            catch (Exception ex) {
                CDialogManager.Warning(ex.Message);
            }
            return false;
        }
        private static async void LoadCacheAsync() {
            await FrmSplasher.ShowSplashAsync(
                CacheManager.GetCacheLoadTasks("LMS.Repository")
            );
        }

    }
}

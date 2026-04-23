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
        public static bool IsLoggedIn {  get; private set; }
        public static async Task InitializeAsync<DbContext>(string[] args, Assembly assembly) {
            //
            CAssembly.SetAssembly<DbContext>(Assembly.GetExecutingAssembly());

            // Backup
            await DbContextHelper.BackupDatabaseAsync(false);

            // Check if any arguments were passed to avoid "Index out of range"
            if (args.GetIndexValue<bool>()) {
                await DbContextHelper.RunDatabaseMigrationAsync();
            }

            try {
                if (CConfigurationManager.IsLoginSkipped()) {
                    IsLoggedIn = await new UserRepository().CheckCredentialsAsync();
                }

                if (!IsLoggedIn) {
                    IsLoggedIn = await CFormLayer.ManageAsync<FrmLogin>();
                }
            }
            catch (Exception ex) {
                CDialogManager.Warning(ex.Message);
            }
        }

    }
}

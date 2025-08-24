using FerPROJ.DBHelper.Base;
using FerPROJ.DBHelper.Forms;
using FerPROJ.Design.Class;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.Helper {
    public static class DBHelpers {
        public static void OpenDatabaseConfiguration() {
            using (var frm = new FrmConf()) {
                frm.ShowDialog();
            }
        }
        public static Type GetDbContextType() {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            // Load all DLLs in the current bin directory
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            foreach (var dll in Directory.GetFiles(basePath, "*.dll")) {
                try {
                    var assemblyName = AssemblyName.GetAssemblyName(dll);
                    if (!assemblies.Any(a => a.GetName().Name == assemblyName.Name)) {
                        assemblies.Add(AppDomain.CurrentDomain.Load(assemblyName));
                    }
                }
                catch {
                    // ignore invalid dlls
                }
            }

            var dbContextTypes = assemblies
                .SelectMany(a => {
                    try {
                        return a.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex) {
                        return ex.Types.Where(t => t != null);
                    }
                })
                .Where(t =>
                    typeof(DbContext).IsAssignableFrom(t) &&
                    t != typeof(DbContext) &&                  // exclude EF base
                    !t.IsAbstract &&
                    !t.FullName.Contains("Migration") &&       // exclude EF migration temp contexts
                    !t.FullName.Contains("TempDbContext"))     // exclude EF's TempDbContext
                .ToList();

            if (dbContextTypes.Count > 1) {
                // If you have multiple DbContexts, you can filter by namespace or name here:
                return dbContextTypes.FirstOrDefault(t => t.Name == "BECMSDbContext");
            }

            return dbContextTypes.FirstOrDefault();
        }
        public static void RunDatabaseMigration() {
            var dbContextType = GetDbContextType();
            if (dbContextType == null) {
                throw new InvalidOperationException("No DbContext type found in loaded assemblies.");
            }

            using (var dbContext = (DbContext)Activator.CreateInstance(dbContextType)) {
                // 1. Check if the database exists
                if (!dbContext.Database.Exists()) {
                    dbContext.Database.Create();
                }
            } 

            // 2. Run migrations (DbMigrator will create its own context when needed)
            var configuration = new DbMigrationsConfiguration {
                AutomaticMigrationsEnabled = true,
                AutomaticMigrationDataLossAllowed = false, // better for production
                ContextType = dbContextType
            };
            try {
                var migrator = new DbMigrator(configuration);

                if (migrator.GetPendingMigrations().Any()) {
                    CShowMessage.Info("Applying migrations...");
                    migrator.Update();
                }
                else {
                    CShowMessage.Info("No pending migrations found.");
                }
            }
            finally {
                CShowMessage.Info("Database migration has been successfully executed.");
            }
        }

    }
}

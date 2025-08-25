using FerPROJ.DBHelper.Base;
using FerPROJ.DBHelper.Forms;
using FerPROJ.Design.Class;
using FerPROJ.Design.Interface;
using MySql.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.Helper {
    public static class DBHelpers {

        #region Open Database Configuration Form
        public static void OpenDatabaseConfiguration() {
            using (var frm = new FrmConf()) {
                frm.ShowDialog();
            }
        }
        #endregion

        #region Get DbContext Type
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
        #endregion

        #region Run Database Migration
        public static void RunDatabaseMigration() {

            // Find the DbContext type in the loaded assemblies
            var dbContextType = GetDbContextType();
            if (dbContextType == null) {
                throw new InvalidOperationException("No DbContext type found in loaded assemblies.");
            }

            // Disable model compatibility check
            var nullInitializerType = typeof(NullDatabaseInitializer<>).MakeGenericType(GetDbContextType());
            var nullInitializer = Activator.CreateInstance(nullInitializerType);
            typeof(Database)
                .GetMethod("SetInitializer", BindingFlags.Public | BindingFlags.Static)
                .MakeGenericMethod(GetDbContextType())
                .Invoke(null, new object[] { nullInitializer });

            using (var dbContext = (DbContext)Activator.CreateInstance(dbContextType)) {

                // 1. Check if the database exists
                if (!dbContext.Database.Exists()) {
                    dbContext.Database.Create();
                }

                // 2. Run custom seeders (classes implementing IDbContextMigration<T>)
                var migrationTypes = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a => {
                        try {
                            return a.GetTypes();
                        }
                        catch (ReflectionTypeLoadException ex) {
                            return ex.Types.Where(t => t != null);
                        }
                    })
                    .Where(t => !t.IsAbstract && !t.IsInterface)
                    .Where(t =>
                        t.GetInterfaces().Any(i =>
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(IDbContextMigration<>) &&
                            i.GetGenericArguments()[0] == dbContextType
                        )
                    )
                    .ToList();

                // 3. Loop through each migration type and execute its RunMigrationAsync method
                foreach (var migrationType in migrationTypes) {
                    var migrationInstance = Activator.CreateInstance(migrationType);

                    // Find the RunMigrationAsync method and invoke it
                    var method = migrationType.GetMethod("RunMigrationAsync");
                    if (method != null) {
                        var task = (Task)method.Invoke(migrationInstance, new object[] { dbContext });
                        task.GetAwaiter().GetResult(); // wait for async method
                    }
                }

            }

            CShowMessage.Info("Database migration has been successfully executed.");
        }
        #endregion

        #region Alter Table Columns
        public static void UpdateTableOfEntity<TEntity>(DbContext dbContext) {
            // Get table name and properties
            var tableName = typeof(TEntity).Name; 
            var properties = typeof(TEntity).GetProperties();

            foreach (var prop in properties) {
                // Determine column details
                var columnName = prop.Name;
                var columnType = GetMySqlColumnType(prop.PropertyType);
                var isNullable = !IsNonNullable(prop.PropertyType);
                var defaultValue = GetDefaultValue(prop, typeof(TEntity));

                // Apply changes to the database
                try {
                    // Check if column exists
                    if (IsColumnExists(dbContext, tableName, columnName)) {
                        // Alter existing column
                        dbContext.Database.ExecuteSqlCommand(
                            $"ALTER TABLE `{tableName}` MODIFY COLUMN `{columnName}` {columnType} {(isNullable ? "NULL" : "NOT NULL")} {(defaultValue != null ? $"DEFAULT '{defaultValue}'" : "")};"
                        );
                    }
                    else {
                        // Add new column
                        dbContext.Database.ExecuteSqlCommand(
                            $"ALTER TABLE `{tableName}` ADD COLUMN `{columnName}` {columnType} {(isNullable ? "NULL" : "NOT NULL")} {(defaultValue != null ? $"DEFAULT '{defaultValue}'" : "")};"
                        );
                    }
                }
                catch (Exception ex) {
                    CLibFilesWriter.CreateOrSetValue($"{tableName}:{columnName}", ex.Message.ToString(), parent: "DataMigrationError", encrypt: false);
                    continue;
                }
            }
        }

        private static string GetMySqlColumnType(Type type) {
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type == typeof(int)) return "INT";
            if (type == typeof(long)) return "BIGINT";
            if (type == typeof(short)) return "SMALLINT";
            if (type == typeof(byte)) return "TINYINT UNSIGNED";
            if (type == typeof(bool)) return "TINYINT(1)";
            if (type == typeof(decimal)) return "DECIMAL(18,2)";
            if (type == typeof(float)) return "FLOAT";
            if (type == typeof(double)) return "DOUBLE";
            if (type == typeof(string)) return "VARCHAR(255)";
            if (type == typeof(DateTime)) return "DATETIME";
            if (type == typeof(Guid)) return "CHAR(36)";
            if (type == typeof(byte[])) return "BLOB";
            // add more types if needed
            throw new NotSupportedException($"Type {type.Name} not supported");
        }

        private static bool IsNonNullable(Type type) {
            // If it's Nullable<T>, it's nullable
            if (Nullable.GetUnderlyingType(type) != null)
                return false;

            // Reference types are nullable
            if (!type.IsValueType)
                return false;

            // Value types (int, DateTime, bool, etc.) are non-nullable
            return true;
        }

        private static object GetDefaultValue(PropertyInfo prop, Type modelType) {
            // Create a temporary instance of the model
            var instance = Activator.CreateInstance(modelType);
            // Get the current value of the property
            return prop.GetValue(instance);
        }

        private static bool IsColumnExists(DbContext dbContext, string tableName, string columnName) {
            var sql = @"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_SCHEMA = DATABASE() 
                  AND TABLE_NAME = @p0 
                  AND COLUMN_NAME = @p1;
            ";

            var count = dbContext.Database
                .SqlQuery<int>(sql, tableName, columnName)
                .FirstOrDefault();

            return count > 0;
        }
        #endregion

    }
}

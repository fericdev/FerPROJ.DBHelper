using FerPROJ.DBHelper.Forms;
using FerPROJ.Design.Class;
using FerPROJ.Design.Forms;
using FerPROJ.Design.Interface;
using MySql.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FerPROJ.DBHelper.Helper {
    public static class DbContextHelper {

        #region Open Database Configuration Form
        public static void OpenDatabaseConfiguration() {
            using (var frm = new FrmDatabaseConfig()) {
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
                return dbContextTypes.FirstOrDefault(t => t.Name == "BaseDbContext");
            }

            return dbContextTypes.FirstOrDefault();
        }
        #endregion

        #region Run Database Migration
        public static async Task RunDatabaseMigrationAsync() {
            await FrmSplasherLoading.ShowSplashAsync();
            FrmSplasherLoading.SetLoadingText(0);

            // Find the DbContext type in the loaded assemblies
            var dbContextType = GetDbContextType();
            if (dbContextType == null) {
                throw new InvalidOperationException("No DbContext type found in loaded assemblies.");
            }

            using (var dbContext = (DbContext)Activator.CreateInstance(dbContextType)) {

                // 1. Check if the database exists
                if (!dbContext.Database.Exists()) {
                    dbContext.Database.Create();
                }

                FrmSplasherLoading.SetLoadingText(50);

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

                var totalMigrations = migrationTypes.Count;

                // 3. Loop through each migration type and execute its RunMigrationAsync method
                foreach (var migrationType in migrationTypes) {

                    FrmSplasherLoading.SetLoadingText(50 + (int)((migrationTypes.IndexOf(migrationType) + 1) / (double)totalMigrations * 50));

                    var migrationInstance = Activator.CreateInstance(migrationType);

                    // Find the RunMigrationAsync method and invoke it
                    var method = migrationType.GetMethod("RunMigrationAsync");
                    if (method != null) {
                        var task = (Task)method.Invoke(migrationInstance, new object[] { dbContext });
                        await task; // wait for async method
                    }
                }
                FrmSplasherLoading.SetLoadingText(100);
                FrmSplasherLoading.CloseSplash();
            }

            CDialogManager.Info("Database migration has been successfully executed.");
        }
        #endregion

        #region Alter Table Columns
        public static async Task CreateOrUpdateTableOfEntityAsync<TEntity>(DbContext dbContext, params Expression<Func<TEntity, object>>[] excludeProperties) {
            // Get table name and properties
            var tableName = typeof(TEntity).Name; 
            var properties = typeof(TEntity).GetProperties();

            // Exclude specified properties
            if (excludeProperties != null && excludeProperties.Length > 0) {
                var excludedNames = excludeProperties
                    .Select(GetPropertyName)
                    .ToHashSet();

                properties = properties
                    .Where(p => !excludedNames.Contains(p.Name))
                    .ToArray();
            }

            // Check if table exists
            if (!IsTableExists(dbContext, tableName)) {

                // 2. Build CREATE TABLE SQL dynamically
                var columnsSql = new List<string>();

                // Add columns
                foreach (var prop in properties) {
                    var columnName = prop.Name;
                    var columnType = GetMySqlColumnType(prop.PropertyType);
                    var isNullable = !IsNonNullable(prop.PropertyType);
                    var defaultValue = GetDefaultValue(prop, typeof(TEntity));

                    string columnDef = $"`{columnName}` {columnType} {(isNullable ? "NULL" : "NOT NULL")}";
                    if (defaultValue != null) {
                        columnDef += $" DEFAULT '{defaultValue}'";
                    }
                    columnsSql.Add(columnDef);
                }

                // Assume first property is primary key
                var primaryKey = properties.FirstOrDefault(p => string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase));
                if (primaryKey != null) {
                    columnsSql.Add($"PRIMARY KEY (`{primaryKey.Name}`)");
                }
                // Final CREATE TABLE SQL
                var createTableSql = $"CREATE TABLE `{tableName}` ({string.Join(", ", columnsSql)});";
                // Execute the CREATE TABLE command
                await dbContext.Database.ExecuteSqlCommandAsync(createTableSql);
            }
            else {

                // Table exists, alter columns as needed
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
                            await dbContext.Database.ExecuteSqlCommandAsync(
                                $"ALTER TABLE `{tableName}` MODIFY COLUMN `{columnName}` {columnType} {(isNullable ? "NULL" : "NOT NULL")} {(defaultValue != null ? $"DEFAULT '{defaultValue}'" : "")};"
                            );
                        }
                        else {
                            // Add new column
                            await dbContext.Database.ExecuteSqlCommandAsync(
                                $"ALTER TABLE `{tableName}` ADD COLUMN `{columnName}` {columnType} {(isNullable ? "NULL" : "NOT NULL")} {(defaultValue != null ? $"DEFAULT '{defaultValue}'" : "")};"
                            );
                        }
                    }
                    catch (Exception ex) {
                        CConfigurationManager.CreateOrSetValue($"{tableName}:{columnName}", ex.Message.ToString(), parent: "DataMigrationError", encrypt: false);
                        continue;
                    }
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
        private static bool IsTableExists(DbContext dbContext, string tableName) {
            var sql = @"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = DATABASE() 
                  AND TABLE_NAME = @p0;
            ";
            var count = dbContext.Database
                .SqlQuery<int>(sql, tableName)
                .FirstOrDefault();

            return count > 0;
        }
        private static string GetPropertyName<TEntity>(
            Expression<Func<TEntity, object>> expression) {
            if (expression.Body is MemberExpression member)
                return member.Member.Name;

            if (expression.Body is UnaryExpression unary &&
                unary.Operand is MemberExpression unaryMember)
                return unaryMember.Member.Name;

            throw new ArgumentException("Invalid property expression");
        }
        #endregion

        #region Create Backup of Database
        public static async Task BackupDatabaseAsync() {
            // Get database connection details from config
            var db = CConfigurationManager.GetValue("DatabaseName", "DatabaseConfig");
            var port = CConfigurationManager.GetValue("Port", "DatabaseConfig");
            var user = CConfigurationManager.GetValue("Uid", "DatabaseConfig");
            var pass = CConfigurationManager.GetValue("Pwd", "DatabaseConfig");
            var host = CConfigurationManager.GetValue("Server", "DatabaseConfig");

            // File name format: yyyy-MM-dd_hh-mm-tt.sql (tt = AM/PM)
            string fileName = $"{db}_{CAccessManager.CurrentDateTime()}.sql";
            string backupPath = CAccessManager.GetOrCreateEnvironmentPath(fileName, "Database Backup");

            // Build the mysqldump command
            string arguments = $"-h {host} -P {port} -u {user} -p{pass} --ssl-mode=DISABLED {db}";

            // Create backup directory if it doesn't exist
            ProcessStartInfo psi = new ProcessStartInfo {
                FileName = @"C:\Program Files\MySQL\MySQL Server 5.7\bin\mysqldump.exe",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,   // ✅ Capture errors too
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Ensure backup directory exists
            using (Process process = Process.Start(psi)) {

                string result = await process.StandardOutput.ReadToEndAsync();  // ✅ dump output
                string error = await process.StandardError.ReadToEndAsync();    // ✅ dump errors

                process.WaitForExit();

                if (!string.IsNullOrWhiteSpace(error)) {
                    CConfigurationManager.CreateOrSetValue($"{db}_{CAccessManager.CurrentDateTime()}", error, parent: "DatabaseBackup", encrypt: false);
                }

                File.WriteAllText(backupPath, result);
            }

            CDialogManager.Info($"Database backup created at: {backupPath}", "Backup Successful");
        }
        #endregion

        #region First Installation
        public static async Task RunDatabaseSetupAsync() {
            if (!string.IsNullOrEmpty(CConfigurationManager.GetValue("DatabaseSetup"))) {
                CDialogManager.Info("MySQL setup already completed.");
                return;
            }

            try {
                if (!IsMySQLInstalled()) {
                    await InstallMySQLAsync();
                }
                else {
                    CDialogManager.Info("MySQL is already installed, skipping installation.");
                }

                if (!IsConnectorInstalled()) {
                    await InstallConnectorAsync();
                }
                else {
                    CDialogManager.Info("MySQL Connector is already installed, skipping installation.");
                }

                await ConfigureMySQLAsync();

                if (!IsMySQLQueryBrowserInstalled()) {
                    await InstallMySQLBrowserAsync();
                }
                else {
                    CDialogManager.Info("MySQL Browser is already installed, skipping installation.");
                }

                CConfigurationManager.CreateOrSetValue("DatabaseSetup", "Completed");

                CDialogManager.Info("MySQL setup/configuration completed successfully.");
            }
            catch (Exception ex) {
                CDialogManager.Info("Setup failed: " + ex.Message);
            }
        }

        private static bool IsMySQLInstalled() {
            try {
                using (ServiceController sc = new ServiceController("MySQL57")) {
                    var status = sc.Status; // will throw if not installed
                    return true;
                }
            }
            catch {
                return false;
            }
        }

        // ✅ Simple check: look for connector DLL in GAC
        private static bool IsConnectorInstalled() {
            // Just a simple check if the MySql.Data.dll exists in GAC
            string gacPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                "Microsoft.NET", "assembly", "GAC_MSIL", "MySql.Data"
            );

            return Directory.Exists(gacPath);
        }
        private static bool IsMySQLQueryBrowserInstalled() {
            string[] possiblePaths = new[]
            {
                @"C:\Program Files\MySQL\MySQL Query Browser 1.1.20\MySQLQueryBrowser.exe",
                @"C:\Program Files (x86)\MySQL\MySQL Query Browser 1.1.20\MySQLQueryBrowser.exe"
            };

            foreach (var path in possiblePaths) {
                if (File.Exists(path))
                    return true;
            }

            return false;
        }

        private static async Task InstallMySQLAsync() {
            var installerPath = CAccessManager.GetOrCreateEnvironmentPath("mysql-installer-community-5.7.44.0.msi", "MySQL Setup");

            if (!File.Exists(installerPath))
                throw new FileNotFoundException("MySQL installer not found.");

            string args = $"/i \"{installerPath}\" /quiet /norestart " +
                          $"INSTALLDIR=\"C:\\Program Files\\MySQL\" " +
                          $"SERVICENAME=\"MySQL57\" " +
                          $"PORT=3309 " +
                          $"ROOTPASSWORD=\"RootPassword123!\" " +
                          $"ADDLOCAL=\"Server,Client\"";

            await RunProcessAsync("msiexec.exe", args);
        }

        private static async Task InstallConnectorAsync() {
            var installerPath = CAccessManager.GetOrCreateEnvironmentPath("mysql-connector-6.9.9.msi", "MySQL Setup");

            if (!File.Exists(installerPath))
                throw new FileNotFoundException("MySQL Connector installer not found.");

            string args = $"/i \"{installerPath}\" /quiet /norestart";
            await RunProcessAsync("msiexec.exe", args);
        }
        private static async Task InstallMySQLBrowserAsync() {
            var installerPath = CAccessManager.GetOrCreateEnvironmentPath("mysql-query-browser-1.1.20-win.msi", "MySQL Setup");

            if (!File.Exists(installerPath))
                throw new FileNotFoundException("MySQL Query Browser not found.");

            string args = $"/i \"{installerPath}\" /quiet /norestart";
            await RunProcessAsync("msiexec.exe", args);
        }

        private static async Task ConfigureMySQLAsync() {
            string sql = @"
                CREATE USER IF NOT EXISTS 'adminserver'@'localhost' IDENTIFIED BY 'admin123!@#';
                GRANT ALL PRIVILEGES ON *.* TO 'adminserver'@'localhost' WITH GRANT OPTION;
                FLUSH PRIVILEGES;
            ";

            File.WriteAllText("mysql_init.sql", sql);

            string[] mysqlPaths = new string[]
            {
                @"C:\MySQL\bin\mysql.exe",
                @"C:\Program Files\MySQL\MySQL Server 5.7\bin\mysql.exe",
                @"C:\Program Files (x86)\MySQL\MySQL Server 5.7\bin\mysql.exe"
            };

            var mysqlPathFound = string.Empty;

            foreach (var path in mysqlPaths) {
                if (File.Exists(path)) {
                    mysqlPathFound = path;
                    break;
                }
                else {
                    CDialogManager.Info($"Location path: {path}", "Not Found");
                }
            }

            if (!mysqlPathFound.IsNullOrEmpty()) {
                await RunProcessAsync("cmd.exe", $"/c \"\"{mysqlPathFound}\" -h localhost -P 3309 -u root -pRootPassword123! < mysql_init.sql\"");
            }
            else {
                CDialogManager.Info("No MySQL executable found.", "Error");
            }
        }

        private static async Task RunProcessAsync(string fileName, string arguments) {
            var process = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            if (process.ExitCode != 0)
                throw new Exception($"Process failed: {error} {output}");
        }

        #endregion

    }
}

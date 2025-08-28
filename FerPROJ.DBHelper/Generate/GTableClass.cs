using FerPROJ.Design.Class;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FerPROJ.DBHelper.Generate
{
    public class GTableClass
    {
        private string connectionString;
        private MySqlConnection connection;
        public GTableClass(string connString) {
            connectionString = connString;
            connection = new MySqlConnection(connectionString);
            connection.Open();
        }
        public DataTable GetListOfDatabases() {
            return connection.GetSchema("Databases");
        }
        private void CloseConnection() {
            if (connection.State == ConnectionState.Open) {
                connection.Close();
                connection.Dispose();
            }
        }
        public void GenerateClass(string dbName) {
            connection.ChangeDatabase(dbName);
            if (connection.State == ConnectionState.Open) {
                using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                    openFileDialog.Title = "Select Directory to Save Generated Files";
                    openFileDialog.Filter = "Folders|*.folder";
                    openFileDialog.CheckFileExists = false;
                    openFileDialog.FileName = dbName;
                    openFileDialog.InitialDirectory = "Model";

                    if (openFileDialog.ShowDialog() == DialogResult.OK) {
                        string filePath = Path.GetDirectoryName(openFileDialog.FileName);

                        DataTable tableSchema = connection.GetSchema("Tables");
                        //
                        foreach (DataRow row in tableSchema.Rows) {
                            string tableName = row["TABLE_NAME"].ToString();

                            DataTable columnsSchema = connection.GetSchema("Columns", new[] { null, null, tableName });

                            GenerateCSharpClass(filePath, tableName, columnsSchema);
                        }
                    }
                }
                CDialogManager.Info("Database column has been successfully generated.", "Success");
                CloseConnection();
            }

        }




        private void GenerateCSharpClass(string filePath, string tableName, DataTable columnsSchema) {
            string className = tableName;
            string fileName = Path.Combine(filePath, $"{className}.cs");

            if (!File.Exists(fileName)) {
                File.Delete(fileName);
            }

            List<string> Added = new List<string>();

            using (StreamWriter sw = new StreamWriter(fileName)) {
                sw.WriteLine($"public class {className}");
                sw.WriteLine("{");

                foreach (DataRow row in columnsSchema.Rows) {
                    string columnName = row["COLUMN_NAME"].ToString();
                    if (Added.Contains(columnName)) {
                        continue;
                    }
                    //
                    string dataType = row["DATA_TYPE"].ToString();
                    string csharpType = MapMySqlTypeToCSharp(dataType);
                    sw.WriteLine($"    public {csharpType} {columnName} {{ get; set; }}");
                    Added.Add(columnName);

                }
                sw.WriteLine($"    public string TableName => {className};");
                sw.WriteLine("}");
            }
        }
        private string MapMySqlTypeToCSharp(string mysqlType) {

            switch (mysqlType.ToLower()) {
                case "int":
                case "smallint":
                case "mediumint":
                case "bigint":
                    return "int";
                case "float":
                case "double":
                case "decimal":
                    return "decimal";
                case "char":
                case "varchar":
                case "text":
                case "mediumtext":
                case "longtext":
                case "enum":
                case "set":
                    return "string";
                case "binary":
                case "varbinary":
                case "blob":
                case "mediumblob":
                case "longblob":
                    return "byte[]";
                case "date":
                case "datetime":
                case "timestamp":
                    return "System.DateTime";
                case "time":
                    return "TimeSpan";
                case "bit":
                case "tinyint":
                    return "bool";
                // Handle other MySQL data types
                default:
                    return "object";
            }

        }

    }
}

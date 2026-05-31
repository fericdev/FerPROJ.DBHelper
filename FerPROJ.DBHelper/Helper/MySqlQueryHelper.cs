using FerPROJ.Design.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.Query {
    public static class MySqlQueryHelper {
        public static TableSchema BuildSchema<T>() {
            var type = typeof(T);

            var table = new TableSchema {
                TableName = type.Name,
            };

            foreach (var prop in type.GetProperties()) {
                // Skip ignored
                if (prop.GetCustomAttribute<IgnoreColumnAttribute>() != null)
                    continue;

                var column = new ColumnSchema {
                    Name = prop.Name,
                    Type = ResolveSqlType(prop),
                };

                // Check for primary key
                if (prop.GetCustomAttribute<PrimaryKeyAttribute>() != null) {
                    column.IsPrimary = true;
                }
                else if(prop.Name.Equals("Id")) {
                    column.IsPrimary = true; 
                }

                table.Columns.Add(column);
            }

            return table;
        }

        private static string ResolveSqlType(PropertyInfo prop) {
            // If manually specified, use it
            var customType = prop.GetCustomAttribute<ColumnTypeAttribute>();
            if (customType != null)
                return customType.Type;

            // Default mappings
            var type = prop.PropertyType;

            if (type == typeof(int)) return "int";
            if (type == typeof(long)) return "bigint";
            if (type == typeof(short)) return "smallint";
            if (type == typeof(byte)) return "tinyint unsigned";
            if (type == typeof(bool)) return "tinyint(1)";
            if (type == typeof(decimal)) return "decimal(18,2)";
            if (type == typeof(float)) return "float";
            if (type == typeof(double)) return "double";
            if (type == typeof(string)) return "varchar(255)";
            if (type == typeof(DateTime)) return "datetime";
            if (type == typeof(Guid)) return "char(36)";
            if (type == typeof(byte[])) return "longblob";

            return "text"; // fallback
        }
    }

    #region Table Schema
    public class TableSchema {
        public string TableName { get; set; }
        public List<ColumnSchema> Columns { get; set; } = new List<ColumnSchema>();
    }

    public class ColumnSchema {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsPrimary { get; set; }
    }
    #endregion

    #region Attributes
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreColumnAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnTypeAttribute : Attribute {
        public string Type { get; }
        public ColumnTypeAttribute(string type) {
            Type = type;
        }
    }
    #endregion
}

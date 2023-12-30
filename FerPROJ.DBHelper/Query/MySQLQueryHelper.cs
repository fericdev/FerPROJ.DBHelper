using FerPROJ.Design.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.Query {
    public static class MySQLQueryHelper {
        public static string GetDateRange(DateTime dtpFrom, DateTime dtpTo, string ColumnName = "DateReference") {
            return $"{ColumnName} > '{CConvert.GetDate(dtpFrom.AddDays(-1))}' AND {ColumnName} <= '{CConvert.GetDate(dtpTo)}'";
        }
        public static string GetMultipleSearchIN(List<string> Values, string ColumnName) {
            string i = $"{ColumnName} IN(";
            foreach (var item in Values) {
                i += $"'{item}',";
            }
            i = i.TrimEnd(',');
            i += ")";
            return i;
        }
        public static string GetMultipleSearchLIKE(string Value, List<string> ColumnsName) {
            string i = string.Empty;

            if (ColumnsName.Count > 0) {
                i = $"{ColumnsName[0]} LIKE '%{Value}%'";

                for (int j = 1; j < ColumnsName.Count; j++) {
                    i += $" OR {ColumnsName[j]} LIKE '%{Value}%'";
                }
            }

            return i;
        }
        public static string GetSearchLIKE(string Value, string ColumnName) {
            return $"{ColumnName} LIKE '%{Value}%'";
        }
    }
}

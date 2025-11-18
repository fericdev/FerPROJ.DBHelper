using FerPROJ.Design.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.Query {
    public static class MySqlQueryHelper {
        public static string GetDateRangeWord(DateTime dtpFrom, DateTime dtpTo) {
            return $"From {dtpFrom.ToString("MMMM dd, yyyy")} To {dtpTo.ToString("MMMM dd, yyyy")}";
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
            if(Value == null || Value == "") {
                Value = "%";
            }
            if (ColumnsName.Count > 0) {
                i = $"({ColumnsName[0]} LIKE '%{Value}%'";

                for (int j = 1; j < ColumnsName.Count; j++) {
                    i += $" OR {ColumnsName[j]} LIKE '%{Value}%'";
                }
                i += ")";
            }

            return i;
        }
        public static string GetSearchLIKE(string Value, string ColumnName) {
            return $"{ColumnName} LIKE '%{Value}%'";
        }
        public static string GetSelectAll(string TableName) {
            return $"SELECT * FROM {TableName} ORDER BY DataReference ASC";
        }
        public static string GetSelectAll(string TableName, string Where) {
            if (!Where.ToUpper().Contains("WHERE")) {
                string oWhere = Where;
                Where = $"WHERE {oWhere}";
            }
            return $"SELECT * FROM {TableName} {Where}";
        }
        public static string GetSelectINNERJOIN(string TableName1, string TableName2, string Id) {
            return $"SELECT * FROM {TableName1} tbl1 INNER JOIN {TableName2} tbl2 ON tbl1.{Id} = tbl2.{Id}";
        }
        public static string GetSelectINNERJOIN(string TableName1, string TableName2, string Id, string Where) {
            return $"SELECT * FROM {TableName1} tbl1 INNER JOIN {TableName2} tbl2 ON tbl1.{Id} = tbl2.{Id} {Where}";
        }
        public static string GetSelectJOIN(string JoinType,string TableName1, string TableName2, string Id, string Where) {
            return $"SELECT * FROM {TableName1} tbl1 {JoinType} {TableName2} tbl2 ON tbl1.{Id} = tbl2.{Id} {Where}";
        }
        public static string GetSelectJOIN(string JoinType, string TableName1, string TableName2, string Id) {
            return $"SELECT * FROM {TableName1} tbl1 {JoinType} {TableName2} tbl2 ON tbl1.{Id} = tbl2.{Id}";
        }
    }
}

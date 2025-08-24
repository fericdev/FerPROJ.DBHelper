using FerPROJ.Design.Class;
using FerPROJ.Design.BaseModels;
using FerPROJ.Design.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using static FerPROJ.Design.Class.CEnum;

namespace FerPROJ.DBHelper.CRUD {
    public class Conn : IDisposable {
        private MySqlCommand commandResult;
        private MySqlCommand commandResult2;
        private MySqlConnection connectionResult = new MySqlConnection();
        private MySqlConnection connectionResult2 = new MySqlConnection();
        private MySqlTransaction beginTransaction;
        private MySqlTransaction beginTransaction2;
        private string connectionString;
        private string connectionString2;
        private int rowsAffected;
        private int rowsAffected2;
        public AllowedOpenDB dbConnection = CEnum.AllowedOpenDB.One;

        public Conn() {
            SetNewConnection();
        }
        public Conn(DbContext _ts) {
            connectionResult = (MySqlConnection)_ts.Database.Connection;
            if (connectionResult.State == ConnectionState.Closed) {
                connectionResult.Open();
            }
        }
        public void SetNewConnection() {
            this.connectionString = CStaticVariable.CONN_STRING_1 != null ? CStaticVariable.CONN_STRING_1 : CStaticVariable.ENTITY_CONNECTION_STRING;
            this.connectionString2 = CStaticVariable.CONN_STRING_2;
        }
        public void CloseConnection() {
            if (dbConnection == AllowedOpenDB.One) {
                if (connectionResult.State == ConnectionState.Open) {
                    connectionResult.Close();
                    connectionResult.Dispose();
                    commandResult.Dispose();
                }
            } else if (dbConnection == AllowedOpenDB.Two) {
                if (connectionResult.State == ConnectionState.Open) {
                    connectionResult.Close();
                    connectionResult.Dispose();
                    commandResult.Dispose();
                    beginTransaction.Dispose();
                    CStaticVariable.CONN_STRING_1 = null;
                }
                if (connectionResult2.State == ConnectionState.Open) {
                    connectionResult2.Close();
                    beginTransaction2.Dispose();
                    connectionResult2.Dispose();
                    commandResult2.Dispose();
                }
            }
        }
        public bool ExecuteQuery(string queryStatement) {
            try {
                if (connectionResult.State == ConnectionState.Closed) {
                    connectionResult = new MySqlConnection(connectionString);
                    connectionResult.Open();
                }
                //
                commandResult = new MySqlCommand(queryStatement, connectionResult);
                rowsAffected = commandResult.ExecuteNonQuery();
                return true;
            } catch (MySqlException ex) {
                CShowMessage.Warning(ex.Message, "Warning");
                CloseConnection();
                return false;
            }
        }
        public bool ExecuteQuery(List<string> queryStatement) {
            try {
                DBConnect();
                //
                foreach (var sQuery in queryStatement) {
                    ExecuteMultpleQuery(sQuery);
                }
                TransCommit();
                return true;
            } catch (MySqlException ex) {
                CShowMessage.Warning(ex.Message, "Warning");
                TransRollback();
                CloseConnection();
                return false;
            }

            void DBConnect() {
                if (dbConnection == AllowedOpenDB.One) {
                    if (connectionResult.State == ConnectionState.Closed) {
                        connectionResult = new MySqlConnection(connectionString);
                        connectionResult.Open();
                    }
                    beginTransaction = connectionResult.BeginTransaction();
                }
            }

            void TransCommit() {
                if (dbConnection == AllowedOpenDB.One) {
                    beginTransaction.Commit();
                }
            }

            void ExecuteMultpleQuery(string sQuery) {
                if (dbConnection == AllowedOpenDB.One) {
                    commandResult = new MySqlCommand(sQuery, connectionResult);
                    rowsAffected += commandResult.ExecuteNonQuery();
                }
            }
        }
        public bool ExecuteQuery(List<string> queryStatement, List<string> queryStatement2) {
            try {
                DBConnect();
                //
                foreach (var sQuery in queryStatement) {
                    ExecuteMultpleQuery(sQuery);
                }
                foreach (var sQuery in queryStatement2) {
                    ExecuteMultpleQuery2(sQuery);
                }
                TransCommit();
                return true;
            } catch (MySqlException ex) {
                CShowMessage.Warning(ex.Message, "Warning");
                TransRollback();
                CloseConnection();
                return false;
            }

            void DBConnect() {
                if (dbConnection == AllowedOpenDB.Two) {
                    if (connectionResult.State == ConnectionState.Closed) {
                        connectionResult = new MySqlConnection(connectionString);
                        connectionResult.Open();
                    }
                    if (connectionResult2.State == ConnectionState.Closed) {
                        connectionResult2 = new MySqlConnection(connectionString2);
                        connectionResult2.Open();
                    }
                    beginTransaction = connectionResult.BeginTransaction();
                    beginTransaction2 = connectionResult.BeginTransaction();
                }
            }

            void TransCommit() {
                if (dbConnection == AllowedOpenDB.Two) {
                    beginTransaction.Commit();
                    beginTransaction2.Commit();
                }
            }

            void ExecuteMultpleQuery(string sQuery) {
                if (dbConnection == AllowedOpenDB.Two) {
                    commandResult = new MySqlCommand(sQuery, connectionResult);
                    rowsAffected = commandResult.ExecuteNonQuery();
                }
            }
            void ExecuteMultpleQuery2(string sQuery) {
                if (dbConnection == AllowedOpenDB.Two) {
                    commandResult2 = new MySqlCommand(sQuery, connectionResult2);
                    rowsAffected2 = commandResult2.ExecuteNonQuery();
                }
            }
        }

        private void TransRollback() {

            if (dbConnection == AllowedOpenDB.One) {
                beginTransaction.Rollback();
            } else if (dbConnection == AllowedOpenDB.Two) {
                beginTransaction.Rollback();
                beginTransaction2.Rollback();
            }

        }
        public TClass GetValueFromColumn<TClass>(string Method, string Table, string Column = "*", string Where = "") where TClass : new() {
            string selectQuery = $"SELECT {Method}({Column}) as result FROM {Table} {Where}";

            if (ExecuteQuery(selectQuery)) {
                using (MySqlDataReader reader = commandResult.ExecuteReader()) {
                    if (reader.Read()) {
                        int idIndex = reader.GetOrdinal("result"); // Get the column index for "ID"

                        if (!reader.IsDBNull(idIndex)) {
                            // Use Convert.ChangeType to convert the value to TClass
                            return (TClass)Convert.ChangeType(reader.GetValue(idIndex), typeof(TClass));
                        } else {
                            return default(TClass); // or another appropriate default value for DBNull
                        }
                    }
                }
            }

            return default(TClass);
        }
        public void FillDGV(DataGridView dgv, string queryStatement) {
            if (ExecuteQuery(queryStatement)) {
                MySqlDataAdapter adapter = new MySqlDataAdapter(commandResult);
                DataTable data = new DataTable();
                adapter.Fill(data);
                dgv.DataSource = data;
            }
        }
        public void FillComboBox(ComboBox cmb, string cmbSelectedText, string queryStatement) {
            if (ExecuteQuery(queryStatement)) {
                using (MySqlDataReader reader = commandResult.ExecuteReader()) {
                    List<string> items = new List<string>();
                    while (reader.Read()) {
                        string cmbName = reader[cmbSelectedText].ToString();
                        if (!items.Contains(cmbName)) {
                            items.Add(cmbName);
                        }
                    }
                    cmb.DisplayMember = "Key"; // This corresponds to cmbName
                    cmb.DataSource = new BindingSource(items, null);
                }
            }
        }
        public void FillComboBox(ComboBox cmb, string cmbSelectedText, string cmbSelectedValue, string queryStatement) {
            if (ExecuteQuery(queryStatement)) {
                using (MySqlDataReader reader = commandResult.ExecuteReader()) {
                    Dictionary<string, string> items = new Dictionary<string, string>();
                    while (reader.Read()) {
                        string cmbName = reader[cmbSelectedText].ToString();
                        string cmbValue = reader[cmbSelectedValue].ToString();
                        if (!items.ContainsKey(cmbName) && !items.ContainsValue(cmbValue)) {
                            items.Add(cmbName, cmbValue);
                        }
                    }
                    cmb.DisplayMember = "Key"; // This corresponds to cmbName
                    cmb.ValueMember = "Value"; // This corresponds to cmbValue
                    cmb.DataSource = new BindingSource(items, null);
                }
            }
        }
        public void FillComboBox(CComboBox cmb, string cmbSelectedText, string cmbSelectedValue, string queryStatement) {
            if (ExecuteQuery(queryStatement)) {
                using (MySqlDataReader reader = commandResult.ExecuteReader()) {
                    Dictionary<string, string> items = new Dictionary<string, string>();
                    while (reader.Read()) {
                        string cmbName = reader[cmbSelectedText].ToString();
                        string cmbValue = reader[cmbSelectedValue].ToString();
                        items.Add(cmbName, cmbValue);
                    }
                    cmb.DisplayMember = "Key"; // This corresponds to cmbName
                    cmb.ValueMember = "Value"; // This corresponds to cmbValue
                    cmb.DataSource = new BindingSource(items, null);
                }
            }
        }
        public DataTable GetDataTable(string queryStatement) {
            DataTable dataTable = new DataTable();

            if (ExecuteQuery(queryStatement)) {
                MySqlDataAdapter adapter = new MySqlDataAdapter(commandResult);
                adapter.Fill(dataTable);
            }
            CloseConnection();
            return dataTable;
        }
        private void GetColumnValue<DTO>(DTO sDTO, out string[] columnList, out string[] valueList, string[] fieldsToExclude = null) {
            if (fieldsToExclude == null)
                fieldsToExclude = new string[0];
            var dtoType = typeof(DTO);
            PropertyInfo[] properties = dtoType.GetProperties();
            List<string> excludedProperties = new List<string> { "Success", "List", "tableName", "IdTrack", "Details", "Item", "Error", "IsValid", "DataValidation" };
            List<string> columns = new List<string>();
            List<string> values = new List<string>();

            foreach (PropertyInfo property in properties) {
                if (!excludedProperties.Any(c => property.Name.Contains(c)) && !fieldsToExclude.Contains(property.Name)) {
                    if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(int) ||
                        property.PropertyType == typeof(decimal)) {
                        columns.Add(property.Name);
                        values.Add($"{property.GetValue(sDTO)}");
                    } else if (property.PropertyType == typeof(byte[])) {
                        byte[] imageBytes = (byte[])property.GetValue(sDTO);
                        string hexValue = BitConverter.ToString(imageBytes).Replace("-", "");
                        columns.Add(property.Name);
                        values.Add($"0x{hexValue}");
                    } else if (property.PropertyType == typeof(DateTime)) {
                        DateTime dt = Convert.ToDateTime(property.GetValue(sDTO));
                        string cdt = dt.ToString("yyyy-MM-dd hh:mm:ss");
                        columns.Add(property.Name);
                        values.Add($"'{cdt}'");
                    } else {
                        columns.Add(property.Name);
                        values.Add($"'{property.GetValue(sDTO)}'");
                    }
                }

            }

            columnList = columns.ToArray();
            valueList = values.ToArray();
        }
        private void GetColumnValueForList<DTO>(DTO sDTO, out string[] columnList, out string[] valueList, string[] fieldsToExclude = null) {
            if (fieldsToExclude == null)
                fieldsToExclude = new string[0];
            var dtoType = sDTO.GetType();
            PropertyInfo[] properties = dtoType.GetProperties();
            List<string> excludedProperties = new List<string> { "Success", "List", "tableName", "IdTrack", "Details", "Item", "Error", "IsValid", "DataValidation" };
            List<string> columns = new List<string>();
            List<string> values = new List<string>();

            foreach (PropertyInfo property in properties) {
                if (!excludedProperties.Any(c => property.Name.Contains(c)) && !fieldsToExclude.Contains(property.Name)) {
                    if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(int) ||
                        property.PropertyType == typeof(decimal)) {
                        columns.Add(property.Name);
                        values.Add($"{property.GetValue(sDTO)}");
                    } else if (property.PropertyType == typeof(byte[])) {
                        byte[] imageBytes = (byte[])property.GetValue(sDTO);
                        string hexValue = BitConverter.ToString(imageBytes).Replace("-", "");
                        columns.Add(property.Name);
                        values.Add($"0x{hexValue}");
                    } else if (property.PropertyType == typeof(DateTime)) {
                        DateTime dt = Convert.ToDateTime(property.GetValue(sDTO));
                        string cdt = dt.ToString("yyyy-MM-dd hh:mm:ss");
                        columns.Add(property.Name);
                        values.Add($"'{cdt}'");
                    } else {
                        columns.Add(property.Name);
                        values.Add($"'{property.GetValue(sDTO)}'");
                    }
                }

            }

            columnList = columns.ToArray();
            valueList = values.ToArray();
        }
        private void GetColumnValueUpdate<DTO>(DTO sDTO, out string[] columnValueList, string[] fieldsToExclude = null) {
            if (fieldsToExclude == null)
                fieldsToExclude = new string[0];
            Type dtoType = typeof(DTO);
            PropertyInfo[] properties = dtoType.GetProperties();

            List<string> excludedProperties = new List<string> { "Success", "List", "tableName", "IdTrack", "Details", "Item", "Error", "IsValid", "DataValidation" };
            List<string> columnValue = new List<string>();

            foreach (PropertyInfo property in properties) {
                if (!excludedProperties.Any(c => property.Name.Contains(c)) && !fieldsToExclude.Contains(property.Name)) {
                    object propertyValue = property.GetValue(sDTO);

                    if (propertyValue.ToString() != "") {
                        if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(int) ||
                            property.PropertyType == typeof(decimal)) {
                            columnValue.Add($"{property.Name} = {property.GetValue(sDTO)}");
                        } else if (property.PropertyType == typeof(byte[])) {
                            byte[] imageBytes = (byte[])property.GetValue(sDTO);
                            string hexValue = BitConverter.ToString(imageBytes).Replace("-", "");
                            columnValue.Add($"{property.Name} = 0x{hexValue}");
                        } else if (property.PropertyType == typeof(DateTime)) {
                            DateTime dt = Convert.ToDateTime(property.GetValue(sDTO));
                            string cdt = dt.ToString("yyyy-MM-dd hh:mm:ss");
                            columnValue.Add($"{property.Name} = '{cdt}'");
                        } else {
                            columnValue.Add($"{property.Name} = '{property.GetValue(sDTO)}'");
                        }
                    }
                }
            }

            columnValueList = columnValue.ToArray();
        }
        public DTO GetData<DTO>(string _tableName, string sWhere, string[] fieldsToExclude = null) where DTO : new() {
            string sQuery = $"SELECT * FROM {_tableName} {sWhere}";
            if (fieldsToExclude == null)
                fieldsToExclude = new string[0];
            DTO result = default(DTO);
            if (ExecuteQuery(sQuery)) {
                using (MySqlDataReader reader = commandResult.ExecuteReader()) {
                    if (reader.Read()) {
                        result = new DTO();

                        foreach (var property in typeof(DTO).GetProperties()) {
                            try {
                                if (reader[property.Name] != DBNull.Value && property.CanWrite && !fieldsToExclude.Contains(property.Name)) {
                                    object dbValue = reader[property.Name];
                                    Type propertyType = property.PropertyType;

                                    var convertedValue = Convert.ChangeType(dbValue, propertyType);
                                    property.SetValue(result, convertedValue);
                                }
                            } catch (IndexOutOfRangeException ex) {
                                Console.WriteLine(ex);
                            }
                        }
                    }
                }
            }
            return result;
        }
        public DTO GetData<DTO>(string _tableName, string _tableDetailsName, string sWhere, string[] fieldsToExclude = null) where DTO : new() {
            string sQuery = $"SELECT * FROM {_tableName} {sWhere}";
            if (fieldsToExclude == null)
                fieldsToExclude = new string[0];
            DTO result = default(DTO);
            if (ExecuteQuery(sQuery)) {
                using (MySqlDataReader reader = commandResult.ExecuteReader()) {
                    if (reader.Read()) {
                        result = new DTO();

                        foreach (var property in typeof(DTO).GetProperties()) {
                            try {
                                if (reader[property.Name] != DBNull.Value && property.CanWrite && !fieldsToExclude.Contains(property.Name)) {
                                    object dbValue = reader[property.Name];
                                    Type propertyType = property.PropertyType;

                                    var convertedValue = Convert.ChangeType(dbValue, propertyType);
                                    property.SetValue(result, convertedValue);
                                }
                            } catch (IndexOutOfRangeException ex) {
                                Console.WriteLine(ex);
                            }
                        }
                        PropertyInfo detailsListProperty = (PropertyInfo)typeof(DTO).GetProperties().Where(c => c.PropertyType.IsGenericType && c.PropertyType.GetGenericTypeDefinition() == typeof(List<>)).FirstOrDefault();

                        if (detailsListProperty != null) {
                            Conn c = new Conn();
                            Type innerType = detailsListProperty.PropertyType.GetGenericArguments()[0];
                            MethodInfo getListDataMethod = typeof(Conn)
                                                          .GetMethod("GetListData")
                                                          .MakeGenericMethod(innerType);

                            object detailsList = getListDataMethod.Invoke(c, new object[] { _tableDetailsName, sWhere, fieldsToExclude });
                            //
                            detailsListProperty.SetValue(result, detailsList);
                        }
                    }
                }
            }
            return result;
        }
        public IQueryable<DTO> GetAll<DTO>(string _tableName, string[] fieldsToExclude = null) where DTO : new() {
            return GetListData<DTO>(_tableName, null, fieldsToExclude).AsQueryable();
        }
        public List<DTO> GetListData<DTO>(string _tableDetailsName, string sWhere, string[] fieldsToExclude = null) where DTO : new() {
            string sQuery = $"SELECT * FROM {_tableDetailsName} {sWhere}";
            if (fieldsToExclude == null)
                fieldsToExclude = new string[0];
            List<DTO> resultList = new List<DTO>();

            if (ExecuteQuery(sQuery)) {
                using (MySqlDataReader reader = commandResult.ExecuteReader()) {
                    while (reader.Read()) {
                        DTO result = new DTO();

                        foreach (var property in typeof(DTO).GetProperties()) {
                            try {
                                if (reader[property.Name] != DBNull.Value && property.CanWrite && !fieldsToExclude.Contains(property.Name)) {
                                    object dbValue = reader[property.Name];
                                    Type propertyType = property.PropertyType;

                                    var convertedValue = Convert.ChangeType(dbValue, propertyType);
                                    property.SetValue(result, convertedValue);
                                }
                            } catch (IndexOutOfRangeException ex) {
                                Console.WriteLine(ex);
                            }
                        }

                        resultList.Add(result);
                    }
                }

            }

            return resultList;
        }
        public string GetNewStringID(string prefix, string tableName) {
            return $"{prefix}-00{GetValueFromColumn<int>("COUNT", tableName) + 1}";
        }
        public bool SaveManual<sDTO>(string tableName, sDTO myDTO) where sDTO : BaseModel {
            if (myDTO.DataValidation()) {
                string[] fieldsToExclude = null;
                string message = null;
                if (CShowMessage.Ask(message != null ? message : "Are you sure to save this data?", "Confirmation")) {
                    string[] columnList;
                    string[] valueList;


                    if (fieldsToExclude != null) {
                        GetColumnValue<sDTO>(myDTO, out columnList, out valueList, fieldsToExclude);
                    } else {
                        GetColumnValue<sDTO>(myDTO, out columnList, out valueList);
                    }


                    string columns = string.Join(", ", columnList);
                    string values = string.Join(", ", valueList);
                    string insertQuery = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
                    if (ExecuteQuery(insertQuery)) {
                        CShowMessage.Info("Saved Successfully.", "Success");
                        return true;
                    }
                }
            }
            CShowMessage.Warning(myDTO.Error, "Warning");
            return false;

        }
        public bool SaveManual<sDTO>(string tableName, sDTO myDTO, string[] fieldsToExclude = null) where sDTO : BaseModel {
            if (myDTO.DataValidation()) {
                string message = null;
                if (CShowMessage.Ask(message != null ? message : "Are you sure to save this data?", "Confirmation")) {
                    string[] columnList;
                    string[] valueList;


                    if (fieldsToExclude != null) {
                        GetColumnValue<sDTO>(myDTO, out columnList, out valueList, fieldsToExclude);
                    } else {
                        GetColumnValue<sDTO>(myDTO, out columnList, out valueList);
                    }


                    string columns = string.Join(", ", columnList);
                    string values = string.Join(", ", valueList);
                    string insertQuery = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
                    if (ExecuteQuery(insertQuery)) {
                        CShowMessage.Info("Saved Successfully.", "Success");
                        return true;
                    }
                }
            }
            CShowMessage.Warning(myDTO.Error, "Warning");
            return false;
        }
        public bool SaveManual<sDTO>(string tableName, sDTO myDTO, string message = null) where sDTO : BaseModel {
            if (myDTO.DataValidation()) {
                string[] fieldsToExclude = null;
                if (CShowMessage.Ask(message != null ? message : "Are you sure to save this data?", "Confirmation")) {
                    string[] columnList;
                    string[] valueList;


                    if (fieldsToExclude != null) {
                        GetColumnValue<sDTO>(myDTO, out columnList, out valueList, fieldsToExclude);
                    } else {
                        GetColumnValue<sDTO>(myDTO, out columnList, out valueList);
                    }


                    string columns = string.Join(", ", columnList);
                    string values = string.Join(", ", valueList);
                    string insertQuery = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
                    if (ExecuteQuery(insertQuery)) {
                        CShowMessage.Info("Saved Successfully.", "Success");
                        return true;
                    }
                }
            }
            CShowMessage.Warning(myDTO.Error, "Warning");
            return false;
        }
        public void SaveDetailsManual<mDTO>(string tableName, mDTO myDTO) where mDTO : BaseModel {
            if (myDTO.DataValidation()) {
                string[] fieldsToExclude = null;
                string[] columnList;
                string[] valueList;

                if (fieldsToExclude != null) {
                    GetColumnValue<mDTO>(myDTO, out columnList, out valueList, fieldsToExclude);
                } else {
                    GetColumnValue<mDTO>(myDTO, out columnList, out valueList);
                }
                string columns = string.Join(", ", columnList);
                string values = string.Join(", ", valueList);
                string insertQuery = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
                if (!ExecuteQuery(insertQuery)) {
                    throw new ArgumentException("Database Error!");
                }
            } else {
                throw new ArgumentException(myDTO.Error);
            }
        }
        public void SaveDetailsManual<mDTO>(string tableName, mDTO myDTO, string[] fieldsToExclude = null) where mDTO : BaseModel {
            if (myDTO.DataValidation()) {
                string[] columnList;
                string[] valueList;

                if (fieldsToExclude != null) {
                    GetColumnValue<mDTO>(myDTO, out columnList, out valueList, fieldsToExclude);
                } else {
                    GetColumnValue<mDTO>(myDTO, out columnList, out valueList);
                }
                string columns = string.Join(", ", columnList);
                string values = string.Join(", ", valueList);
                string insertQuery = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
                if (!ExecuteQuery(insertQuery)) {
                    throw new ArgumentException("Database Error");
                }
            } else {
                throw new ArgumentException(myDTO.Error);
            }
        }
        public bool UpdateManual<sDTO>(string tableName, string whereCondition, sDTO myDTO) where sDTO : BaseModel {
            if (myDTO.DataValidation()) {
                string message = null;
                string[] fieldsToExclude = null;
                if (CShowMessage.Ask(message != null ? message : "Are you sure to update this data?", "Confirmation")) {
                    string[] columnAndValueList;

                    if (fieldsToExclude != null) {
                        GetColumnValueUpdate<sDTO>(myDTO, out columnAndValueList, fieldsToExclude);
                    } else {
                        GetColumnValueUpdate<sDTO>(myDTO, out columnAndValueList);
                    }

                    string columns = string.Join(", ", columnAndValueList);
                    string insertQuery = $"UPDATE {tableName} SET {columns} {whereCondition}";
                    if (ExecuteQuery(insertQuery)) {
                        CShowMessage.Info("Updated Successfully.", "Success");
                        return true;
                    }
                }
            }
            CShowMessage.Warning(myDTO.Error, "Warning");
            return false;
        }
        public bool UpdateManual<sDTO>(string tableName, string whereCondition, sDTO myDTO, string message = null) where sDTO : BaseModel {
            if (myDTO.DataValidation()) {
                string[] fieldsToExclude = null;
                if (CShowMessage.Ask(message != null ? message : "Are you sure to update this data?", "Confirmation")) {
                    string[] columnAndValueList;

                    if (fieldsToExclude != null) {
                        GetColumnValueUpdate<sDTO>(myDTO, out columnAndValueList, fieldsToExclude);
                    } else {
                        GetColumnValueUpdate<sDTO>(myDTO, out columnAndValueList);
                    }

                    string columns = string.Join(", ", columnAndValueList);
                    string insertQuery = $"UPDATE {tableName} SET {columns} {whereCondition}";
                    if (ExecuteQuery(insertQuery)) {
                        CShowMessage.Info("Updated Successfully.", "Success");
                        return true;
                    }
                }
            }
            CShowMessage.Warning(myDTO.Error, "Warning");
            return false;
        }
        public bool UpdateManual<sDTO>(string tableName, string whereCondition, sDTO myDTO, string[] fieldsToExclude = null) where sDTO : BaseModel {
            if (myDTO.DataValidation()) {
                string message = null;
                if (CShowMessage.Ask(message != null ? message : "Are you sure to update this data?", "Confirmation")) {
                    string[] columnAndValueList;

                    if (fieldsToExclude != null) {
                        GetColumnValueUpdate<sDTO>(myDTO, out columnAndValueList, fieldsToExclude);
                    } else {
                        GetColumnValueUpdate<sDTO>(myDTO, out columnAndValueList);
                    }

                    string columns = string.Join(", ", columnAndValueList);
                    string insertQuery = $"UPDATE {tableName} SET {columns} {whereCondition}";
                    if (ExecuteQuery(insertQuery)) {
                        CShowMessage.Info("Updated Successfully.", "Success");
                        return true;
                    }
                }
            }
            CShowMessage.Warning(myDTO.Error, "Warning");
            return false;
        }
        public bool UpdateManual<sDTO>(string tableName, string whereCondition, sDTO myDTO, string[] fieldsToExclude = null, string message = null) where sDTO : BaseModel {
            if (myDTO.DataValidation()) {
                if (CShowMessage.Ask(message != null ? message : "Are you sure to update this data?", "Confirmation")) {
                    string[] columnAndValueList;

                    if (fieldsToExclude != null) {
                        GetColumnValueUpdate<sDTO>(myDTO, out columnAndValueList, fieldsToExclude);
                    } else {
                        GetColumnValueUpdate<sDTO>(myDTO, out columnAndValueList);
                    }

                    string columns = string.Join(", ", columnAndValueList);
                    string insertQuery = $"UPDATE {tableName} SET {columns} {whereCondition}";
                    if (ExecuteQuery(insertQuery)) {
                        CShowMessage.Info("Updated Successfully.", "Success");
                        return true;
                    }
                }
            }
            CShowMessage.Warning(myDTO.Error, "Warning");
            return false;
        }
        public bool UpdateDetailsManual<mDTO>(string tableName, string whereCondition, mDTO myDTO) where mDTO : BaseModel {
            if (myDTO.DataValidation()) {
                string[] fieldsToExclude = null;
                string[] columnAndValueList;

                if (fieldsToExclude != null) {
                    GetColumnValueUpdate<mDTO>(myDTO, out columnAndValueList, fieldsToExclude);
                } else {
                    GetColumnValueUpdate<mDTO>(myDTO, out columnAndValueList);
                }

                string columns = string.Join(", ", columnAndValueList);
                string insertQuery = $"UPDATE {tableName} SET {columns} {whereCondition}";
                if (ExecuteQuery(insertQuery)) {
                    CShowMessage.Info("Updated Successfully.", "Success");
                    return true;
                }
            }
            CShowMessage.Warning(myDTO.Error, "Warning");
            return false;
        }
        public bool UpdateDetailsManual<mDTO>(string tableName, string whereCondition, mDTO myDTO, string[] fieldsToExclude = null) where mDTO : BaseModel {
            if (myDTO.DataValidation()) {
                string[] columnAndValueList;

                if (fieldsToExclude != null) {
                    GetColumnValueUpdate<mDTO>(myDTO, out columnAndValueList, fieldsToExclude);
                } else {
                    GetColumnValueUpdate<mDTO>(myDTO, out columnAndValueList);
                }

                string columns = string.Join(", ", columnAndValueList);
                string insertQuery = $"UPDATE {tableName} SET {columns} {whereCondition}";
                if (ExecuteQuery(insertQuery)) {
                    CShowMessage.Info("Updated Successfully.", "Success");
                    return true;
                }
            }
            CShowMessage.Warning(myDTO.Error, "Warning");
            return false;
        }
        public bool UpdateCustom(string queryStatement) {
            if (ExecuteQuery(queryStatement)) {
                CShowMessage.Info("Updated Successfully.", "Info");
                return true;
            }
            return false;
        }
        public bool UpdateDetailsCustom(string queryStatement) {
            if (ExecuteQuery(queryStatement)) {
                return true;
            }
            return false;
        }
        public bool DeleteManual(string tableName, string whereCondition, string message = null) {
            if (CShowMessage.Ask(message != null ? message : "Are you sure to delete this data?", "Confirmation")) {
                string insertQuery = $"DELETE FROM {tableName} {whereCondition}";
                if (ExecuteQuery(insertQuery)) {
                    CShowMessage.Info("Deleted Successfully.", "Success");
                    return true;
                }
            }
            return false;
        }
        public bool DeleteDetailsManual(string tableName, string whereCondition) {
            string insertQuery = $"DELETE FROM {tableName} {whereCondition}";
            if (ExecuteQuery(insertQuery)) {
                return true;
            }
            return false;
        }
        public string GetSaveManualQuery<sDTO>(string tableName, sDTO myDTO, string[] fieldsToExclude = null) where sDTO : BaseModel {
            if (myDTO.DataValidation()) {
                string[] columnList;
                string[] valueList;

                if (fieldsToExclude != null) {
                    GetColumnValue<sDTO>(myDTO, out columnList, out valueList, fieldsToExclude);
                } else {
                    GetColumnValue<sDTO>(myDTO, out columnList, out valueList);
                }

                string columns = string.Join(", ", columnList);
                string values = string.Join(", ", valueList);
                string insertQuery = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
                return insertQuery;

            }
            throw new ArgumentException(myDTO.Error);
        }
        public string GetSaveDetailsManualQuery<sDTO>(string tableName, sDTO myDTO, string[] fieldsToExclude = null) {
            string[] columnList;
            string[] valueList;

            if (fieldsToExclude != null) {
                GetColumnValueForList<sDTO>(myDTO, out columnList, out valueList, fieldsToExclude);
            } else {
                GetColumnValueForList<sDTO>(myDTO, out columnList, out valueList);
            }

            string columns = string.Join(", ", columnList);
            string values = string.Join(", ", valueList);
            string insertQuery = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
            return insertQuery;
        }
        public string GetUpdateDetailsManualQuery<sDTO>(string tableName, string whereCondition, sDTO myDTO, string[] fieldsToExclude = null) {
            string[] columnAndValueList;

            if (fieldsToExclude != null) {
                GetColumnValueUpdate<sDTO>(myDTO, out columnAndValueList, fieldsToExclude);
            } else {
                GetColumnValueUpdate<sDTO>(myDTO, out columnAndValueList);
            }

            string columns = string.Join(", ", columnAndValueList);
            string insertQuery = $"UPDATE {tableName} SET {columns} {whereCondition}";
            return insertQuery;
        }
        public string GetUpdateManualQuery<sDTO>(string tableName, string whereCondition, sDTO myDTO, string[] fieldsToExclude = null) where sDTO : BaseModel {
            if (myDTO.DataValidation()) {
                string[] columnAndValueList;

                if (fieldsToExclude != null) {
                    GetColumnValueUpdate<sDTO>(myDTO, out columnAndValueList, fieldsToExclude);
                } else {
                    GetColumnValueUpdate<sDTO>(myDTO, out columnAndValueList);
                }

                string columns = string.Join(", ", columnAndValueList);
                string insertQuery = $"UPDATE {tableName} SET {columns} {whereCondition}";
                return insertQuery;

            }
            throw new ArgumentException(myDTO.Error);
        }
        public string GetDeleteManualQuery(string tableName, string whereCondition) {
            return $"DELETE FROM {tableName} {whereCondition}";
        }
        public string GetDeleteManualQuery(string tableName, int idTrack) {
            return $"DELETE FROM {tableName} WHERE IdTrack = {idTrack}";
        }
        public void SaveMultipleQuery(List<string> queryStatement, List<string> queryStatement2) {
            if (dbConnection == AllowedOpenDB.Two) {
                if (!string.IsNullOrEmpty(connectionString2)) {
                    if (queryStatement.Count > 0 && queryStatement2.Count > 0) {
                        if (CShowMessage.Ask("Execute Transaction?", "Confirmation")) {
                            if (ExecuteQuery(queryStatement, queryStatement2)) {
                                CShowMessage.Info("Transaction has been successfully commited.", "Success");
                            } else {
                                throw new ArgumentException("Error: Save!");
                            }
                        }
                    }
                } else {
                    throw new ArgumentException("Please set ConnectionString for second database!", "Warning");
                }
            } else {
                throw new ArgumentException("Please set DB Connection allowed 2 databases!", "Warning");
            }
        }
        public void SaveMultipleQuery(List<string> queryStatement) {
            if (queryStatement.Count > 0) {
                if (CShowMessage.Ask("Execute Transaction?", "Confirmation")) {
                    if (ExecuteQuery(queryStatement)) {
                        CShowMessage.Info($"Transaction has been successfully commited with {rowsAffected} rows affected.", "Success");
                    } else {
                        throw new ArgumentException("Error: Save!");
                    }
                }
            }
        }
        public void SaveTransaction<DTO>(DTO myDTO, string _tableName, string _tableDetailsName, List<string> queryToAdd = null) where DTO : BaseModel {
            var MultipleQuery = new List<string>();
            //
            if (queryToAdd != null) {
                foreach (var item in queryToAdd) {
                    MultipleQuery.Add(item);
                }
            }
            //
            MultipleQuery.Add(GetSaveManualQuery(_tableName, myDTO));
            //
            var listProperties = typeof(DTO).GetProperties()
                .Where(c => c.PropertyType.IsGenericType && c.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                .ToList();
            //
            foreach (var listItem in listProperties) {
                var itemToAdd = (IEnumerable)listItem.GetValue(myDTO);
                if (itemToAdd != null) {
                    foreach (var item in itemToAdd) {
                        ListValidation((BaseModel)item);
                        MultipleQuery.Add(GetSaveDetailsManualQuery(_tableDetailsName, item));
                    }
                }
            }
            //
            SaveMultipleQuery(MultipleQuery);
        }
        public void SaveTransaction<DTO>(DTO myDTO, string _tableName, string _tableDetailsName) where DTO : BaseModel {
            var MultipleQuery = new List<string>();
            //
            MultipleQuery.Add(GetSaveManualQuery(_tableName, myDTO));
            //
            var listProperties = typeof(DTO).GetProperties()
                .Where(c => c.PropertyType.IsGenericType && c.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                .ToList();
            //
            foreach (var listItem in listProperties) {
                var itemToAdd = (IEnumerable)listItem.GetValue(myDTO);
                if (itemToAdd != null) {
                    foreach (var item in itemToAdd) {
                        ListValidation((BaseModel)item);
                        MultipleQuery.Add(GetSaveDetailsManualQuery(_tableDetailsName, item));
                    }
                }
            }
            //
            SaveMultipleQuery(MultipleQuery);
        }
        public void SaveTransaction<DTO>(DTO myDTO, string _tableName, string _tableDetailsName, string[] fieldToExclude, string[] fieldToExcludeList) where DTO : BaseModel {
            var MultipleQuery = new List<string>();
            //
            MultipleQuery.Add(GetSaveManualQuery(_tableName, myDTO, fieldToExclude));
            //
            var listProperties = typeof(DTO).GetProperties()
                .Where(c => c.PropertyType.IsGenericType && c.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                .ToList();
            //
            foreach (var listItem in listProperties) {
                var itemToAdd = (IEnumerable)listItem.GetValue(myDTO);
                if (itemToAdd != null) {
                    foreach (var item in itemToAdd) {
                        ListValidation((BaseModel)item);
                        MultipleQuery.Add(GetSaveDetailsManualQuery(_tableDetailsName, item, fieldToExcludeList));
                    }
                }
            }
            //
            SaveMultipleQuery(MultipleQuery);
        }
        public void UpdateTransaction<DTO>(DTO myDTO, string sWhere, string _tableName, string _tableDetailsName, List<string> queryToAdd = null) where DTO : BaseModel {
            var MultipleQuery = new List<string>();
            //
            if (queryToAdd != null) {
                foreach (var item in queryToAdd) {
                    MultipleQuery.Add(item);
                }
            }
            //
            MultipleQuery.Add(GetUpdateManualQuery(_tableName, sWhere, myDTO));
            //
            var listProperties = typeof(DTO).GetProperties()
                .Where(c => c.PropertyType.IsGenericType && c.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                .ToList();
            //
            foreach (var listItem in listProperties) {
                var itemToAdd = (IEnumerable)listItem.GetValue(myDTO);
                if (itemToAdd != null) {
                    foreach (var item in itemToAdd) {
                        ListValidation((BaseModel)item);
                        MultipleQuery.Add(GetUpdateDetailsManualQuery(_tableDetailsName, sWhere, item));
                    }
                }
            }
            //
            SaveMultipleQuery(MultipleQuery);
        }
        public void UpdateTransaction<DTO>(DTO myDTO, string sWhere, string _tableName, string _tableDetailsName, string[] fieldToExclude, string[] fieldToExcludeDetails) where DTO : BaseModel {
            var MultipleQuery = new List<string>();
            //
            MultipleQuery.Add(GetUpdateManualQuery(_tableName, sWhere, myDTO, fieldToExclude));
            //
            var listProperties = typeof(DTO).GetProperties()
                .Where(c => c.PropertyType.IsGenericType && c.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                .ToList();
            //
            foreach (var listItem in listProperties) {
                var itemToAdd = (IEnumerable)listItem.GetValue(myDTO);
                if (itemToAdd != null) {
                    foreach (var item in itemToAdd) {
                        ListValidation((BaseModel)item);
                        MultipleQuery.Add(GetUpdateDetailsManualQuery(_tableDetailsName, sWhere, item, fieldToExcludeDetails));
                    }
                }
            }
            //
            SaveMultipleQuery(MultipleQuery);
        }
        public void DeleteTransaction(string _tableName, string _tableDetailsName, string sWhere, List<string> queryToAdd = null) {
            var MultipleQuery = new List<string>();
            //
            if (queryToAdd != null) {
                foreach (var item in queryToAdd) {
                    MultipleQuery.Add(item);
                }
            }
            //
            MultipleQuery.Add(GetDeleteManualQuery(_tableName, sWhere));
            //
            MultipleQuery.Add(GetDeleteManualQuery(_tableDetailsName, sWhere));
            //
            SaveMultipleQuery(MultipleQuery);
        }
        public void DeleteTransaction(string _tableName, string _tableDetailsName, int idTrack) {
            var MultipleQuery = new List<string>();
            //
            MultipleQuery.Add(GetDeleteManualQuery(_tableName, idTrack));
            //
            MultipleQuery.Add(GetDeleteManualQuery(_tableDetailsName, idTrack));
            //
            SaveMultipleQuery(MultipleQuery);
        }
        private void ListValidation<DTO>(DTO myDTO) where DTO : BaseModel {
            if (!myDTO.DataValidation()) {
                throw new ArgumentException(myDTO.Error);
            }
        }

        public void Dispose() {
            CloseConnection();
        }

    }
}

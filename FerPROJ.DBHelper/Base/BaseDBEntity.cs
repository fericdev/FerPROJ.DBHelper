using FerPROJ.DBHelper.Class;
using FerPROJ.DBHelper.CRUD;
using FerPROJ.DBHelper.Query;
using FerPROJ.Design.Class;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FerPROJ.Design.Class.CEnum;

namespace FerPROJ.DBHelper.Base {
    public abstract class BaseDBEntity<EntityContext, DBConn, TSource, TType> : IDisposable where DBConn : Conn where EntityContext : DbContext where TSource : CValidator {
        public string _tableName { get; set; }
        public string _tableDetailsName { get; set; }
        public EntityContext _ts;
        public DBConn _conn;
        private Conn conn = new Conn();
        private Conn entityConn;

        protected BaseDBEntity() {
            _ts = NewConnection();
            _conn = UseConnectionDBEntity(_ts);
            SetTables();
        }
        protected BaseDBEntity(EntityContext ts) {
            _ts = UseConnection(ts);
            _conn = UseConnectionDBEntity(ts);
            SetTables();
        }
        protected BaseDBEntity(DBConn conn) {
            _conn = UseConnectionDB(conn);
            SetTables();
        }
        private EntityContext NewConnection() {
            return Activator.CreateInstance<EntityContext>();
        }
        private EntityContext UseConnection(EntityContext useConn) {
            return useConn;
        }
        private DBConn UseConnectionDB(DBConn useConn) {
            return useConn;
        }
        private DBConn UseConnectionDBEntity(EntityContext useConn) {
            entityConn = new Conn(useConn);
            return (DBConn)entityConn;
        }
        public void Dispose() {
            _ts.Dispose();
            _conn.CloseConnection();
        }
        protected abstract void SetTables();
        public void SaveDTO(TSource myDTO, bool EnableValidation = false, bool IsWeb = false) {
            if (EnableValidation) {
                if (!myDTO.DataValidation()) {
                    throw new ArgumentException("Failed!");
                }
            }
            if (!myDTO.Success) {
                throw new ArgumentException(myDTO.Error);
            }
            if (!IsWeb) {
                if (CShowMessage.Ask("Are you sure to save this data?", "Confirmation")) {
                    SaveData(myDTO);
                    CShowMessage.Info("Saved Successfully!", "Success");
                }
            }
            else {
                SaveData(myDTO);
            }
        }
        protected abstract void SaveData(TSource myDTO);
        //
        public void UpdateDTO(TSource myDTO, bool EnableValidation = false, bool isWeb = false) {
            if (EnableValidation) {
                if (!myDTO.DataValidation()) {
                    throw new ArgumentException("Failed!");
                }
            }
            if (!myDTO.Success) {
                throw new ArgumentException(myDTO.Error);
            }
            if (!isWeb) {
                if (CShowMessage.Ask("Are you sure to update this data?", "Confirmation")) {
                    UpdateData(myDTO);
                    CShowMessage.Info("Updated Successfully!", "Success");
                }
            }
            else {
                UpdateData(myDTO);
            }
        }
        protected abstract void UpdateData(TSource myDTO);
        //
        public void Delete(TType id, bool isWeb = false) {
            if (!isWeb) {
                if (CShowMessage.Ask("Are you sure to delete this data?", "Confirmation")) {
                    DeleteData(id);
                    CShowMessage.Info("Deleted Successfully!", "Success");
                }
            }
            else {
                DeleteData(id);
            }
        }
        protected abstract void DeleteData(TType id);
        //
        public string SelectAll<T>(string search) where T : new() {
            var columnToSearch = CGet.GetMemberName<T>();
            return $"SELECT * FROM {_tableName} WHERE {MySQLQueryHelper.GetMultipleSearchLIKE(search, columnToSearch)}";
        }
        public string SelectAll() {
            return $"SELECT * FROM {_tableName}";
        }
        public string SelectAll(string sWhere) {
            return $"SELECT * FROM {_tableName} {sWhere}";
        }
        public string SelectAll(DateTime dtpFrom, DateTime dtpTo) {
            string sWhere = $"WHERE {MySQLQueryHelper.GetDateRange(dtpFrom, dtpTo)}";
            return $"SELECT * FROM {_tableName} {sWhere}";
        }
        public string SelectAll<T>(DateTime dtpFrom, DateTime dtpTo, string search) where T : new() {
            var columnToSearch = CGet.GetMemberName<T>();
            string sWhere = $"WHERE {MySQLQueryHelper.GetDateRange(dtpFrom, dtpTo)} AND {MySQLQueryHelper.GetMultipleSearchLIKE(search, columnToSearch)} ";
            return $"SELECT * FROM {_tableName} {sWhere}";
        }
        public string SelectAllDetails<T>(string search) where T : new() {
            var columnToSearch = CGet.GetMemberName<T>();
            return $"SELECT * FROM {_tableDetailsName} WHERE {MySQLQueryHelper.GetMultipleSearchLIKE(search, columnToSearch)}";
        }
        public string OrderBy(string columnName = "DateReference", Sort sort = Sort.DESC) {
            return $" ORDER BY {columnName} {sort}";
        }
        public string Status(Status status = CEnum.Status.ACTIVE) {
            return $" AND Status = '{status}' ";
        }
        public string StatusWhere(Status status = CEnum.Status.ACTIVE) {
            return $" WHERE Status = '{status}' ";
        }
    }
}

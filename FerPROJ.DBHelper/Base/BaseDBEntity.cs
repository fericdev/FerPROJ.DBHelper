using FerPROJ.DBHelper.CRUD;
using FerPROJ.Design.Class;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.Base
{
    public abstract class BaseDBEntity<EntityContext, DBConn, TSource, TType> : IDisposable where DBConn : Conn where EntityContext : DbContext where TSource : CValidator where TType : class
    {
        public string _tableName { get; set; }
        public string _tableDetailsName { get; set; }
        public EntityContext _ts;
        public DBConn _conn;
        private Conn conn = new Conn();

        protected BaseDBEntity() {
            _ts = NewConnection();
            _conn = NewConnectionDB();
            SetTables();
        }
        protected BaseDBEntity(EntityContext ts) {
            _ts = UseConnection(ts);
            SetTables();
        }
        protected BaseDBEntity(DBConn conn) {
            _conn = UseConnectionDB(conn);
            SetTables();
        }
        private DBConn NewConnectionDB() {
            return (DBConn)conn;
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
        public void Dispose() {
            _ts.Dispose();
            _conn.CloseConnection();
        }
        protected abstract void SetTables();
        public void SaveDTO(TSource myDTO, bool EnableValidation = false) {
            if (EnableValidation) {
                if (!myDTO.DataValidation()) {
                    throw new ArgumentException("Failed!");
                }
            }
            if (!myDTO.Success) {
                throw new ArgumentException(myDTO.Error);
            }
            if (CShowMessage.Ask("Are you sure to save this data?", "Confirmation")) {
                SaveData(myDTO);
                CShowMessage.Info("Saved Successfully!", "Success");
            }
        }
        protected abstract void SaveData(TSource myDTO);
        //
        public void UpdateDTO(TSource myDTO, bool EnableValidation = false) {
            if (EnableValidation) {
                if (!myDTO.DataValidation()) {
                    throw new ArgumentException("Failed!");
                }
            }
            if (!myDTO.Success) {
                throw new ArgumentException(myDTO.Error);
            }
            if (CShowMessage.Ask("Are you sure to update this data?", "Confirmation")) {
                UpdateData(myDTO);
                CShowMessage.Info("Updated Successfully!", "Success");
            }
        }
        protected abstract void UpdateData(TSource myDTO);
        //
        public void Delete(TType id) {
            if (CShowMessage.Ask("Are you sure to delete this data?", "Confirmation")) {
                DeleteData(id);
                CShowMessage.Info("Deleted Successfully!", "Success");
            }
        }
        protected abstract void DeleteData(TType id);
        //
        public string SelectAll(string sWhere = null) {
            return $"SELECT * FROM {_tableName} {sWhere}";
        }
        public string SelectAllDetails(string sWhere = null) {
            return $"SELECT * FROM {_tableDetailsName} {sWhere}";
        }
    }
}

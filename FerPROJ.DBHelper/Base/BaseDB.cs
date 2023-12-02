using FerPROJ.DBHelper.CRUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.Base
{
    public abstract class BaseDB<DBHelper> : IDisposable where DBHelper : Conn
    {
        public DBHelper _conn;
        private Conn conn = new Conn();
        public string _tableName { get; set; }
        public string _tableDetailsName { get;  set; }

        protected BaseDB()
        {
            SetTables();
            _conn = NewConnection();
        }
        protected BaseDB(DBHelper useConn)
        {
            SetTables();
            _conn = UseConnection(useConn);
        }
        private DBHelper NewConnection() 
        {
            return (DBHelper)conn;
        }
        private DBHelper UseConnection(DBHelper useConn)
        {
            return useConn;
        }
        protected abstract void SetTables();

        public void Dispose()
        {           
            _conn.CloseConnection();
        }
        public string _selectStatement(string sWhere = null)
        {
            return $"SELECT * FROM {_tableName} {sWhere}";
        }
        public string _selectStatementDetails(string sWhere = null)
        {
            return $"SELECT * FROM {_tableDetailsName} {sWhere}";
        }
    }
  
}

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
        private DBHelper NewConnection() 
        {
            return (DBHelper)conn;
        }
        protected abstract void SetTables();

        public void Dispose()
        {           
            _conn.CloseConnection();
        }
    }
  
}

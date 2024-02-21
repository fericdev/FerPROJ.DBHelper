using FerPROJ.Design.Class;
using FerPROJ.Design.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FerPROJ.DBHelper.Generate
{
    public partial class ManageCreateTableClass : FrmManage
    {
        GTableClass generateClass;
        public ManageCreateTableClass()
        {
            InitializeComponent();
        }
        private string GetConnection()
        {
            return $"Server={hostnamecTextBoxBasic1.Text};Port={portcTextBoxBasic4.Text};Uid={usernamecTextBoxBasic2.Text};Pwd={passwordcTextBoxBasic3.Text};";
        }

        private void generatecButton1_Click(object sender, EventArgs e)
        {
            listOfDBCDatagridview1.Rows.Clear();
            generateClass = new GTableClass(GetConnection());
            //
            foreach(DataRow row in generateClass.GetListOfDatabases().Rows)
            {
                string databaseName = row["database_name"].ToString();
                int nIndex = listOfDBCDatagridview1.Rows.Add();
                listOfDBCDatagridview1[DatabaseName.Index, nIndex].Value = databaseName;
            }
        }
        protected override bool OnSaveData()
        {
            if(listOfDBCDatagridview1.GetSelectedValue(DatabaseName.Index, out string sout))
            {
                generateClass.GenerateClass(sout);
                return true;
            }
            return false;
        }
        protected override bool OnSaveNewData()
        {
            if (listOfDBCDatagridview1.GetSelectedValue(DatabaseName.Index, out string sout))
            {
                generateClass.GenerateClass(sout);
                return true;
            }
            return false;
        }
    }
}

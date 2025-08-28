
using FerPROJ.DBHelper.Helper;
using FerPROJ.Design.Class;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FerPROJ.DBHelper.Forms {
    public partial class FrmConf : Form {
        public FrmConf() {
            InitializeComponent();
        }

        private void saveConfigCustomButton_Click(object sender, EventArgs e) {
            if (CShowMessage.Ask("Save?", "Confirmation")) {
                string sslMode = cbSSL.Checked ? "SslMode=None;" : "SslMode=Preferred;";
                CSet.SetEntityConnectionString(hostnameCustomTextBox.Text, usernameCustomTextBox.Text, passwordCustomTextBox.Text, portCustomTextBox.Text, databaseNameCustomTextBox.Text, sslMode);
                UpdateConfigurationFile();
                this.Close();
            }
        }

        private void FrmConnectionSettings_Load(object sender, EventArgs e) {
            databaseNameCustomTextBox.Text = CConfigurationManager.GetValue("DatabaseName", "DatabaseConfig");
            portCustomTextBox.Text = CConfigurationManager.GetValue("Port", "DatabaseConfig");
            usernameCustomTextBox.Text = CConfigurationManager.GetValue("Uid", "DatabaseConfig");
            passwordCustomTextBox.Text = CConfigurationManager.GetValue("Pwd", "DatabaseConfig");
            hostnameCustomTextBox.Text = CConfigurationManager.GetValue("Server", "DatabaseConfig");
            cbSSL.Checked = CConfigurationManager.GetValue("SslMode", "DatabaseConfig") == "None";
        }
        private void UpdateConfigurationFile() {
            CConfigurationManager.CreateOrSetValue("DatabaseName", databaseNameCustomTextBox.Text, "DatabaseConfig");
            CConfigurationManager.CreateOrSetValue("Port", portCustomTextBox.Text, "DatabaseConfig");
            CConfigurationManager.CreateOrSetValue("Uid", usernameCustomTextBox.Text, "DatabaseConfig");
            CConfigurationManager.CreateOrSetValue("Pwd", passwordCustomTextBox.Text, "DatabaseConfig");
            CConfigurationManager.CreateOrSetValue("Server", hostnameCustomTextBox.Text, "DatabaseConfig");
            CConfigurationManager.CreateOrSetValue("SslMode", cbSSL.Checked ? "None" : "Preferred", "DatabaseConfig");
            CShowMessage.Info("Database Configuration Updated Successfully!", "Info");
        }

        private void cButtonRunMigration_Click(object sender, EventArgs e) {
            if (CShowMessage.Ask("Run Database Migration?", "Confirmation")) {

                DBHelpers.RunDatabaseMigration();

                CShowMessage.Info("Database Updated Successfully!", "Info");
            }
        }
    }
}


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
            databaseNameCustomTextBox.Text = CLibFilesReader.GetValue("DatabaseName", "DatabaseConfig");
            portCustomTextBox.Text = CLibFilesReader.GetValue("Port", "DatabaseConfig");
            usernameCustomTextBox.Text = CLibFilesReader.GetValue("Uid", "DatabaseConfig");
            passwordCustomTextBox.Text = CLibFilesReader.GetValue("Pwd", "DatabaseConfig");
            hostnameCustomTextBox.Text = CLibFilesReader.GetValue("Server", "DatabaseConfig");
            cbSSL.Checked = CLibFilesReader.GetValue("SslMode", "DatabaseConfig") == "None";
        }
        private void UpdateConfigurationFile() {
            CLibFilesWriter.CreateOrSetValue("DatabaseName", databaseNameCustomTextBox.Text, "DatabaseConfig");
            CLibFilesWriter.CreateOrSetValue("Port", portCustomTextBox.Text, "DatabaseConfig");
            CLibFilesWriter.CreateOrSetValue("Uid", usernameCustomTextBox.Text, "DatabaseConfig");
            CLibFilesWriter.CreateOrSetValue("Pwd", passwordCustomTextBox.Text, "DatabaseConfig");
            CLibFilesWriter.CreateOrSetValue("Server", hostnameCustomTextBox.Text, "DatabaseConfig");
            CLibFilesWriter.CreateOrSetValue("SslMode", cbSSL.Checked ? "None" : "Preferred", "DatabaseConfig");
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

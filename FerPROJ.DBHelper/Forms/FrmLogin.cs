using FerPROJ.DBHelper.Helper;
using FerPROJ.DBHelper.Repository;
using FerPROJ.Design.Class;
using FerPROJ.Design.Forms;
using FerPROJ.Design.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FerPROJ.DBHelper.Forms {
    public partial class FrmLogin : FrmManageKrypton {
        UserModel userModel = new UserModel();
        public FrmLogin() {
            InitializeComponent();
        }
        protected override async Task LoadComponentsAsync() {
            userModelBindingSource.DataSource = userModel;
            userModel.UserName = CConfigurationManager.GetRememberedUsername(cbRememberMe, userNameCTextBoxKrypton);
            await Task.CompletedTask;
        }
        private async void cButtonLogin_Click(object sender, EventArgs e) {
            if (await OnSaveDataAsync()) {
                CurrentFormResult = Task.FromResult(true);
                this.Close();
            }
        }
        protected override async Task<bool> OnSaveDataAsync() {
            using (var repo = new UserRepository()) {
                return await repo.CheckCredentialsAsync(userModel);
            }
        }

        private void cButtonClose_Click(object sender, EventArgs e) {
            Application.Exit();
        }
        protected override void InitializeKeyboardShortcuts() {
            keyboardShortcuts[Keys.F1] = DBHelpers.OpenDatabaseConfiguration;
        }
    }
}

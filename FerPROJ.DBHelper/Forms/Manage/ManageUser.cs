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
using static FerPROJ.Design.Class.CBaseEnums;

namespace FerPROJ.DBHelper.Forms.Manage {
    public partial class ManageUser : FrmManageKrypton {
        UserModel model = new UserModel();
        public ManageUser() {
            InitializeComponent();
        }
        protected override async Task LoadComponentsAsync() {
            userRoleCComboBoxKrypton.FillComboBoxEnum<CBaseEnums.Role>();
            switch (CurrentFormMode) {
                case FormMode.Add:
                    break;
                case FormMode.Update:
                    using (var repo = new UserRepository()) {
                        var entity = await repo.GetByIdAsync(Manage_IdTrack);
                        model = entity.ToDestination<UserModel>();
                        model.Password = CEncryptionManager.DecryptText(entity.Password);
                    }
                    break;
                default:
                    break;
            }
            userModelBindingSource.DataSource = model;
        }
        protected override async Task<bool> OnSaveDataAsync() {
            using (var repo = new UserRepository()) {
                return await repo.SaveDTOAsync(model, true);
            }
        }
        protected override async Task<bool> OnUpdateDataAsync() {
            using (var repo = new UserRepository()) {
                return await repo.UpdateDTOAsync(model, true);
            }
        }
    }
}

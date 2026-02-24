using FerPROJ.DBHelper.Forms.Manage;
using FerPROJ.DBHelper.Repository;
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
using static FerPROJ.Design.Class.CBaseEnums;
using static FerPROJ.Design.Forms.FrmManageKrypton;

namespace FerPROJ.DBHelper.Forms.List {
    public partial class ListUser : FrmListKrypton {
        public ListUser() {
            InitializeComponent();
        }
        protected override async Task InitializeFormPropertiesAsync() {
            MainModelDataGridView = userModelCDatagridview;
            await base.InitializeFormPropertiesAsync();
        }
        protected override async Task RefreshDataAsync() {
            using(var repo = new UserRepository()) {
                var users = repo.GetViewModelWithSearchAsync(searchValue, dateFrom, dateTo);
                await userModelBindingSource.LoadDataAsync(users, ComboBoxKryptonPage, ComboBoxKryptonDataLimit, dataPage, dataLimit);
            }
        }
        protected override async Task<bool> AddNewItemAsync() {
            using (var frm = new ManageUser()) {
                frm.Manage_IdTrack = Guid.Empty;
                frm.CurrentFormMode = FormMode.Add;
                frm.ShowDialog();
                return await frm.CurrentFormResult;
            }
        }
        protected override async Task<bool> UpdateItemAsync() {
            if(userModelCDatagridview.GetSelectedValue(UserID.Index, out Form_IdTrack)) {
                using (var frm = new ManageUser()) {
                    frm.Manage_IdTrack = Form_IdTrack.ToGuid();
                    frm.CurrentFormMode = FormMode.Update;
                    frm.ShowDialog();
                    return await frm.CurrentFormResult;
                }
            }
            return false;
        }
        protected override async Task<bool> DeleteItemAsync() {
            if (userModelCDatagridview.GetSelectedValue(UserID.Index, out Form_IdTrack)) {
                using (var repo = new UserRepository()) {
                    return await repo.DeleteByIdAsync(Form_IdTrack.ToGuid());
                }
            }
            return false;
        }
    }
}

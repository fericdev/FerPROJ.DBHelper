using FerPROJ.DBHelper.ApiRepository;
using FerPROJ.DBHelper.Repository;
using FerPROJ.Design.Class;
using FerPROJ.Design.FormModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FerPROJ.Design.Class.CBaseEnums;

namespace FerPROJ.Design.Forms {
    public partial class ManageSystemCompany : FrmManageKrypton {
        SystemCompanyModel model = new SystemCompanyModel();
        public ManageSystemCompany() {
            InitializeComponent();
        }
        protected override async Task LoadComponentsAsync() {
            switch (CurrentFormMode) {
                case FormMode.Add:
                    break;
                case FormMode.Update:
                    if (CAppConstants.API_ENABLED) {
                        model = await new SystemCompanyApiRepository().GetPrepareModelByIdAsync(Manage_IdTrack);
                    }
                    else {
                        model = await new SystemCompanyRepository().ExecuteAsync(c => c.GetPrepareModelByIdAsync(Manage_IdTrack));
                    }
                    break;

            }
            companyModelBindingSource.DataSource = model;
        }
        protected override async Task<bool> OnSaveDataAsync() {
            if (CAppConstants.API_ENABLED) {
                return await new SystemCompanyApiRepository().SaveModelAsync(model);
            }
            else {
                return await new SystemCompanyRepository().ExecuteAsync(c => c.SaveDTOAsync(model));
            }
        }
        protected override async Task<bool> OnUpdateDataAsync() {
            if (CAppConstants.API_ENABLED) {
                return await new SystemCompanyApiRepository().UpdateModelAsync(model);
            }
            else {
                return await new SystemCompanyRepository().ExecuteAsync(c => c.UpdateDTOAsync(model));
            }
        }

        private void selectLogoLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            using (OpenFileDialog ofd = new OpenFileDialog()) {
                ofd.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tif;*.tiff;*.ico;*.webp)|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tif;*.tiff;*.ico;*.webp|All Files (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK) {
                    model.CompanyLogoUrl = ofd.FileName;
                    model.CompanyLogo = File.ReadAllBytes(ofd.FileName);
                    companyModelBindingSource.ResetBindings(false);
                }
            }
        }
    }
}

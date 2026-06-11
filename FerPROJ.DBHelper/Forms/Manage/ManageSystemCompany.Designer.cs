namespace FerPROJ.Design.Forms {
    partial class ManageSystemCompany {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label nameLabel;
            System.Windows.Forms.Label companyEmailLabel;
            System.Windows.Forms.Label companyContactNoLabel;
            System.Windows.Forms.Label companyAddressLabel;
            System.Windows.Forms.Label label1;
            this.companyModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.nameCTextBoxKrypton = new FerPROJ.Design.Controls.CTextBoxKrypton();
            this.companyEmailCTextBoxKrypton = new FerPROJ.Design.Controls.CTextBoxKrypton();
            this.companyContactNoCTextBoxKrypton = new FerPROJ.Design.Controls.CTextBoxKrypton();
            this.companyAddressCTextBoxKrypton = new FerPROJ.Design.Controls.CTextBoxKrypton();
            this.companyLogoCTextBoxKrypton = new FerPROJ.Design.Controls.CTextBoxKrypton();
            this.selectLogoLinkLabel = new System.Windows.Forms.LinkLabel();
            this.cTextBoxKrypton1 = new FerPROJ.Design.Controls.CTextBoxKrypton();
            this.kryptonCheckBox1 = new Krypton.Toolkit.KryptonCheckBox();
            nameLabel = new System.Windows.Forms.Label();
            companyEmailLabel = new System.Windows.Forms.Label();
            companyContactNoLabel = new System.Windows.Forms.Label();
            companyAddressLabel = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            this.basePnl2.SuspendLayout();
            this.PanelMain3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.companyModelBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // basePnl2
            // 
            this.basePnl2.Location = new System.Drawing.Point(218, 1);
            // 
            // baseButtonUpdate
            // 
            this.baseButtonUpdate.FlatAppearance.BorderSize = 0;
            // 
            // baseButtonSave
            // 
            this.baseButtonSave.FlatAppearance.BorderSize = 0;
            // 
            // baseButtonCancel
            // 
            this.baseButtonCancel.FlatAppearance.BorderSize = 0;
            // 
            // PanelMain3
            // 
            this.PanelMain3.Controls.Add(this.kryptonCheckBox1);
            this.PanelMain3.Controls.Add(label1);
            this.PanelMain3.Controls.Add(this.cTextBoxKrypton1);
            this.PanelMain3.Controls.Add(this.selectLogoLinkLabel);
            this.PanelMain3.Controls.Add(this.companyLogoCTextBoxKrypton);
            this.PanelMain3.Controls.Add(companyAddressLabel);
            this.PanelMain3.Controls.Add(this.companyAddressCTextBoxKrypton);
            this.PanelMain3.Controls.Add(companyContactNoLabel);
            this.PanelMain3.Controls.Add(this.companyContactNoCTextBoxKrypton);
            this.PanelMain3.Controls.Add(companyEmailLabel);
            this.PanelMain3.Controls.Add(this.companyEmailCTextBoxKrypton);
            this.PanelMain3.Controls.Add(nameLabel);
            this.PanelMain3.Controls.Add(this.nameCTextBoxKrypton);
            this.PanelMain3.Size = new System.Drawing.Size(453, 413);
            // 
            // baseButtonAddNew
            // 
            this.baseButtonAddNew.FlatAppearance.BorderSize = 0;
            // 
            // panelMain1
            // 
            this.panelMain1.Size = new System.Drawing.Size(453, 73);
            // 
            // nameLabel
            // 
            nameLabel.AutoSize = true;
            nameLabel.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            nameLabel.Location = new System.Drawing.Point(45, 14);
            nameLabel.Name = "nameLabel";
            nameLabel.Size = new System.Drawing.Size(62, 19);
            nameLabel.TabIndex = 0;
            nameLabel.Text = "Name:";
            // 
            // companyEmailLabel
            // 
            companyEmailLabel.AutoSize = true;
            companyEmailLabel.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            companyEmailLabel.Location = new System.Drawing.Point(45, 136);
            companyEmailLabel.Name = "companyEmailLabel";
            companyEmailLabel.Size = new System.Drawing.Size(140, 19);
            companyEmailLabel.TabIndex = 2;
            companyEmailLabel.Text = "Company Email:";
            // 
            // companyContactNoLabel
            // 
            companyContactNoLabel.AutoSize = true;
            companyContactNoLabel.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            companyContactNoLabel.Location = new System.Drawing.Point(45, 197);
            companyContactNoLabel.Name = "companyContactNoLabel";
            companyContactNoLabel.Size = new System.Drawing.Size(185, 19);
            companyContactNoLabel.TabIndex = 4;
            companyContactNoLabel.Text = "Company Contact No:";
            // 
            // companyAddressLabel
            // 
            companyAddressLabel.AutoSize = true;
            companyAddressLabel.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            companyAddressLabel.Location = new System.Drawing.Point(45, 257);
            companyAddressLabel.Name = "companyAddressLabel";
            companyAddressLabel.Size = new System.Drawing.Size(160, 19);
            companyAddressLabel.TabIndex = 6;
            companyAddressLabel.Text = "Company Address:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label1.Location = new System.Drawing.Point(45, 76);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(151, 19);
            label1.TabIndex = 11;
            label1.Text = "Company Branch:";
            // 
            // companyModelBindingSource
            // 
            this.companyModelBindingSource.DataSource = typeof(FerPROJ.Design.FormModels.SystemCompanyModel);
            // 
            // nameCTextBoxKrypton
            // 
            this.nameCTextBoxKrypton.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.companyModelBindingSource, "Name", true));
            this.nameCTextBoxKrypton.Location = new System.Drawing.Point(45, 36);
            this.nameCTextBoxKrypton.Name = "nameCTextBoxKrypton";
            this.nameCTextBoxKrypton.Size = new System.Drawing.Size(367, 29);
            this.nameCTextBoxKrypton.StateActive.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.nameCTextBoxKrypton.StateActive.Border.Color1 = System.Drawing.Color.DarkGray;
            this.nameCTextBoxKrypton.StateActive.Border.Color2 = System.Drawing.Color.White;
            this.nameCTextBoxKrypton.StateActive.Border.Rounding = 10F;
            this.nameCTextBoxKrypton.StateActive.Content.Color1 = System.Drawing.Color.Black;
            this.nameCTextBoxKrypton.StateCommon.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.nameCTextBoxKrypton.StateCommon.Border.Color1 = System.Drawing.Color.White;
            this.nameCTextBoxKrypton.StateCommon.Border.Color2 = System.Drawing.Color.White;
            this.nameCTextBoxKrypton.StateCommon.Border.Rounding = 10F;
            this.nameCTextBoxKrypton.StateCommon.Content.Color1 = System.Drawing.Color.Black;
            this.nameCTextBoxKrypton.StateDisabled.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.nameCTextBoxKrypton.StateDisabled.Border.Color1 = System.Drawing.Color.White;
            this.nameCTextBoxKrypton.StateDisabled.Border.Color2 = System.Drawing.Color.White;
            this.nameCTextBoxKrypton.StateDisabled.Border.Rounding = 10F;
            this.nameCTextBoxKrypton.StateDisabled.Content.Color1 = System.Drawing.Color.Black;
            this.nameCTextBoxKrypton.StateNormal.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.nameCTextBoxKrypton.StateNormal.Border.Color1 = System.Drawing.Color.White;
            this.nameCTextBoxKrypton.StateNormal.Border.Color2 = System.Drawing.Color.White;
            this.nameCTextBoxKrypton.StateNormal.Border.Rounding = 10F;
            this.nameCTextBoxKrypton.StateNormal.Content.Color1 = System.Drawing.Color.Black;
            this.nameCTextBoxKrypton.TabIndex = 1;
            this.nameCTextBoxKrypton.Text = "cTextBoxKrypton1";
            // 
            // companyEmailCTextBoxKrypton
            // 
            this.companyEmailCTextBoxKrypton.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.companyModelBindingSource, "CompanyEmail", true));
            this.companyEmailCTextBoxKrypton.Location = new System.Drawing.Point(45, 158);
            this.companyEmailCTextBoxKrypton.Name = "companyEmailCTextBoxKrypton";
            this.companyEmailCTextBoxKrypton.Size = new System.Drawing.Size(367, 29);
            this.companyEmailCTextBoxKrypton.StateActive.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.companyEmailCTextBoxKrypton.StateActive.Border.Color1 = System.Drawing.Color.DarkGray;
            this.companyEmailCTextBoxKrypton.StateActive.Border.Color2 = System.Drawing.Color.White;
            this.companyEmailCTextBoxKrypton.StateActive.Border.Rounding = 10F;
            this.companyEmailCTextBoxKrypton.StateActive.Content.Color1 = System.Drawing.Color.Black;
            this.companyEmailCTextBoxKrypton.StateCommon.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.companyEmailCTextBoxKrypton.StateCommon.Border.Color1 = System.Drawing.Color.White;
            this.companyEmailCTextBoxKrypton.StateCommon.Border.Color2 = System.Drawing.Color.White;
            this.companyEmailCTextBoxKrypton.StateCommon.Border.Rounding = 10F;
            this.companyEmailCTextBoxKrypton.StateCommon.Content.Color1 = System.Drawing.Color.Black;
            this.companyEmailCTextBoxKrypton.StateDisabled.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.companyEmailCTextBoxKrypton.StateDisabled.Border.Color1 = System.Drawing.Color.White;
            this.companyEmailCTextBoxKrypton.StateDisabled.Border.Color2 = System.Drawing.Color.White;
            this.companyEmailCTextBoxKrypton.StateDisabled.Border.Rounding = 10F;
            this.companyEmailCTextBoxKrypton.StateDisabled.Content.Color1 = System.Drawing.Color.Black;
            this.companyEmailCTextBoxKrypton.StateNormal.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.companyEmailCTextBoxKrypton.StateNormal.Border.Color1 = System.Drawing.Color.White;
            this.companyEmailCTextBoxKrypton.StateNormal.Border.Color2 = System.Drawing.Color.White;
            this.companyEmailCTextBoxKrypton.StateNormal.Border.Rounding = 10F;
            this.companyEmailCTextBoxKrypton.StateNormal.Content.Color1 = System.Drawing.Color.Black;
            this.companyEmailCTextBoxKrypton.TabIndex = 3;
            this.companyEmailCTextBoxKrypton.Text = "cTextBoxKrypton1";
            // 
            // companyContactNoCTextBoxKrypton
            // 
            this.companyContactNoCTextBoxKrypton.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.companyModelBindingSource, "CompanyContactNo", true));
            this.companyContactNoCTextBoxKrypton.Location = new System.Drawing.Point(45, 219);
            this.companyContactNoCTextBoxKrypton.Name = "companyContactNoCTextBoxKrypton";
            this.companyContactNoCTextBoxKrypton.Size = new System.Drawing.Size(367, 29);
            this.companyContactNoCTextBoxKrypton.StateActive.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.companyContactNoCTextBoxKrypton.StateActive.Border.Color1 = System.Drawing.Color.DarkGray;
            this.companyContactNoCTextBoxKrypton.StateActive.Border.Color2 = System.Drawing.Color.White;
            this.companyContactNoCTextBoxKrypton.StateActive.Border.Rounding = 10F;
            this.companyContactNoCTextBoxKrypton.StateActive.Content.Color1 = System.Drawing.Color.Black;
            this.companyContactNoCTextBoxKrypton.StateCommon.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.companyContactNoCTextBoxKrypton.StateCommon.Border.Color1 = System.Drawing.Color.White;
            this.companyContactNoCTextBoxKrypton.StateCommon.Border.Color2 = System.Drawing.Color.White;
            this.companyContactNoCTextBoxKrypton.StateCommon.Border.Rounding = 10F;
            this.companyContactNoCTextBoxKrypton.StateCommon.Content.Color1 = System.Drawing.Color.Black;
            this.companyContactNoCTextBoxKrypton.StateDisabled.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.companyContactNoCTextBoxKrypton.StateDisabled.Border.Color1 = System.Drawing.Color.White;
            this.companyContactNoCTextBoxKrypton.StateDisabled.Border.Color2 = System.Drawing.Color.White;
            this.companyContactNoCTextBoxKrypton.StateDisabled.Border.Rounding = 10F;
            this.companyContactNoCTextBoxKrypton.StateDisabled.Content.Color1 = System.Drawing.Color.Black;
            this.companyContactNoCTextBoxKrypton.StateNormal.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.companyContactNoCTextBoxKrypton.StateNormal.Border.Color1 = System.Drawing.Color.White;
            this.companyContactNoCTextBoxKrypton.StateNormal.Border.Color2 = System.Drawing.Color.White;
            this.companyContactNoCTextBoxKrypton.StateNormal.Border.Rounding = 10F;
            this.companyContactNoCTextBoxKrypton.StateNormal.Content.Color1 = System.Drawing.Color.Black;
            this.companyContactNoCTextBoxKrypton.TabIndex = 5;
            this.companyContactNoCTextBoxKrypton.Text = "cTextBoxKrypton1";
            // 
            // companyAddressCTextBoxKrypton
            // 
            this.companyAddressCTextBoxKrypton.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.companyModelBindingSource, "CompanyAddress", true));
            this.companyAddressCTextBoxKrypton.Location = new System.Drawing.Point(45, 279);
            this.companyAddressCTextBoxKrypton.Name = "companyAddressCTextBoxKrypton";
            this.companyAddressCTextBoxKrypton.Size = new System.Drawing.Size(367, 29);
            this.companyAddressCTextBoxKrypton.StateActive.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.companyAddressCTextBoxKrypton.StateActive.Border.Color1 = System.Drawing.Color.DarkGray;
            this.companyAddressCTextBoxKrypton.StateActive.Border.Color2 = System.Drawing.Color.White;
            this.companyAddressCTextBoxKrypton.StateActive.Border.Rounding = 10F;
            this.companyAddressCTextBoxKrypton.StateActive.Content.Color1 = System.Drawing.Color.Black;
            this.companyAddressCTextBoxKrypton.StateCommon.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.companyAddressCTextBoxKrypton.StateCommon.Border.Color1 = System.Drawing.Color.White;
            this.companyAddressCTextBoxKrypton.StateCommon.Border.Color2 = System.Drawing.Color.White;
            this.companyAddressCTextBoxKrypton.StateCommon.Border.Rounding = 10F;
            this.companyAddressCTextBoxKrypton.StateCommon.Content.Color1 = System.Drawing.Color.Black;
            this.companyAddressCTextBoxKrypton.StateDisabled.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.companyAddressCTextBoxKrypton.StateDisabled.Border.Color1 = System.Drawing.Color.White;
            this.companyAddressCTextBoxKrypton.StateDisabled.Border.Color2 = System.Drawing.Color.White;
            this.companyAddressCTextBoxKrypton.StateDisabled.Border.Rounding = 10F;
            this.companyAddressCTextBoxKrypton.StateDisabled.Content.Color1 = System.Drawing.Color.Black;
            this.companyAddressCTextBoxKrypton.StateNormal.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.companyAddressCTextBoxKrypton.StateNormal.Border.Color1 = System.Drawing.Color.White;
            this.companyAddressCTextBoxKrypton.StateNormal.Border.Color2 = System.Drawing.Color.White;
            this.companyAddressCTextBoxKrypton.StateNormal.Border.Rounding = 10F;
            this.companyAddressCTextBoxKrypton.StateNormal.Content.Color1 = System.Drawing.Color.Black;
            this.companyAddressCTextBoxKrypton.TabIndex = 7;
            this.companyAddressCTextBoxKrypton.Text = "cTextBoxKrypton1";
            // 
            // companyLogoCTextBoxKrypton
            // 
            this.companyLogoCTextBoxKrypton.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.companyModelBindingSource, "CompanyLogoUrl", true));
            this.companyLogoCTextBoxKrypton.Location = new System.Drawing.Point(45, 338);
            this.companyLogoCTextBoxKrypton.Name = "companyLogoCTextBoxKrypton";
            this.companyLogoCTextBoxKrypton.Size = new System.Drawing.Size(367, 29);
            this.companyLogoCTextBoxKrypton.StateActive.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.companyLogoCTextBoxKrypton.StateActive.Border.Color1 = System.Drawing.Color.DarkGray;
            this.companyLogoCTextBoxKrypton.StateActive.Border.Color2 = System.Drawing.Color.White;
            this.companyLogoCTextBoxKrypton.StateActive.Border.Rounding = 10F;
            this.companyLogoCTextBoxKrypton.StateActive.Content.Color1 = System.Drawing.Color.Black;
            this.companyLogoCTextBoxKrypton.StateCommon.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.companyLogoCTextBoxKrypton.StateCommon.Border.Color1 = System.Drawing.Color.White;
            this.companyLogoCTextBoxKrypton.StateCommon.Border.Color2 = System.Drawing.Color.White;
            this.companyLogoCTextBoxKrypton.StateCommon.Border.Rounding = 10F;
            this.companyLogoCTextBoxKrypton.StateCommon.Content.Color1 = System.Drawing.Color.Black;
            this.companyLogoCTextBoxKrypton.StateDisabled.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.companyLogoCTextBoxKrypton.StateDisabled.Border.Color1 = System.Drawing.Color.White;
            this.companyLogoCTextBoxKrypton.StateDisabled.Border.Color2 = System.Drawing.Color.White;
            this.companyLogoCTextBoxKrypton.StateDisabled.Border.Rounding = 10F;
            this.companyLogoCTextBoxKrypton.StateDisabled.Content.Color1 = System.Drawing.Color.Black;
            this.companyLogoCTextBoxKrypton.StateNormal.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.companyLogoCTextBoxKrypton.StateNormal.Border.Color1 = System.Drawing.Color.White;
            this.companyLogoCTextBoxKrypton.StateNormal.Border.Color2 = System.Drawing.Color.White;
            this.companyLogoCTextBoxKrypton.StateNormal.Border.Rounding = 10F;
            this.companyLogoCTextBoxKrypton.StateNormal.Content.Color1 = System.Drawing.Color.Black;
            this.companyLogoCTextBoxKrypton.TabIndex = 9;
            this.companyLogoCTextBoxKrypton.Text = "cTextBoxKrypton1";
            // 
            // selectLogoLinkLabel
            // 
            this.selectLogoLinkLabel.AutoSize = true;
            this.selectLogoLinkLabel.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.selectLogoLinkLabel.Location = new System.Drawing.Point(45, 316);
            this.selectLogoLinkLabel.Name = "selectLogoLinkLabel";
            this.selectLogoLinkLabel.Size = new System.Drawing.Size(134, 19);
            this.selectLogoLinkLabel.TabIndex = 10;
            this.selectLogoLinkLabel.TabStop = true;
            this.selectLogoLinkLabel.Text = "Company Logo:";
            this.selectLogoLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.selectLogoLinkLabel_LinkClicked);
            // 
            // cTextBoxKrypton1
            // 
            this.cTextBoxKrypton1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.companyModelBindingSource, "ApplicationId", true));
            this.cTextBoxKrypton1.Location = new System.Drawing.Point(45, 98);
            this.cTextBoxKrypton1.Name = "cTextBoxKrypton1";
            this.cTextBoxKrypton1.Size = new System.Drawing.Size(367, 29);
            this.cTextBoxKrypton1.StateActive.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.cTextBoxKrypton1.StateActive.Border.Color1 = System.Drawing.Color.DarkGray;
            this.cTextBoxKrypton1.StateActive.Border.Color2 = System.Drawing.Color.White;
            this.cTextBoxKrypton1.StateActive.Border.Rounding = 10F;
            this.cTextBoxKrypton1.StateActive.Content.Color1 = System.Drawing.Color.Black;
            this.cTextBoxKrypton1.StateCommon.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.cTextBoxKrypton1.StateCommon.Border.Color1 = System.Drawing.Color.White;
            this.cTextBoxKrypton1.StateCommon.Border.Color2 = System.Drawing.Color.White;
            this.cTextBoxKrypton1.StateCommon.Border.Rounding = 10F;
            this.cTextBoxKrypton1.StateCommon.Content.Color1 = System.Drawing.Color.Black;
            this.cTextBoxKrypton1.StateDisabled.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.cTextBoxKrypton1.StateDisabled.Border.Color1 = System.Drawing.Color.White;
            this.cTextBoxKrypton1.StateDisabled.Border.Color2 = System.Drawing.Color.White;
            this.cTextBoxKrypton1.StateDisabled.Border.Rounding = 10F;
            this.cTextBoxKrypton1.StateDisabled.Content.Color1 = System.Drawing.Color.Black;
            this.cTextBoxKrypton1.StateNormal.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.cTextBoxKrypton1.StateNormal.Border.Color1 = System.Drawing.Color.White;
            this.cTextBoxKrypton1.StateNormal.Border.Color2 = System.Drawing.Color.White;
            this.cTextBoxKrypton1.StateNormal.Border.Rounding = 10F;
            this.cTextBoxKrypton1.StateNormal.Content.Color1 = System.Drawing.Color.Black;
            this.cTextBoxKrypton1.TabIndex = 12;
            this.cTextBoxKrypton1.Text = "cTextBoxKrypton1";
            // 
            // kryptonCheckBox1
            // 
            this.kryptonCheckBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.companyModelBindingSource, "CompanyEnabled", true));
            this.kryptonCheckBox1.Location = new System.Drawing.Point(45, 370);
            this.kryptonCheckBox1.Name = "kryptonCheckBox1";
            this.kryptonCheckBox1.Size = new System.Drawing.Size(90, 25);
            this.kryptonCheckBox1.StateCommon.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.kryptonCheckBox1.TabIndex = 13;
            this.kryptonCheckBox1.Values.Text = "Enabled";
            // 
            // ManageSystemCompany
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 571);
            this.CurrentFormMode = FerPROJ.Design.Class.CBaseEnums.FormMode.Add;
            this.FormTitleAlign = Krypton.Toolkit.PaletteRelativeAlign.Near;
            this.GroupBackStyle = Krypton.Toolkit.PaletteBackStyle.FormMain;
            this.GroupBorderStyle = Krypton.Toolkit.PaletteBorderStyle.FormMain;
            this.HeaderStyle = Krypton.Toolkit.HeaderStyle.Form;
            this.ImageStyle = Krypton.Toolkit.PaletteImageStyle.Inherit;
            this.Name = "ManageSystemCompany";
            this.StateCommon.Back.Color1 = System.Drawing.Color.RoyalBlue;
            this.StateCommon.Back.Color2 = System.Drawing.Color.RoyalBlue;
            this.StateCommon.Border.Color1 = System.Drawing.Color.RoyalBlue;
            this.StateCommon.Border.Color2 = System.Drawing.Color.RoyalBlue;
            this.StateCommon.Header.Back.Color2 = System.Drawing.Color.RoyalBlue;
            this.StateCommon.Header.Border.Color1 = System.Drawing.Color.RoyalBlue;
            this.StateCommon.Header.Border.Color2 = System.Drawing.Color.RoyalBlue;
            this.Text = "FrmCompany";
            this.TitleStyle = Krypton.Toolkit.KryptonFormTitleStyle.Inherit;
            this.basePnl2.ResumeLayout(false);
            this.PanelMain3.ResumeLayout(false);
            this.PanelMain3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.companyModelBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.CTextBoxKrypton companyAddressCTextBoxKrypton;
        private System.Windows.Forms.BindingSource companyModelBindingSource;
        private Controls.CTextBoxKrypton companyContactNoCTextBoxKrypton;
        private Controls.CTextBoxKrypton companyEmailCTextBoxKrypton;
        private Controls.CTextBoxKrypton nameCTextBoxKrypton;
        private Controls.CTextBoxKrypton companyLogoCTextBoxKrypton;
        private System.Windows.Forms.LinkLabel selectLogoLinkLabel;
        private Controls.CTextBoxKrypton cTextBoxKrypton1;
        private Krypton.Toolkit.KryptonCheckBox kryptonCheckBox1;
    }
}
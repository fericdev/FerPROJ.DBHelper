namespace FerPROJ.DBHelper.Forms.Manage {
    partial class ManageUser {
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
            System.Windows.Forms.Label userNameLabel;
            System.Windows.Forms.Label passwordLabel;
            System.Windows.Forms.Label nameLabel;
            System.Windows.Forms.Label userRoleLabel;
            this.userNameCTextBoxKrypton = new FerPROJ.Design.Controls.CTextBoxKrypton();
            this.passwordCTextBoxKrypton = new FerPROJ.Design.Controls.CTextBoxKrypton();
            this.nameCTextBoxKrypton = new FerPROJ.Design.Controls.CTextBoxKrypton();
            this.userRoleCComboBoxKrypton = new FerPROJ.Design.Controls.CComboBoxKrypton();
            this.userModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            userNameLabel = new System.Windows.Forms.Label();
            passwordLabel = new System.Windows.Forms.Label();
            nameLabel = new System.Windows.Forms.Label();
            userRoleLabel = new System.Windows.Forms.Label();
            this.basePnl2.SuspendLayout();
            this.PanelMain3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.userRoleCComboBoxKrypton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.userModelBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // basePnl2
            // 
            this.basePnl2.Location = new System.Drawing.Point(282, 1);
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
            this.PanelMain3.Controls.Add(userRoleLabel);
            this.PanelMain3.Controls.Add(this.userRoleCComboBoxKrypton);
            this.PanelMain3.Controls.Add(nameLabel);
            this.PanelMain3.Controls.Add(this.nameCTextBoxKrypton);
            this.PanelMain3.Controls.Add(passwordLabel);
            this.PanelMain3.Controls.Add(this.passwordCTextBoxKrypton);
            this.PanelMain3.Controls.Add(userNameLabel);
            this.PanelMain3.Controls.Add(this.userNameCTextBoxKrypton);
            this.PanelMain3.Size = new System.Drawing.Size(517, 243);
            // 
            // baseButtonAddNew
            // 
            this.baseButtonAddNew.FlatAppearance.BorderSize = 0;
            // 
            // panelMain1
            // 
            this.panelMain1.Size = new System.Drawing.Size(517, 73);
            // 
            // userNameLabel
            // 
            userNameLabel.AutoSize = true;
            userNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            userNameLabel.Location = new System.Drawing.Point(249, 27);
            userNameLabel.Name = "userNameLabel";
            userNameLabel.Size = new System.Drawing.Size(93, 20);
            userNameLabel.TabIndex = 2;
            userNameLabel.Text = "User Name:";
            // 
            // passwordLabel
            // 
            passwordLabel.AutoSize = true;
            passwordLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            passwordLabel.Location = new System.Drawing.Point(26, 125);
            passwordLabel.Name = "passwordLabel";
            passwordLabel.Size = new System.Drawing.Size(82, 20);
            passwordLabel.TabIndex = 4;
            passwordLabel.Text = "Password:";
            // 
            // nameLabel
            // 
            nameLabel.AutoSize = true;
            nameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            nameLabel.Location = new System.Drawing.Point(26, 27);
            nameLabel.Name = "nameLabel";
            nameLabel.Size = new System.Drawing.Size(55, 20);
            nameLabel.TabIndex = 0;
            nameLabel.Text = "Name:";
            // 
            // userRoleLabel
            // 
            userRoleLabel.AutoSize = true;
            userRoleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            userRoleLabel.Location = new System.Drawing.Point(249, 125);
            userRoleLabel.Name = "userRoleLabel";
            userRoleLabel.Size = new System.Drawing.Size(84, 20);
            userRoleLabel.TabIndex = 6;
            userRoleLabel.Text = "User Role:";
            // 
            // userNameCTextBoxKrypton
            // 
            this.userNameCTextBoxKrypton.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.userModelBindingSource, "UserName", true));
            this.userNameCTextBoxKrypton.Location = new System.Drawing.Point(253, 60);
            this.userNameCTextBoxKrypton.Name = "userNameCTextBoxKrypton";
            this.userNameCTextBoxKrypton.Size = new System.Drawing.Size(220, 29);
            this.userNameCTextBoxKrypton.StateActive.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.userNameCTextBoxKrypton.StateActive.Border.Color1 = System.Drawing.Color.DarkGray;
            this.userNameCTextBoxKrypton.StateActive.Border.Color2 = System.Drawing.Color.White;
            this.userNameCTextBoxKrypton.StateActive.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.userNameCTextBoxKrypton.StateActive.Border.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.Inherit;
            this.userNameCTextBoxKrypton.StateActive.Border.Rounding = 10;
            this.userNameCTextBoxKrypton.StateActive.Content.Color1 = System.Drawing.Color.Black;
            this.userNameCTextBoxKrypton.StateCommon.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.userNameCTextBoxKrypton.StateCommon.Border.Color1 = System.Drawing.Color.White;
            this.userNameCTextBoxKrypton.StateCommon.Border.Color2 = System.Drawing.Color.White;
            this.userNameCTextBoxKrypton.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.userNameCTextBoxKrypton.StateCommon.Border.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.Inherit;
            this.userNameCTextBoxKrypton.StateCommon.Border.Rounding = 10;
            this.userNameCTextBoxKrypton.StateCommon.Content.Color1 = System.Drawing.Color.Black;
            this.userNameCTextBoxKrypton.StateDisabled.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.userNameCTextBoxKrypton.StateDisabled.Border.Color1 = System.Drawing.Color.White;
            this.userNameCTextBoxKrypton.StateDisabled.Border.Color2 = System.Drawing.Color.White;
            this.userNameCTextBoxKrypton.StateDisabled.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.userNameCTextBoxKrypton.StateDisabled.Border.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.Inherit;
            this.userNameCTextBoxKrypton.StateDisabled.Border.Rounding = 10;
            this.userNameCTextBoxKrypton.StateDisabled.Content.Color1 = System.Drawing.Color.Black;
            this.userNameCTextBoxKrypton.StateNormal.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.userNameCTextBoxKrypton.StateNormal.Border.Color1 = System.Drawing.Color.White;
            this.userNameCTextBoxKrypton.StateNormal.Border.Color2 = System.Drawing.Color.White;
            this.userNameCTextBoxKrypton.StateNormal.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.userNameCTextBoxKrypton.StateNormal.Border.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.Inherit;
            this.userNameCTextBoxKrypton.StateNormal.Border.Rounding = 10;
            this.userNameCTextBoxKrypton.StateNormal.Content.Color1 = System.Drawing.Color.Black;
            this.userNameCTextBoxKrypton.TabIndex = 3;
            this.userNameCTextBoxKrypton.Text = "cTextBoxKrypton1";
            // 
            // passwordCTextBoxKrypton
            // 
            this.passwordCTextBoxKrypton.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.userModelBindingSource, "Password", true));
            this.passwordCTextBoxKrypton.Location = new System.Drawing.Point(30, 161);
            this.passwordCTextBoxKrypton.Name = "passwordCTextBoxKrypton";
            this.passwordCTextBoxKrypton.PasswordChar = '●';
            this.passwordCTextBoxKrypton.Size = new System.Drawing.Size(207, 29);
            this.passwordCTextBoxKrypton.StateActive.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.passwordCTextBoxKrypton.StateActive.Border.Color1 = System.Drawing.Color.DarkGray;
            this.passwordCTextBoxKrypton.StateActive.Border.Color2 = System.Drawing.Color.White;
            this.passwordCTextBoxKrypton.StateActive.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.passwordCTextBoxKrypton.StateActive.Border.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.Inherit;
            this.passwordCTextBoxKrypton.StateActive.Border.Rounding = 10;
            this.passwordCTextBoxKrypton.StateActive.Content.Color1 = System.Drawing.Color.Black;
            this.passwordCTextBoxKrypton.StateCommon.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.passwordCTextBoxKrypton.StateCommon.Border.Color1 = System.Drawing.Color.White;
            this.passwordCTextBoxKrypton.StateCommon.Border.Color2 = System.Drawing.Color.White;
            this.passwordCTextBoxKrypton.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.passwordCTextBoxKrypton.StateCommon.Border.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.Inherit;
            this.passwordCTextBoxKrypton.StateCommon.Border.Rounding = 10;
            this.passwordCTextBoxKrypton.StateCommon.Content.Color1 = System.Drawing.Color.Black;
            this.passwordCTextBoxKrypton.StateDisabled.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.passwordCTextBoxKrypton.StateDisabled.Border.Color1 = System.Drawing.Color.White;
            this.passwordCTextBoxKrypton.StateDisabled.Border.Color2 = System.Drawing.Color.White;
            this.passwordCTextBoxKrypton.StateDisabled.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.passwordCTextBoxKrypton.StateDisabled.Border.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.Inherit;
            this.passwordCTextBoxKrypton.StateDisabled.Border.Rounding = 10;
            this.passwordCTextBoxKrypton.StateDisabled.Content.Color1 = System.Drawing.Color.Black;
            this.passwordCTextBoxKrypton.StateNormal.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.passwordCTextBoxKrypton.StateNormal.Border.Color1 = System.Drawing.Color.White;
            this.passwordCTextBoxKrypton.StateNormal.Border.Color2 = System.Drawing.Color.White;
            this.passwordCTextBoxKrypton.StateNormal.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.passwordCTextBoxKrypton.StateNormal.Border.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.Inherit;
            this.passwordCTextBoxKrypton.StateNormal.Border.Rounding = 10;
            this.passwordCTextBoxKrypton.StateNormal.Content.Color1 = System.Drawing.Color.Black;
            this.passwordCTextBoxKrypton.TabIndex = 5;
            this.passwordCTextBoxKrypton.Text = "cTextBoxKrypton1";
            this.passwordCTextBoxKrypton.UseSystemPasswordChar = true;
            // 
            // nameCTextBoxKrypton
            // 
            this.nameCTextBoxKrypton.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.userModelBindingSource, "Name", true));
            this.nameCTextBoxKrypton.Location = new System.Drawing.Point(30, 60);
            this.nameCTextBoxKrypton.Name = "nameCTextBoxKrypton";
            this.nameCTextBoxKrypton.Size = new System.Drawing.Size(207, 29);
            this.nameCTextBoxKrypton.StateActive.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.nameCTextBoxKrypton.StateActive.Border.Color1 = System.Drawing.Color.DarkGray;
            this.nameCTextBoxKrypton.StateActive.Border.Color2 = System.Drawing.Color.White;
            this.nameCTextBoxKrypton.StateActive.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.nameCTextBoxKrypton.StateActive.Border.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.Inherit;
            this.nameCTextBoxKrypton.StateActive.Border.Rounding = 10;
            this.nameCTextBoxKrypton.StateActive.Content.Color1 = System.Drawing.Color.Black;
            this.nameCTextBoxKrypton.StateCommon.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.nameCTextBoxKrypton.StateCommon.Border.Color1 = System.Drawing.Color.White;
            this.nameCTextBoxKrypton.StateCommon.Border.Color2 = System.Drawing.Color.White;
            this.nameCTextBoxKrypton.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.nameCTextBoxKrypton.StateCommon.Border.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.Inherit;
            this.nameCTextBoxKrypton.StateCommon.Border.Rounding = 10;
            this.nameCTextBoxKrypton.StateCommon.Content.Color1 = System.Drawing.Color.Black;
            this.nameCTextBoxKrypton.StateDisabled.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.nameCTextBoxKrypton.StateDisabled.Border.Color1 = System.Drawing.Color.White;
            this.nameCTextBoxKrypton.StateDisabled.Border.Color2 = System.Drawing.Color.White;
            this.nameCTextBoxKrypton.StateDisabled.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.nameCTextBoxKrypton.StateDisabled.Border.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.Inherit;
            this.nameCTextBoxKrypton.StateDisabled.Border.Rounding = 10;
            this.nameCTextBoxKrypton.StateDisabled.Content.Color1 = System.Drawing.Color.Black;
            this.nameCTextBoxKrypton.StateNormal.Back.Color1 = System.Drawing.Color.WhiteSmoke;
            this.nameCTextBoxKrypton.StateNormal.Border.Color1 = System.Drawing.Color.White;
            this.nameCTextBoxKrypton.StateNormal.Border.Color2 = System.Drawing.Color.White;
            this.nameCTextBoxKrypton.StateNormal.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.nameCTextBoxKrypton.StateNormal.Border.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.Inherit;
            this.nameCTextBoxKrypton.StateNormal.Border.Rounding = 10;
            this.nameCTextBoxKrypton.StateNormal.Content.Color1 = System.Drawing.Color.Black;
            this.nameCTextBoxKrypton.TabIndex = 1;
            this.nameCTextBoxKrypton.Text = "cTextBoxKrypton1";
            // 
            // userRoleCComboBoxKrypton
            // 
            this.userRoleCComboBoxKrypton.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.userRoleCComboBoxKrypton.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.userRoleCComboBoxKrypton.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.userModelBindingSource, "UserRole", true));
            this.userRoleCComboBoxKrypton.DropDownWidth = 214;
            this.userRoleCComboBoxKrypton.Location = new System.Drawing.Point(253, 163);
            this.userRoleCComboBoxKrypton.Name = "userRoleCComboBoxKrypton";
            this.userRoleCComboBoxKrypton.Size = new System.Drawing.Size(220, 27);
            this.userRoleCComboBoxKrypton.StateActive.ComboBox.Border.Color1 = System.Drawing.Color.DarkGray;
            this.userRoleCComboBoxKrypton.StateActive.ComboBox.Border.Color2 = System.Drawing.Color.White;
            this.userRoleCComboBoxKrypton.StateActive.ComboBox.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.userRoleCComboBoxKrypton.StateActive.ComboBox.Border.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.Inherit;
            this.userRoleCComboBoxKrypton.StateActive.ComboBox.Border.Rounding = 10;
            this.userRoleCComboBoxKrypton.StateActive.ComboBox.Content.Color1 = System.Drawing.Color.Black;
            this.userRoleCComboBoxKrypton.StateDisabled.ComboBox.Border.Color1 = System.Drawing.Color.DarkGray;
            this.userRoleCComboBoxKrypton.StateDisabled.ComboBox.Border.Color2 = System.Drawing.Color.White;
            this.userRoleCComboBoxKrypton.StateDisabled.ComboBox.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.userRoleCComboBoxKrypton.StateDisabled.ComboBox.Border.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.Inherit;
            this.userRoleCComboBoxKrypton.StateDisabled.ComboBox.Border.Rounding = 10;
            this.userRoleCComboBoxKrypton.StateDisabled.ComboBox.Content.Color1 = System.Drawing.Color.Black;
            this.userRoleCComboBoxKrypton.TabIndex = 7;
            this.userRoleCComboBoxKrypton.Text = "cComboBoxKrypton1";
            // 
            // userModelBindingSource
            // 
            this.userModelBindingSource.DataSource = typeof(FerPROJ.Design.Models.UserModel);
            // 
            // ManageUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(531, 401);
            this.CurrentFormMode = FerPROJ.Design.Forms.FrmManageKrypton.FormMode.Add;
            this.Name = "ManageUser";
            this.Text = "ManageUser";
            this.basePnl2.ResumeLayout(false);
            this.PanelMain3.ResumeLayout(false);
            this.PanelMain3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.userRoleCComboBoxKrypton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.userModelBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private FerPROJ.Design.Controls.CComboBoxKrypton userRoleCComboBoxKrypton;
        private FerPROJ.Design.Controls.CTextBoxKrypton nameCTextBoxKrypton;
        private FerPROJ.Design.Controls.CTextBoxKrypton passwordCTextBoxKrypton;
        private FerPROJ.Design.Controls.CTextBoxKrypton userNameCTextBoxKrypton;
        private System.Windows.Forms.BindingSource userModelBindingSource;
    }
}
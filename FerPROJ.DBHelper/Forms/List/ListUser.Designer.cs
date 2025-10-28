namespace FerPROJ.DBHelper.Forms.List {
    partial class ListUser {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.userModelCDatagridview = new FerPROJ.Design.Controls.CDatagridview();
            this.userModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.formIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.userNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.passwordDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.userRoleDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UserID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dateCreatedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dateCreatedStringDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dateModifiedStringDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dateModifiedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.createdByDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.createdByIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.modifiedByDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.modifiedByIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PanelMain4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ComboBoxKryptonDataLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.userModelCDatagridview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.userModelBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // baseButtonSelect
            // 
            this.baseButtonSelect.FlatAppearance.BorderSize = 0;
            // 
            // baseButtonCancel
            // 
            this.baseButtonCancel.FlatAppearance.BorderSize = 0;
            // 
            // PanelMain4
            // 
            this.PanelMain4.Controls.Add(this.userModelCDatagridview);
            this.PanelMain4.Size = new System.Drawing.Size(1101, 522);
            this.PanelMain4.Controls.SetChildIndex(this.userModelCDatagridview, 0);
            // 
            // panelMain11
            // 
            this.panelMain11.Size = new System.Drawing.Size(1101, 73);
            // 
            // ComboBoxKryptonDataLimit
            // 
            this.ComboBoxKryptonDataLimit.StateActive.ComboBox.Border.Color1 = System.Drawing.Color.DarkGray;
            this.ComboBoxKryptonDataLimit.StateActive.ComboBox.Border.Color2 = System.Drawing.Color.White;
            this.ComboBoxKryptonDataLimit.StateActive.ComboBox.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.ComboBoxKryptonDataLimit.StateActive.ComboBox.Border.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.Inherit;
            this.ComboBoxKryptonDataLimit.StateActive.ComboBox.Border.Rounding = 10;
            this.ComboBoxKryptonDataLimit.StateActive.ComboBox.Content.Color1 = System.Drawing.Color.Black;
            this.ComboBoxKryptonDataLimit.StateDisabled.ComboBox.Border.Color1 = System.Drawing.Color.DarkGray;
            this.ComboBoxKryptonDataLimit.StateDisabled.ComboBox.Border.Color2 = System.Drawing.Color.White;
            this.ComboBoxKryptonDataLimit.StateDisabled.ComboBox.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.ComboBoxKryptonDataLimit.StateDisabled.ComboBox.Border.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.Inherit;
            this.ComboBoxKryptonDataLimit.StateDisabled.ComboBox.Border.Rounding = 10;
            this.ComboBoxKryptonDataLimit.StateDisabled.ComboBox.Content.Color1 = System.Drawing.Color.Black;
            // 
            // userModelCDatagridview
            // 
            this.userModelCDatagridview.AllowUserToAddRows = false;
            this.userModelCDatagridview.AllowUserToDeleteRows = false;
            this.userModelCDatagridview.AllowUserToOrderColumns = true;
            this.userModelCDatagridview.AllowUserToResizeRows = false;
            this.userModelCDatagridview.AlternatingRowColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightGray;
            this.userModelCDatagridview.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.userModelCDatagridview.AutoGenerateColumns = false;
            this.userModelCDatagridview.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.userModelCDatagridview.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.userModelCDatagridview.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.userModelCDatagridview.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.userModelCDatagridview.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Custom;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.userModelCDatagridview.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.userModelCDatagridview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.userModelCDatagridview.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.formIdDataGridViewTextBoxColumn,
            this.userNameDataGridViewTextBoxColumn,
            this.passwordDataGridViewTextBoxColumn,
            this.userRoleDataGridViewTextBoxColumn,
            this.UserID,
            this.nameDataGridViewTextBoxColumn,
            this.dateCreatedDataGridViewTextBoxColumn,
            this.dateCreatedStringDataGridViewTextBoxColumn,
            this.dateModifiedStringDataGridViewTextBoxColumn,
            this.dateModifiedDataGridViewTextBoxColumn,
            this.createdByDataGridViewTextBoxColumn,
            this.createdByIdDataGridViewTextBoxColumn,
            this.modifiedByDataGridViewTextBoxColumn,
            this.modifiedByIdDataGridViewTextBoxColumn,
            this.statusDataGridViewTextBoxColumn});
            this.userModelCDatagridview.CustomHeaderFontStyle = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.userModelCDatagridview.CustomHeaderForeColor = System.Drawing.Color.Black;
            this.userModelCDatagridview.CustomRowFontStyle = new System.Drawing.Font("Tahoma", 8F);
            this.userModelCDatagridview.DataSource = this.userModelBindingSource;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Tahoma", 8F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.userModelCDatagridview.DefaultCellStyle = dataGridViewCellStyle3;
            this.userModelCDatagridview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userModelCDatagridview.EnableHeadersVisualStyles = false;
            this.userModelCDatagridview.HeaderColor = System.Drawing.Color.WhiteSmoke;
            this.userModelCDatagridview.Location = new System.Drawing.Point(0, 41);
            this.userModelCDatagridview.Name = "userModelCDatagridview";
            this.userModelCDatagridview.ReadOnly = true;
            this.userModelCDatagridview.RowHeadersVisible = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Tahoma", 8F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.userModelCDatagridview.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.userModelCDatagridview.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.userModelCDatagridview.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.userModelCDatagridview.Size = new System.Drawing.Size(1101, 410);
            this.userModelCDatagridview.TabIndex = 4;
            // 
            // userModelBindingSource
            // 
            this.userModelBindingSource.DataSource = typeof(FerPROJ.Design.Models.UserModel);
            // 
            // formIdDataGridViewTextBoxColumn
            // 
            this.formIdDataGridViewTextBoxColumn.DataPropertyName = "FormId";
            this.formIdDataGridViewTextBoxColumn.HeaderText = "FormId";
            this.formIdDataGridViewTextBoxColumn.Name = "formIdDataGridViewTextBoxColumn";
            this.formIdDataGridViewTextBoxColumn.ReadOnly = true;
            this.formIdDataGridViewTextBoxColumn.Visible = false;
            // 
            // userNameDataGridViewTextBoxColumn
            // 
            this.userNameDataGridViewTextBoxColumn.DataPropertyName = "UserName";
            this.userNameDataGridViewTextBoxColumn.HeaderText = "UserName";
            this.userNameDataGridViewTextBoxColumn.Name = "userNameDataGridViewTextBoxColumn";
            this.userNameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // passwordDataGridViewTextBoxColumn
            // 
            this.passwordDataGridViewTextBoxColumn.DataPropertyName = "Password";
            this.passwordDataGridViewTextBoxColumn.HeaderText = "Password";
            this.passwordDataGridViewTextBoxColumn.Name = "passwordDataGridViewTextBoxColumn";
            this.passwordDataGridViewTextBoxColumn.ReadOnly = true;
            this.passwordDataGridViewTextBoxColumn.Visible = false;
            // 
            // userRoleDataGridViewTextBoxColumn
            // 
            this.userRoleDataGridViewTextBoxColumn.DataPropertyName = "UserRole";
            this.userRoleDataGridViewTextBoxColumn.HeaderText = "UserRole";
            this.userRoleDataGridViewTextBoxColumn.Name = "userRoleDataGridViewTextBoxColumn";
            this.userRoleDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // UserID
            // 
            this.UserID.DataPropertyName = "Id";
            this.UserID.HeaderText = "Id";
            this.UserID.Name = "UserID";
            this.UserID.ReadOnly = true;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // dateCreatedDataGridViewTextBoxColumn
            // 
            this.dateCreatedDataGridViewTextBoxColumn.DataPropertyName = "DateCreated";
            this.dateCreatedDataGridViewTextBoxColumn.HeaderText = "DateCreated";
            this.dateCreatedDataGridViewTextBoxColumn.Name = "dateCreatedDataGridViewTextBoxColumn";
            this.dateCreatedDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // dateCreatedStringDataGridViewTextBoxColumn
            // 
            this.dateCreatedStringDataGridViewTextBoxColumn.DataPropertyName = "DateCreatedString";
            this.dateCreatedStringDataGridViewTextBoxColumn.HeaderText = "DateCreatedString";
            this.dateCreatedStringDataGridViewTextBoxColumn.Name = "dateCreatedStringDataGridViewTextBoxColumn";
            this.dateCreatedStringDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // dateModifiedStringDataGridViewTextBoxColumn
            // 
            this.dateModifiedStringDataGridViewTextBoxColumn.DataPropertyName = "DateModifiedString";
            this.dateModifiedStringDataGridViewTextBoxColumn.HeaderText = "DateModifiedString";
            this.dateModifiedStringDataGridViewTextBoxColumn.Name = "dateModifiedStringDataGridViewTextBoxColumn";
            this.dateModifiedStringDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // dateModifiedDataGridViewTextBoxColumn
            // 
            this.dateModifiedDataGridViewTextBoxColumn.DataPropertyName = "DateModified";
            this.dateModifiedDataGridViewTextBoxColumn.HeaderText = "DateModified";
            this.dateModifiedDataGridViewTextBoxColumn.Name = "dateModifiedDataGridViewTextBoxColumn";
            this.dateModifiedDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // createdByDataGridViewTextBoxColumn
            // 
            this.createdByDataGridViewTextBoxColumn.DataPropertyName = "CreatedBy";
            this.createdByDataGridViewTextBoxColumn.HeaderText = "CreatedBy";
            this.createdByDataGridViewTextBoxColumn.Name = "createdByDataGridViewTextBoxColumn";
            this.createdByDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // createdByIdDataGridViewTextBoxColumn
            // 
            this.createdByIdDataGridViewTextBoxColumn.DataPropertyName = "CreatedById";
            this.createdByIdDataGridViewTextBoxColumn.HeaderText = "CreatedById";
            this.createdByIdDataGridViewTextBoxColumn.Name = "createdByIdDataGridViewTextBoxColumn";
            this.createdByIdDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // modifiedByDataGridViewTextBoxColumn
            // 
            this.modifiedByDataGridViewTextBoxColumn.DataPropertyName = "ModifiedBy";
            this.modifiedByDataGridViewTextBoxColumn.HeaderText = "ModifiedBy";
            this.modifiedByDataGridViewTextBoxColumn.Name = "modifiedByDataGridViewTextBoxColumn";
            this.modifiedByDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // modifiedByIdDataGridViewTextBoxColumn
            // 
            this.modifiedByIdDataGridViewTextBoxColumn.DataPropertyName = "ModifiedById";
            this.modifiedByIdDataGridViewTextBoxColumn.HeaderText = "ModifiedById";
            this.modifiedByIdDataGridViewTextBoxColumn.Name = "modifiedByIdDataGridViewTextBoxColumn";
            this.modifiedByIdDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // statusDataGridViewTextBoxColumn
            // 
            this.statusDataGridViewTextBoxColumn.DataPropertyName = "Status";
            this.statusDataGridViewTextBoxColumn.HeaderText = "Status";
            this.statusDataGridViewTextBoxColumn.Name = "statusDataGridViewTextBoxColumn";
            this.statusDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // ListUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1115, 609);
            this.CurrentManageMode = true;
            this.Name = "ListUser";
            this.Text = "ListUser";
            this.PanelMain4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ComboBoxKryptonDataLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.userModelCDatagridview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.userModelBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private FerPROJ.Design.Controls.CDatagridview userModelCDatagridview;
        private System.Windows.Forms.BindingSource userModelBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn formIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn userNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn passwordDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn userRoleDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn UserID;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dateCreatedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dateCreatedStringDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dateModifiedStringDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dateModifiedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn createdByDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn createdByIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn modifiedByDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn modifiedByIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn statusDataGridViewTextBoxColumn;
    }
}

using FerPROJ.Design.Controls;
using FerPROJ.Design.Forms;


namespace FerPROJ.DBHelper.Forms
{
    partial class FrmDatabaseConfig
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.customPanel1 = new FerPROJ.Design.Forms.CPanel();
            this.cbSSL = new System.Windows.Forms.CheckBox();
            this.saveConfigCustomButton = new FerPROJ.Design.Controls.CButton();
            this.passwordCustomTextBox = new FerPROJ.Design.Controls.CTextBox();
            this.usernameCustomTextBox = new FerPROJ.Design.Controls.CTextBox();
            this.portCustomTextBox = new FerPROJ.Design.Controls.CTextBox();
            this.databaseNameCustomTextBox = new FerPROJ.Design.Controls.CTextBox();
            this.hostnameCustomTextBox = new FerPROJ.Design.Controls.CTextBox();
            this.customLabelDesc5 = new FerPROJ.Design.Controls.CLabelDesc();
            this.customLabelDesc4 = new FerPROJ.Design.Controls.CLabelDesc();
            this.customLabelDesc3 = new FerPROJ.Design.Controls.CLabelDesc();
            this.customLabelDesc2 = new FerPROJ.Design.Controls.CLabelDesc();
            this.customLabelDesc1 = new FerPROJ.Design.Controls.CLabelDesc();
            this.cButtonRunMigration = new FerPROJ.Design.Controls.CButton();
            this.customPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // customPanel1
            // 
            this.customPanel1.BorderColor = System.Drawing.Color.MediumSlateBlue;
            this.customPanel1.BorderFocusColor = System.Drawing.Color.HotPink;
            this.customPanel1.BorderRadius = 0;
            this.customPanel1.BorderSize = 2;
            this.customPanel1.Controls.Add(this.cButtonRunMigration);
            this.customPanel1.Controls.Add(this.cbSSL);
            this.customPanel1.Controls.Add(this.saveConfigCustomButton);
            this.customPanel1.Controls.Add(this.passwordCustomTextBox);
            this.customPanel1.Controls.Add(this.usernameCustomTextBox);
            this.customPanel1.Controls.Add(this.portCustomTextBox);
            this.customPanel1.Controls.Add(this.databaseNameCustomTextBox);
            this.customPanel1.Controls.Add(this.hostnameCustomTextBox);
            this.customPanel1.Controls.Add(this.customLabelDesc5);
            this.customPanel1.Controls.Add(this.customLabelDesc4);
            this.customPanel1.Controls.Add(this.customLabelDesc3);
            this.customPanel1.Controls.Add(this.customLabelDesc2);
            this.customPanel1.Controls.Add(this.customLabelDesc1);
            this.customPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customPanel1.Location = new System.Drawing.Point(5, 5);
            this.customPanel1.Name = "customPanel1";
            this.customPanel1.Size = new System.Drawing.Size(377, 448);
            this.customPanel1.TabIndex = 0;
            this.customPanel1.UnderlinedStyle = false;
            // 
            // cbSSL
            // 
            this.cbSSL.AutoSize = true;
            this.cbSSL.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbSSL.Location = new System.Drawing.Point(41, 348);
            this.cbSSL.Name = "cbSSL";
            this.cbSSL.Size = new System.Drawing.Size(109, 20);
            this.cbSSL.TabIndex = 6;
            this.cbSSL.Text = "SSL Disabled";
            this.cbSSL.UseVisualStyleBackColor = true;
            // 
            // saveConfigCustomButton
            // 
            this.saveConfigCustomButton.BackColor = System.Drawing.Color.CornflowerBlue;
            this.saveConfigCustomButton.BackgroundColor = System.Drawing.Color.CornflowerBlue;
            this.saveConfigCustomButton.BorderColor = System.Drawing.Color.Green;
            this.saveConfigCustomButton.BorderRadius = 20;
            this.saveConfigCustomButton.BorderSize = 0;
            this.saveConfigCustomButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.saveConfigCustomButton.FlatAppearance.BorderSize = 0;
            this.saveConfigCustomButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveConfigCustomButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.saveConfigCustomButton.ForeColor = System.Drawing.Color.White;
            this.saveConfigCustomButton.Location = new System.Drawing.Point(41, 383);
            this.saveConfigCustomButton.Name = "saveConfigCustomButton";
            this.saveConfigCustomButton.Size = new System.Drawing.Size(147, 40);
            this.saveConfigCustomButton.TabIndex = 5;
            this.saveConfigCustomButton.Text = "Save Configuration";
            this.saveConfigCustomButton.TextColor = System.Drawing.Color.White;
            this.saveConfigCustomButton.UseVisualStyleBackColor = false;
            this.saveConfigCustomButton.Click += new System.EventHandler(this.saveConfigCustomButton_Click);
            // 
            // passwordCustomTextBox
            // 
            this.passwordCustomTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.passwordCustomTextBox.BorderColor = System.Drawing.Color.DarkGray;
            this.passwordCustomTextBox.BorderFocusColor = System.Drawing.Color.HotPink;
            this.passwordCustomTextBox.BorderRadius = 15;
            this.passwordCustomTextBox.BorderSize = 2;
            this.passwordCustomTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passwordCustomTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.passwordCustomTextBox.Location = new System.Drawing.Point(41, 308);
            this.passwordCustomTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.passwordCustomTextBox.Multiline = false;
            this.passwordCustomTextBox.Name = "passwordCustomTextBox";
            this.passwordCustomTextBox.OnFocus = false;
            this.passwordCustomTextBox.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.passwordCustomTextBox.PasswordChar = true;
            this.passwordCustomTextBox.PlaceholderColor = System.Drawing.Color.DarkGray;
            this.passwordCustomTextBox.PlaceholderText = "";
            this.passwordCustomTextBox.Size = new System.Drawing.Size(310, 31);
            this.passwordCustomTextBox.TabIndex = 4;
            this.passwordCustomTextBox.TextProperty = "";
            this.passwordCustomTextBox.Texts = "";
            this.passwordCustomTextBox.UnderlinedStyle = false;
            // 
            // usernameCustomTextBox
            // 
            this.usernameCustomTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.usernameCustomTextBox.BorderColor = System.Drawing.Color.DarkGray;
            this.usernameCustomTextBox.BorderFocusColor = System.Drawing.Color.HotPink;
            this.usernameCustomTextBox.BorderRadius = 15;
            this.usernameCustomTextBox.BorderSize = 2;
            this.usernameCustomTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usernameCustomTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.usernameCustomTextBox.Location = new System.Drawing.Point(41, 237);
            this.usernameCustomTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.usernameCustomTextBox.Multiline = false;
            this.usernameCustomTextBox.Name = "usernameCustomTextBox";
            this.usernameCustomTextBox.OnFocus = false;
            this.usernameCustomTextBox.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.usernameCustomTextBox.PasswordChar = true;
            this.usernameCustomTextBox.PlaceholderColor = System.Drawing.Color.DarkGray;
            this.usernameCustomTextBox.PlaceholderText = "";
            this.usernameCustomTextBox.Size = new System.Drawing.Size(310, 31);
            this.usernameCustomTextBox.TabIndex = 3;
            this.usernameCustomTextBox.TextProperty = null;
            this.usernameCustomTextBox.Texts = "";
            this.usernameCustomTextBox.UnderlinedStyle = false;
            // 
            // portCustomTextBox
            // 
            this.portCustomTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.portCustomTextBox.BorderColor = System.Drawing.Color.DarkGray;
            this.portCustomTextBox.BorderFocusColor = System.Drawing.Color.HotPink;
            this.portCustomTextBox.BorderRadius = 15;
            this.portCustomTextBox.BorderSize = 2;
            this.portCustomTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.portCustomTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.portCustomTextBox.Location = new System.Drawing.Point(41, 171);
            this.portCustomTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.portCustomTextBox.Multiline = false;
            this.portCustomTextBox.Name = "portCustomTextBox";
            this.portCustomTextBox.OnFocus = false;
            this.portCustomTextBox.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.portCustomTextBox.PasswordChar = false;
            this.portCustomTextBox.PlaceholderColor = System.Drawing.Color.DarkGray;
            this.portCustomTextBox.PlaceholderText = "";
            this.portCustomTextBox.Size = new System.Drawing.Size(310, 31);
            this.portCustomTextBox.TabIndex = 2;
            this.portCustomTextBox.TextProperty = null;
            this.portCustomTextBox.Texts = "";
            this.portCustomTextBox.UnderlinedStyle = false;
            // 
            // databaseNameCustomTextBox
            // 
            this.databaseNameCustomTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.databaseNameCustomTextBox.BorderColor = System.Drawing.Color.DarkGray;
            this.databaseNameCustomTextBox.BorderFocusColor = System.Drawing.Color.HotPink;
            this.databaseNameCustomTextBox.BorderRadius = 15;
            this.databaseNameCustomTextBox.BorderSize = 2;
            this.databaseNameCustomTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.databaseNameCustomTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.databaseNameCustomTextBox.Location = new System.Drawing.Point(41, 112);
            this.databaseNameCustomTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.databaseNameCustomTextBox.Multiline = false;
            this.databaseNameCustomTextBox.Name = "databaseNameCustomTextBox";
            this.databaseNameCustomTextBox.OnFocus = false;
            this.databaseNameCustomTextBox.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.databaseNameCustomTextBox.PasswordChar = true;
            this.databaseNameCustomTextBox.PlaceholderColor = System.Drawing.Color.DarkGray;
            this.databaseNameCustomTextBox.PlaceholderText = "";
            this.databaseNameCustomTextBox.Size = new System.Drawing.Size(310, 31);
            this.databaseNameCustomTextBox.TabIndex = 1;
            this.databaseNameCustomTextBox.TextProperty = null;
            this.databaseNameCustomTextBox.Texts = "";
            this.databaseNameCustomTextBox.UnderlinedStyle = false;
            // 
            // hostnameCustomTextBox
            // 
            this.hostnameCustomTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.hostnameCustomTextBox.BorderColor = System.Drawing.Color.DarkGray;
            this.hostnameCustomTextBox.BorderFocusColor = System.Drawing.Color.HotPink;
            this.hostnameCustomTextBox.BorderRadius = 15;
            this.hostnameCustomTextBox.BorderSize = 2;
            this.hostnameCustomTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hostnameCustomTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.hostnameCustomTextBox.Location = new System.Drawing.Point(41, 48);
            this.hostnameCustomTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.hostnameCustomTextBox.Multiline = false;
            this.hostnameCustomTextBox.Name = "hostnameCustomTextBox";
            this.hostnameCustomTextBox.OnFocus = false;
            this.hostnameCustomTextBox.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.hostnameCustomTextBox.PasswordChar = false;
            this.hostnameCustomTextBox.PlaceholderColor = System.Drawing.Color.DarkGray;
            this.hostnameCustomTextBox.PlaceholderText = "";
            this.hostnameCustomTextBox.Size = new System.Drawing.Size(310, 31);
            this.hostnameCustomTextBox.TabIndex = 0;
            this.hostnameCustomTextBox.TextProperty = null;
            this.hostnameCustomTextBox.Texts = "";
            this.hostnameCustomTextBox.UnderlinedStyle = false;
            // 
            // customLabelDesc5
            // 
            this.customLabelDesc5.AutoSize = true;
            this.customLabelDesc5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.customLabelDesc5.Location = new System.Drawing.Point(36, 286);
            this.customLabelDesc5.Name = "customLabelDesc5";
            this.customLabelDesc5.Size = new System.Drawing.Size(73, 17);
            this.customLabelDesc5.TabIndex = 4;
            this.customLabelDesc5.Text = "Password:";
            // 
            // customLabelDesc4
            // 
            this.customLabelDesc4.AutoSize = true;
            this.customLabelDesc4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.customLabelDesc4.Location = new System.Drawing.Point(36, 214);
            this.customLabelDesc4.Name = "customLabelDesc4";
            this.customLabelDesc4.Size = new System.Drawing.Size(77, 17);
            this.customLabelDesc4.TabIndex = 3;
            this.customLabelDesc4.Text = "Username:";
            // 
            // customLabelDesc3
            // 
            this.customLabelDesc3.AutoSize = true;
            this.customLabelDesc3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.customLabelDesc3.Location = new System.Drawing.Point(36, 150);
            this.customLabelDesc3.Name = "customLabelDesc3";
            this.customLabelDesc3.Size = new System.Drawing.Size(38, 17);
            this.customLabelDesc3.TabIndex = 2;
            this.customLabelDesc3.Text = "Port:";
            // 
            // customLabelDesc2
            // 
            this.customLabelDesc2.AutoSize = true;
            this.customLabelDesc2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.customLabelDesc2.Location = new System.Drawing.Point(36, 88);
            this.customLabelDesc2.Name = "customLabelDesc2";
            this.customLabelDesc2.Size = new System.Drawing.Size(114, 17);
            this.customLabelDesc2.TabIndex = 1;
            this.customLabelDesc2.Text = "Database Name:";
            // 
            // customLabelDesc1
            // 
            this.customLabelDesc1.AutoSize = true;
            this.customLabelDesc1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.customLabelDesc1.Location = new System.Drawing.Point(36, 25);
            this.customLabelDesc1.Name = "customLabelDesc1";
            this.customLabelDesc1.Size = new System.Drawing.Size(76, 17);
            this.customLabelDesc1.TabIndex = 0;
            this.customLabelDesc1.Text = "Hostname:";
            // 
            // cButtonRunMigration
            // 
            this.cButtonRunMigration.BackColor = System.Drawing.Color.Red;
            this.cButtonRunMigration.BackgroundColor = System.Drawing.Color.Red;
            this.cButtonRunMigration.BorderColor = System.Drawing.Color.Green;
            this.cButtonRunMigration.BorderRadius = 20;
            this.cButtonRunMigration.BorderSize = 0;
            this.cButtonRunMigration.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cButtonRunMigration.FlatAppearance.BorderSize = 0;
            this.cButtonRunMigration.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cButtonRunMigration.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.cButtonRunMigration.ForeColor = System.Drawing.Color.White;
            this.cButtonRunMigration.Location = new System.Drawing.Point(204, 383);
            this.cButtonRunMigration.Name = "cButtonRunMigration";
            this.cButtonRunMigration.Size = new System.Drawing.Size(147, 40);
            this.cButtonRunMigration.TabIndex = 7;
            this.cButtonRunMigration.Text = "Run Migration";
            this.cButtonRunMigration.TextColor = System.Drawing.Color.White;
            this.cButtonRunMigration.UseVisualStyleBackColor = false;
            this.cButtonRunMigration.Click += new System.EventHandler(this.cButtonRunMigration_Click);
            // 
            // FrmConf
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 458);
            this.Controls.Add(this.customPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FrmDatabaseConfig";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Database Connection Settings";
            this.Load += new System.EventHandler(this.FrmConnectionSettings_Load);
            this.customPanel1.ResumeLayout(false);
            this.customPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private CPanel customPanel1;
        private CButton saveConfigCustomButton;
        private CTextBox passwordCustomTextBox;
        private CTextBox usernameCustomTextBox;
        private CTextBox portCustomTextBox;
        private CTextBox databaseNameCustomTextBox;
        private CTextBox hostnameCustomTextBox;
        private CLabelDesc customLabelDesc5;
        private CLabelDesc customLabelDesc4;
        private CLabelDesc customLabelDesc3;
        private CLabelDesc customLabelDesc2;
        private CLabelDesc customLabelDesc1;
        private System.Windows.Forms.CheckBox cbSSL;
        private CButton cButtonRunMigration;
    }
}
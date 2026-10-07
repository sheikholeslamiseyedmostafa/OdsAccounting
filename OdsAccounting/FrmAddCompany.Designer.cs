namespace OdsAccounting
{
    partial class FrmAddCompany
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            lblCompanyName = new Label();
            txtCompanyName = new TextBox();
            lblNationalID = new Label();
            txtNationalID = new TextBox();
            lblEconomicCode = new Label();
            txtEconomicCode = new TextBox();
            lblRegistrationNo = new Label();
            txtRegistrationNo = new TextBox();
            lblPhone = new Label();
            txtPhone = new TextBox();
            lblAddress = new Label();
            txtAddress = new TextBox();
            lblDescription = new Label();
            txtDescription = new TextBox();
            lblColor = new Label();
            cmbColor = new ComboBox();
            chkIsActive = new CheckBox();
            btnSave = new Button();
            SuspendLayout();
            // 
            // lblCompanyName
            // 
            lblCompanyName.AutoSize = true;
            lblCompanyName.Font = new Font("B Nazanin", 12F, FontStyle.Bold);
            lblCompanyName.Location = new Point(68, 65);
            lblCompanyName.Name = "lblCompanyName";
            lblCompanyName.Size = new Size(168, 51);
            lblCompanyName.TabIndex = 17;
            lblCompanyName.Text = "نام شرکت * :";
            // 
            // txtCompanyName
            // 
            txtCompanyName.Font = new Font("B Nazanin", 10.125F);
            txtCompanyName.Location = new Point(242, 69);
            txtCompanyName.Name = "txtCompanyName";
            txtCompanyName.Size = new Size(701, 48);
            txtCompanyName.TabIndex = 16;
            // 
            // lblNationalID
            // 
            lblNationalID.AutoSize = true;
            lblNationalID.Font = new Font("B Nazanin", 12F, FontStyle.Bold);
            lblNationalID.Location = new Point(78, 133);
            lblNationalID.Name = "lblNationalID";
            lblNationalID.Size = new Size(158, 51);
            lblNationalID.TabIndex = 15;
            lblNationalID.Text = "شناسه ملی :";
            // 
            // txtNationalID
            // 
            txtNationalID.Font = new Font("B Nazanin", 10.125F);
            txtNationalID.Location = new Point(242, 137);
            txtNationalID.Name = "txtNationalID";
            txtNationalID.Size = new Size(701, 48);
            txtNationalID.TabIndex = 14;
            // 
            // lblEconomicCode
            // 
            lblEconomicCode.AutoSize = true;
            lblEconomicCode.Font = new Font("B Nazanin", 12F, FontStyle.Bold);
            lblEconomicCode.Location = new Point(34, 210);
            lblEconomicCode.Name = "lblEconomicCode";
            lblEconomicCode.Size = new Size(202, 51);
            lblEconomicCode.TabIndex = 13;
            lblEconomicCode.Text = "شماره اقتصادی :";
            // 
            // txtEconomicCode
            // 
            txtEconomicCode.Font = new Font("B Nazanin", 10.125F);
            txtEconomicCode.Location = new Point(242, 210);
            txtEconomicCode.Name = "txtEconomicCode";
            txtEconomicCode.Size = new Size(701, 48);
            txtEconomicCode.TabIndex = 12;
            // 
            // lblRegistrationNo
            // 
            lblRegistrationNo.AutoSize = true;
            lblRegistrationNo.Font = new Font("B Nazanin", 12F, FontStyle.Bold);
            lblRegistrationNo.Location = new Point(83, 280);
            lblRegistrationNo.Name = "lblRegistrationNo";
            lblRegistrationNo.Size = new Size(153, 51);
            lblRegistrationNo.TabIndex = 11;
            lblRegistrationNo.Text = "شماره ثبت :";
            // 
            // txtRegistrationNo
            // 
            txtRegistrationNo.Font = new Font("B Nazanin", 10.125F);
            txtRegistrationNo.Location = new Point(242, 280);
            txtRegistrationNo.Name = "txtRegistrationNo";
            txtRegistrationNo.Size = new Size(701, 48);
            txtRegistrationNo.TabIndex = 10;
            // 
            // lblPhone
            // 
            lblPhone.AutoSize = true;
            lblPhone.Font = new Font("B Nazanin", 12F, FontStyle.Bold);
            lblPhone.Location = new Point(149, 348);
            lblPhone.Name = "lblPhone";
            lblPhone.Size = new Size(87, 51);
            lblPhone.TabIndex = 9;
            lblPhone.Text = "تلفن :";
            // 
            // txtPhone
            // 
            txtPhone.Font = new Font("B Nazanin", 10.125F);
            txtPhone.Location = new Point(242, 352);
            txtPhone.Name = "txtPhone";
            txtPhone.Size = new Size(701, 48);
            txtPhone.TabIndex = 8;
            // 
            // lblAddress
            // 
            lblAddress.AutoSize = true;
            lblAddress.Font = new Font("B Nazanin", 12F, FontStyle.Bold);
            lblAddress.Location = new Point(134, 422);
            lblAddress.Name = "lblAddress";
            lblAddress.Size = new Size(102, 51);
            lblAddress.TabIndex = 7;
            lblAddress.Text = "آدرس :";
            // 
            // txtAddress
            // 
            txtAddress.Font = new Font("B Nazanin", 10.125F);
            txtAddress.Location = new Point(242, 422);
            txtAddress.Name = "txtAddress";
            txtAddress.Size = new Size(701, 48);
            txtAddress.TabIndex = 6;
            // 
            // lblDescription
            // 
            lblDescription.AutoSize = true;
            lblDescription.Font = new Font("B Nazanin", 12F, FontStyle.Bold);
            lblDescription.Location = new Point(96, 544);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(140, 51);
            lblDescription.TabIndex = 5;
            lblDescription.Text = "توضیحات :";
            // 
            // txtDescription
            // 
            txtDescription.Font = new Font("B Nazanin", 10.125F);
            txtDescription.Location = new Point(242, 503);
            txtDescription.Multiline = true;
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(701, 141);
            txtDescription.TabIndex = 4;
            // 
            // lblColor
            // 
            lblColor.AutoSize = true;
            lblColor.Font = new Font("B Nazanin", 12F, FontStyle.Bold);
            lblColor.Location = new Point(151, 668);
            lblColor.Name = "lblColor";
            lblColor.Size = new Size(85, 51);
            lblColor.TabIndex = 3;
            lblColor.Text = "رنگ :";
            // 
            // cmbColor
            // 
            cmbColor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbColor.Font = new Font("B Nazanin", 10.125F);
            cmbColor.FormattingEnabled = true;
            cmbColor.ItemHeight = 40;
            cmbColor.Items.AddRange(new object[] { "بدون رنگ", "قرمز", "آبی", "سبز", "زرد" });
            cmbColor.Location = new Point(242, 668);
            cmbColor.Name = "cmbColor";
            cmbColor.Size = new Size(701, 48);
            cmbColor.TabIndex = 2;
            // 
            // chkIsActive
            // 
            chkIsActive.AutoSize = true;
            chkIsActive.Checked = true;
            chkIsActive.CheckState = CheckState.Checked;
            chkIsActive.Location = new Point(242, 745);
            chkIsActive.Name = "chkIsActive";
            chkIsActive.Size = new Size(95, 35);
            chkIsActive.TabIndex = 1;
            chkIsActive.Text = "فعال";
            chkIsActive.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            btnSave.Font = new Font("B Nazanin", 12F, FontStyle.Bold, GraphicsUnit.Point, 178);
            btnSave.Location = new Point(462, 777);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(225, 75);
            btnSave.TabIndex = 0;
            btnSave.Text = "ثبت اطلاعات";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // FrmAddCompany
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1029, 960);
            Controls.Add(btnSave);
            Controls.Add(chkIsActive);
            Controls.Add(cmbColor);
            Controls.Add(lblColor);
            Controls.Add(txtDescription);
            Controls.Add(lblDescription);
            Controls.Add(txtAddress);
            Controls.Add(lblAddress);
            Controls.Add(txtPhone);
            Controls.Add(lblPhone);
            Controls.Add(txtRegistrationNo);
            Controls.Add(lblRegistrationNo);
            Controls.Add(txtEconomicCode);
            Controls.Add(lblEconomicCode);
            Controls.Add(txtNationalID);
            Controls.Add(lblNationalID);
            Controls.Add(txtCompanyName);
            Controls.Add(lblCompanyName);
            Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 178);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmAddCompany";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            StartPosition = FormStartPosition.CenterParent;
            Text = "افزودن شرکت جدید";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblCompanyName;
        private System.Windows.Forms.TextBox txtCompanyName;
        private System.Windows.Forms.Label lblNationalID;
        private System.Windows.Forms.TextBox txtNationalID;
        private System.Windows.Forms.Label lblEconomicCode;
        private System.Windows.Forms.TextBox txtEconomicCode;
        private System.Windows.Forms.Label lblRegistrationNo;
        private System.Windows.Forms.TextBox txtRegistrationNo;
        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblColor;
        private System.Windows.Forms.ComboBox cmbColor;
        private System.Windows.Forms.CheckBox chkIsActive;
        private System.Windows.Forms.Button btnSave;
    }
}
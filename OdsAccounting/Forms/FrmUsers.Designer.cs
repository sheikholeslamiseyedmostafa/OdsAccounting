// فایل دیزاینر فرم - قابل ویرایش در Visual Studio Designer
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OdsAccounting
{
    partial class FrmUsers
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlTitle;
        private FlowLayoutPanel pnlFields;
        private FlowLayoutPanel pnlButtons;
        private SplitContainer splitMain;
        private Label lblHeader;
        private DataGridView dgvUsers;
        private DataGridView dgvAudit;
        private Label lblUser;
        private TextBox txtUser;
        private Label lblFull;
        private TextBox txtFull;
        private Label lblPass;
        private TextBox txtPass;
        private Label lblRole;
        private ComboBox cmbRole;
        private CheckBox chkActive;
        private Button btnNew;
        private Button btnSave;
        private Button btnResetPass;
        private Button btnRefresh;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlTitle = new Panel();
            this.pnlFields = new FlowLayoutPanel();
            this.pnlButtons = new FlowLayoutPanel();
            this.splitMain = new SplitContainer();
            this.lblHeader = new Label();
            this.dgvUsers = new DataGridView();
            this.dgvAudit = new DataGridView();
            this.lblUser = new Label();
            this.txtUser = new TextBox();
            this.lblFull = new Label();
            this.txtFull = new TextBox();
            this.lblPass = new Label();
            this.txtPass = new TextBox();
            this.lblRole = new Label();
            this.cmbRole = new ComboBox();
            this.chkActive = new CheckBox();
            this.btnNew = new Button();
            this.btnSave = new Button();
            this.btnResetPass = new Button();
            this.btnRefresh = new Button();
            this.SuspendLayout();
            this.pnlTitle.Dock = DockStyle.Top;
            this.pnlTitle.Height = 56;
            this.pnlTitle.BackColor = Ui.Primary;
            this.pnlFields.Dock = DockStyle.Top;
            this.pnlFields.Height = 60;
            this.pnlFields.Padding = new Padding(8, 6, 8, 6);
            this.pnlFields.AutoScroll = false;
            this.pnlButtons.Dock = DockStyle.Top;
            this.pnlButtons.Height = 56;
            this.pnlButtons.Padding = new Padding(8, 6, 8, 6);
            this.pnlButtons.AutoScroll = false;
            this.splitMain.Dock = DockStyle.Fill;
            this.splitMain.Orientation = Orientation.Horizontal;
            this.splitMain.Panel1MinSize = 120;
            this.lblHeader.Dock = DockStyle.Fill;
            this.lblHeader.Text = "مدیریت کاربران، نقش‌ها و امنیت";
            this.lblHeader.ForeColor = Color.White;
            this.lblHeader.TextAlign = ContentAlignment.MiddleCenter;
            this.dgvUsers.Dock = DockStyle.Fill;
            this.dgvUsers.ReadOnly = true;
            this.dgvAudit.Dock = DockStyle.Fill;
            this.dgvAudit.ReadOnly = true;
            this.lblUser.Text = "نام کاربری:";
            this.lblUser.AutoSize = true;
            this.lblUser.Margin = new Padding(4, 10, 4, 4);
            this.txtUser.Width = 140;
            this.lblFull.Text = "نام کامل:";
            this.lblFull.AutoSize = true;
            this.lblFull.Margin = new Padding(4, 10, 4, 4);
            this.txtFull.Width = 180;
            this.lblPass.Text = "کلمه عبور:";
            this.lblPass.AutoSize = true;
            this.lblPass.Margin = new Padding(4, 10, 4, 4);
            this.txtPass.Width = 160;
            this.lblRole.Text = "نقش:";
            this.lblRole.AutoSize = true;
            this.lblRole.Margin = new Padding(4, 10, 4, 4);
            this.cmbRole.Width = 150;
            this.cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;
            this.chkActive.Text = "فعال";
            this.chkActive.AutoSize = true;
            this.chkActive.Margin = new Padding(4, 8, 4, 4);
            this.btnNew.Text = "کاربر جدید";
            this.btnNew.Width = 140;
            this.btnNew.Height = 38;
            this.btnNew.Margin = new Padding(4);
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.BackColor = Color.FromArgb(0, 84, 147);
            this.btnNew.ForeColor = Color.White;
            this.btnNew.FlatStyle = FlatStyle.Flat;
            this.btnNew.Click += BtnNew_Click;
            this.btnSave.Text = "ذخیره کاربر";
            this.btnSave.Width = 140;
            this.btnSave.Height = 38;
            this.btnSave.Margin = new Padding(4);
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.BackColor = Color.FromArgb(0, 84, 147);
            this.btnSave.ForeColor = Color.White;
            this.btnSave.FlatStyle = FlatStyle.Flat;
            this.btnSave.Click += BtnSave_Click;
            this.btnResetPass.Text = "تغییر کلمه عبور";
            this.btnResetPass.Width = 170;
            this.btnResetPass.Height = 38;
            this.btnResetPass.Margin = new Padding(4);
            this.btnResetPass.UseVisualStyleBackColor = false;
            this.btnResetPass.BackColor = Color.FromArgb(0, 84, 147);
            this.btnResetPass.ForeColor = Color.White;
            this.btnResetPass.FlatStyle = FlatStyle.Flat;
            this.btnResetPass.Click += BtnResetPass_Click;
            this.btnRefresh.Text = "بازخوانی";
            this.btnRefresh.Width = 120;
            this.btnRefresh.Height = 38;
            this.btnRefresh.Margin = new Padding(4);
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.BackColor = Color.FromArgb(0, 84, 147);
            this.btnRefresh.ForeColor = Color.White;
            this.btnRefresh.FlatStyle = FlatStyle.Flat;
            this.btnRefresh.Click += BtnRefresh_Click;
            // ---- ساختار کنترل‌ها ----
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.pnlFields);
            this.Controls.Add(this.pnlTitle);
            this.pnlTitle.Controls.Add(this.lblHeader);
            this.splitMain.Panel1.Controls.Add(this.dgvUsers);
            this.splitMain.Panel2.Controls.Add(this.dgvAudit);
            this.pnlFields.Controls.Add(this.lblUser);
            this.pnlFields.Controls.Add(this.txtUser);
            this.pnlFields.Controls.Add(this.lblFull);
            this.pnlFields.Controls.Add(this.txtFull);
            this.pnlFields.Controls.Add(this.lblPass);
            this.pnlFields.Controls.Add(this.txtPass);
            this.pnlFields.Controls.Add(this.lblRole);
            this.pnlFields.Controls.Add(this.cmbRole);
            this.pnlFields.Controls.Add(this.chkActive);
            this.pnlButtons.Controls.Add(this.btnNew);
            this.pnlButtons.Controls.Add(this.btnSave);
            this.pnlButtons.Controls.Add(this.btnResetPass);
            this.pnlButtons.Controls.Add(this.btnRefresh);
            // ---- ترتیب z-order (برای Dock صحیح) ----
            this.pnlTitle.SendToBack();
            this.pnlFields.SendToBack();
            this.pnlButtons.SendToBack();
            this.splitMain.SendToBack();
            this.lblHeader.SendToBack();
            this.dgvUsers.SendToBack();
            this.dgvAudit.SendToBack();
            this.lblUser.SendToBack();
            this.txtUser.SendToBack();
            this.lblFull.SendToBack();
            this.txtFull.SendToBack();
            this.lblPass.SendToBack();
            this.txtPass.SendToBack();
            this.lblRole.SendToBack();
            this.cmbRole.SendToBack();
            this.chkActive.SendToBack();
            this.btnNew.SendToBack();
            this.btnSave.SendToBack();
            this.btnResetPass.SendToBack();
            this.btnRefresh.SendToBack();
            // ---- فرم ----
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1200, 740);
            this.Name = "FrmUsers";
            this.Text = "کاربران و امنیت";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Load += FrmUsers_Load;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

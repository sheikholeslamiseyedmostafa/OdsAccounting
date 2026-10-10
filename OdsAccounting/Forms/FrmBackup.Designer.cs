// فایل دیزاینر فرم - قابل ویرایش در Visual Studio Designer
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OdsAccounting
{
    partial class FrmBackup
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlTitle;
        private FlowLayoutPanel pnlFields;
        private FlowLayoutPanel pnlButtons;
        private DataGridView dgvHistory;
        private Label lblHeader;
        private Label lblPath;
        private TextBox txtPath;
        private Button btnBrowse;
        private Button btnBackup;
        private Button btnRestore;
        private Button btnRefresh;
        private Label lblNote;

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
            this.dgvHistory = new DataGridView();
            this.lblHeader = new Label();
            this.lblPath = new Label();
            this.txtPath = new TextBox();
            this.btnBrowse = new Button();
            this.btnBackup = new Button();
            this.btnRestore = new Button();
            this.btnRefresh = new Button();
            this.lblNote = new Label();
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
            this.dgvHistory.Dock = DockStyle.Fill;
            this.dgvHistory.ReadOnly = true;
            this.lblHeader.Dock = DockStyle.Fill;
            this.lblHeader.Text = "پشتیبان‌گیری و بازیابی اطلاعات (SQL Server 2025)";
            this.lblHeader.ForeColor = Color.White;
            this.lblHeader.TextAlign = ContentAlignment.MiddleCenter;
            this.lblPath.Text = "مسیر فایل پشتیبان (.bak):";
            this.lblPath.AutoSize = true;
            this.lblPath.Margin = new Padding(4, 10, 4, 4);
            this.txtPath.Width = 600;
            this.btnBrowse.Text = "انتخاب مسیر";
            this.btnBrowse.Width = 140;
            this.btnBrowse.Height = 38;
            this.btnBrowse.Margin = new Padding(4);
            this.btnBrowse.UseVisualStyleBackColor = false;
            this.btnBrowse.BackColor = Color.FromArgb(0, 84, 147);
            this.btnBrowse.ForeColor = Color.White;
            this.btnBrowse.FlatStyle = FlatStyle.Flat;
            this.btnBrowse.Click += BtnBrowse_Click;
            this.btnBackup.Text = "پشتیبان‌گیری";
            this.btnBackup.Width = 150;
            this.btnBackup.Height = 38;
            this.btnBackup.Margin = new Padding(4);
            this.btnBackup.UseVisualStyleBackColor = false;
            this.btnBackup.BackColor = Color.FromArgb(0, 84, 147);
            this.btnBackup.ForeColor = Color.White;
            this.btnBackup.FlatStyle = FlatStyle.Flat;
            this.btnBackup.Click += BtnBackup_Click;
            this.btnRestore.Text = "بازیابی از فایل";
            this.btnRestore.Width = 160;
            this.btnRestore.Height = 38;
            this.btnRestore.Margin = new Padding(4);
            this.btnRestore.UseVisualStyleBackColor = false;
            this.btnRestore.BackColor = Color.FromArgb(0, 84, 147);
            this.btnRestore.ForeColor = Color.White;
            this.btnRestore.FlatStyle = FlatStyle.Flat;
            this.btnRestore.Click += BtnRestore_Click;
            this.btnRefresh.Text = "تاریخچه";
            this.btnRefresh.Width = 120;
            this.btnRefresh.Height = 38;
            this.btnRefresh.Margin = new Padding(4);
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.BackColor = Color.FromArgb(0, 84, 147);
            this.btnRefresh.ForeColor = Color.White;
            this.btnRefresh.FlatStyle = FlatStyle.Flat;
            this.btnRefresh.Click += BtnRefresh_Click;
            this.lblNote.AutoSize = true;
            this.lblNote.Text = "مسیر باید از دید سرویس SQL Server قابل دسترسی باشد.";
            this.lblNote.Margin = new Padding(12,10,4,4);
            // ---- ساختار کنترل‌ها ----
            this.Controls.Add(this.dgvHistory);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.pnlFields);
            this.Controls.Add(this.pnlTitle);
            this.pnlTitle.Controls.Add(this.lblHeader);
            this.pnlFields.Controls.Add(this.lblPath);
            this.pnlFields.Controls.Add(this.txtPath);
            this.pnlButtons.Controls.Add(this.btnBrowse);
            this.pnlButtons.Controls.Add(this.btnBackup);
            this.pnlButtons.Controls.Add(this.btnRestore);
            this.pnlButtons.Controls.Add(this.btnRefresh);
            this.pnlButtons.Controls.Add(this.lblNote);
            // ---- ترتیب z-order (برای Dock صحیح) ----
            this.pnlTitle.SendToBack();
            this.pnlFields.SendToBack();
            this.pnlButtons.SendToBack();
            this.dgvHistory.SendToBack();
            this.lblHeader.SendToBack();
            this.lblPath.SendToBack();
            this.txtPath.SendToBack();
            this.btnBrowse.SendToBack();
            this.btnBackup.SendToBack();
            this.btnRestore.SendToBack();
            this.btnRefresh.SendToBack();
            this.lblNote.SendToBack();
            // ---- فرم ----
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1150, 700);
            this.Name = "FrmBackup";
            this.Text = "پشتیبان‌گیری و بازیابی اطلاعات";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Load += FrmBackup_Load;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

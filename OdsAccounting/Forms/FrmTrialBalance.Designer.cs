// فایل دیزاینر فرم - قابل ویرایش در Visual Studio Designer
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OdsAccounting
{
    partial class FrmTrialBalance
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlTitle;
        private FlowLayoutPanel pnlFields;
        private FlowLayoutPanel pnlButtons;
        private DataGridView dgvReport;
        private Label lblHeader;
        private Label lblReport;
        private ComboBox cmbReport;
        private Label lblFrom;
        private TextBox txtFrom;
        private Label lblTo;
        private TextBox txtTo;
        private Label lblLevel;
        private ComboBox cmbLevel;
        private Label lblAccount;
        private TextBox txtAccount;
        private Button btnRun;
        private Button btnExport;
        private Label lblSummary;

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
            this.dgvReport = new DataGridView();
            this.lblHeader = new Label();
            this.lblReport = new Label();
            this.cmbReport = new ComboBox();
            this.lblFrom = new Label();
            this.txtFrom = new TextBox();
            this.lblTo = new Label();
            this.txtTo = new TextBox();
            this.lblLevel = new Label();
            this.cmbLevel = new ComboBox();
            this.lblAccount = new Label();
            this.txtAccount = new TextBox();
            this.btnRun = new Button();
            this.btnExport = new Button();
            this.lblSummary = new Label();
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
            this.dgvReport.Dock = DockStyle.Fill;
            this.dgvReport.ReadOnly = true;
            this.lblHeader.Dock = DockStyle.Fill;
            this.lblHeader.Text = "گزارش‌ها و انواع ترازها";
            this.lblHeader.ForeColor = Color.White;
            this.lblHeader.TextAlign = ContentAlignment.MiddleCenter;
            this.lblReport.Text = "نوع گزارش:";
            this.lblReport.AutoSize = true;
            this.lblReport.Margin = new Padding(4, 10, 4, 4);
            this.cmbReport.Width = 240;
            this.cmbReport.DropDownStyle = ComboBoxStyle.DropDownList;
            this.lblFrom.Text = "از تاریخ:";
            this.lblFrom.AutoSize = true;
            this.lblFrom.Margin = new Padding(4, 10, 4, 4);
            this.txtFrom.Width = 120;
            this.lblTo.Text = "تا تاریخ:";
            this.lblTo.AutoSize = true;
            this.lblTo.Margin = new Padding(4, 10, 4, 4);
            this.txtTo.Width = 120;
            this.lblLevel.Text = "سطح تراز:";
            this.lblLevel.AutoSize = true;
            this.lblLevel.Margin = new Padding(4, 10, 4, 4);
            this.cmbLevel.Width = 120;
            this.cmbLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            this.lblAccount.Text = "کد حساب (کاردکس):";
            this.lblAccount.AutoSize = true;
            this.lblAccount.Margin = new Padding(4, 10, 4, 4);
            this.txtAccount.Width = 120;
            this.btnRun.Text = "نمایش گزارش";
            this.btnRun.Width = 150;
            this.btnRun.Height = 38;
            this.btnRun.Margin = new Padding(4);
            this.btnRun.UseVisualStyleBackColor = false;
            this.btnRun.BackColor = Color.FromArgb(0, 84, 147);
            this.btnRun.ForeColor = Color.White;
            this.btnRun.FlatStyle = FlatStyle.Flat;
            this.btnRun.Click += BtnRun_Click;
            this.btnExport.Text = "خروجی CSV";
            this.btnExport.Width = 150;
            this.btnExport.Height = 38;
            this.btnExport.Margin = new Padding(4);
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.BackColor = Color.FromArgb(0, 84, 147);
            this.btnExport.ForeColor = Color.White;
            this.btnExport.FlatStyle = FlatStyle.Flat;
            this.btnExport.Click += BtnExport_Click;
            this.lblSummary.AutoSize = true;
            this.lblSummary.Text = "آماده";
            this.lblSummary.Margin = new Padding(12,10,4,4);
            // ---- ساختار کنترل‌ها ----
            this.Controls.Add(this.dgvReport);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.pnlFields);
            this.Controls.Add(this.pnlTitle);
            this.pnlTitle.Controls.Add(this.lblHeader);
            this.pnlFields.Controls.Add(this.lblReport);
            this.pnlFields.Controls.Add(this.cmbReport);
            this.pnlFields.Controls.Add(this.lblFrom);
            this.pnlFields.Controls.Add(this.txtFrom);
            this.pnlFields.Controls.Add(this.lblTo);
            this.pnlFields.Controls.Add(this.txtTo);
            this.pnlFields.Controls.Add(this.lblLevel);
            this.pnlFields.Controls.Add(this.cmbLevel);
            this.pnlFields.Controls.Add(this.lblAccount);
            this.pnlFields.Controls.Add(this.txtAccount);
            this.pnlButtons.Controls.Add(this.btnRun);
            this.pnlButtons.Controls.Add(this.btnExport);
            this.pnlButtons.Controls.Add(this.lblSummary);
            // ---- ترتیب z-order (برای Dock صحیح) ----
            this.pnlTitle.SendToBack();
            this.pnlFields.SendToBack();
            this.pnlButtons.SendToBack();
            this.dgvReport.SendToBack();
            this.lblHeader.SendToBack();
            this.lblReport.SendToBack();
            this.cmbReport.SendToBack();
            this.lblFrom.SendToBack();
            this.txtFrom.SendToBack();
            this.lblTo.SendToBack();
            this.txtTo.SendToBack();
            this.lblLevel.SendToBack();
            this.cmbLevel.SendToBack();
            this.lblAccount.SendToBack();
            this.txtAccount.SendToBack();
            this.btnRun.SendToBack();
            this.btnExport.SendToBack();
            this.lblSummary.SendToBack();
            // ---- فرم ----
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1200, 740);
            this.Name = "FrmTrialBalance";
            this.Text = "گزارش‌ها و انواع تراز";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Load += FrmTrialBalance_Load;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

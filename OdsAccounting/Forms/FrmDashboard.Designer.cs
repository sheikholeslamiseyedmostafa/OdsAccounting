// فایل دیزاینر فرم - قابل ویرایش در Visual Studio Designer
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OdsAccounting
{
    partial class FrmDashboard
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlTitle;
        private FlowLayoutPanel pnlKpi;
        private Label lblWelcome;
        private Label lblHeader;
        private Label kpiVouchers;
        private Label kpiPending;
        private Label kpiInvoices;
        private Label kpiEmployees;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlTitle = new Panel();
            this.pnlKpi = new FlowLayoutPanel();
            this.lblWelcome = new Label();
            this.lblHeader = new Label();
            this.kpiVouchers = new Label();
            this.kpiPending = new Label();
            this.kpiInvoices = new Label();
            this.kpiEmployees = new Label();
            this.SuspendLayout();
            this.pnlTitle.Dock = DockStyle.Top;
            this.pnlTitle.Height = 56;
            this.pnlTitle.BackColor = Ui.Primary;
            this.pnlKpi.Dock = DockStyle.Top;
            this.pnlKpi.Height = 220;
            this.pnlKpi.Padding = new Padding(8, 6, 8, 6);
            this.pnlKpi.AutoScroll = false;
            this.lblWelcome.Dock = DockStyle.Fill;
            this.lblWelcome.Text = "به نرم‌افزار حسابداری ODS خوش آمدید.";
            this.lblWelcome.TextAlign = ContentAlignment.MiddleCenter;
            this.lblHeader.Dock = DockStyle.Fill;
            this.lblHeader.Text = "میز کار (داشبورد)";
            this.lblHeader.ForeColor = Color.White;
            this.lblHeader.TextAlign = ContentAlignment.MiddleCenter;
            this.kpiVouchers.Text = "اسناد قطعی: 0";
            this.kpiVouchers.AutoSize = true;
            this.kpiVouchers.Margin = new Padding(20,40,20,20);
            this.kpiPending.Text = "در انتظار تایید: 0";
            this.kpiPending.AutoSize = true;
            this.kpiPending.Margin = new Padding(20,40,20,20);
            this.kpiInvoices.Text = "فاکتورهای ارسال‌شده: 0";
            this.kpiInvoices.AutoSize = true;
            this.kpiInvoices.Margin = new Padding(20,40,20,20);
            this.kpiEmployees.Text = "پرسنل فعال: 0";
            this.kpiEmployees.AutoSize = true;
            this.kpiEmployees.Margin = new Padding(20,40,20,20);
            // ---- ساختار کنترل‌ها ----
            this.Controls.Add(this.pnlTitle);
            this.Controls.Add(this.pnlKpi);
            this.Controls.Add(this.lblWelcome);
            this.pnlTitle.Controls.Add(this.lblHeader);
            this.pnlKpi.Controls.Add(this.kpiVouchers);
            this.pnlKpi.Controls.Add(this.kpiPending);
            this.pnlKpi.Controls.Add(this.kpiInvoices);
            this.pnlKpi.Controls.Add(this.kpiEmployees);
            // ---- ترتیب z-order (برای Dock صحیح) ----
            this.pnlTitle.SendToBack();
            this.pnlKpi.SendToBack();
            this.lblWelcome.SendToBack();
            this.lblHeader.SendToBack();
            this.kpiVouchers.SendToBack();
            this.kpiPending.SendToBack();
            this.kpiInvoices.SendToBack();
            this.kpiEmployees.SendToBack();
            // ---- فرم ----
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1000, 600);
            this.Name = "FrmDashboard";
            this.Text = "میز کار";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Load += FrmDashboard_Load;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

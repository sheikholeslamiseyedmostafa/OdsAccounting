// فایل دیزاینر فرم - قابل ویرایش در Visual Studio Designer
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OdsAccounting
{
    partial class FrmSettings
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlTitle;
        private FlowLayoutPanel pnlFields;
        private FlowLayoutPanel pnlButtons;
        private Label lblHeader;
        private RadioButton rbLocal;
        private RadioButton rbCloud;
        private Label lblLocalConn;
        private TextBox txtLocalConn;
        private Label lblCloudConn;
        private TextBox txtCloudConn;
        private Label lblEndpoint;
        private TextBox txtEndpoint;
        private Label lblTaxId;
        private TextBox txtTaxId;
        private Label lblThumb;
        private TextBox txtThumb;
        private Button btnTest;
        private Button btnEnsure;
        private Button btnSave;
        private Label lblStatus;

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
            this.lblHeader = new Label();
            this.rbLocal = new RadioButton();
            this.rbCloud = new RadioButton();
            this.lblLocalConn = new Label();
            this.txtLocalConn = new TextBox();
            this.lblCloudConn = new Label();
            this.txtCloudConn = new TextBox();
            this.lblEndpoint = new Label();
            this.txtEndpoint = new TextBox();
            this.lblTaxId = new Label();
            this.txtTaxId = new TextBox();
            this.lblThumb = new Label();
            this.txtThumb = new TextBox();
            this.btnTest = new Button();
            this.btnEnsure = new Button();
            this.btnSave = new Button();
            this.lblStatus = new Label();
            this.SuspendLayout();
            this.pnlTitle.Dock = DockStyle.Top;
            this.pnlTitle.Height = 56;
            this.pnlTitle.BackColor = Ui.Primary;
            this.pnlFields.Dock = DockStyle.Top;
            this.pnlFields.Height = 330;
            this.pnlFields.Padding = new Padding(8, 6, 8, 6);
            this.pnlFields.AutoScroll = false;
            this.pnlButtons.Dock = DockStyle.Top;
            this.pnlButtons.Height = 56;
            this.pnlButtons.Padding = new Padding(8, 6, 8, 6);
            this.pnlButtons.AutoScroll = false;
            this.lblHeader.Dock = DockStyle.Fill;
            this.lblHeader.Text = "تنظیمات سیستم، اتصال پایگاه داده و مودیان";
            this.lblHeader.ForeColor = Color.White;
            this.lblHeader.TextAlign = ContentAlignment.MiddleCenter;
            this.rbLocal.Text = "حالت لوکال (شبکه داخلی)";
            this.rbLocal.AutoSize = true;
            this.rbLocal.Margin = new Padding(4,8,20,4);
            this.rbCloud.Text = "حالت ابری";
            this.rbCloud.AutoSize = true;
            this.rbCloud.Margin = new Padding(4,8,4,4);
            this.lblLocalConn.Text = "رشته اتصال لوکال:";
            this.lblLocalConn.AutoSize = true;
            this.lblLocalConn.Margin = new Padding(4, 10, 4, 4);
            this.txtLocalConn.Width = 760;
            this.lblCloudConn.Text = "رشته اتصال ابری:";
            this.lblCloudConn.AutoSize = true;
            this.lblCloudConn.Margin = new Padding(4, 10, 4, 4);
            this.txtCloudConn.Width = 760;
            this.lblEndpoint.Text = "آدرس API مودیان:";
            this.lblEndpoint.AutoSize = true;
            this.lblEndpoint.Margin = new Padding(4, 10, 4, 4);
            this.txtEndpoint.Width = 760;
            this.lblTaxId.Text = "شناسه حافظه مالیاتی (مؤدی):";
            this.lblTaxId.AutoSize = true;
            this.lblTaxId.Margin = new Padding(4, 10, 4, 4);
            this.txtTaxId.Width = 260;
            this.lblThumb.Text = "اثرانگشت گواهی امضا:";
            this.lblThumb.AutoSize = true;
            this.lblThumb.Margin = new Padding(4, 10, 4, 4);
            this.txtThumb.Width = 420;
            this.btnTest.Text = "آزمون اتصال";
            this.btnTest.Width = 150;
            this.btnTest.Height = 38;
            this.btnTest.Margin = new Padding(4);
            this.btnTest.UseVisualStyleBackColor = false;
            this.btnTest.BackColor = Color.FromArgb(0, 84, 147);
            this.btnTest.ForeColor = Color.White;
            this.btnTest.FlatStyle = FlatStyle.Flat;
            this.btnTest.Click += BtnTest_Click;
            this.btnEnsure.Text = "ایجاد/به‌روزرسانی ساختار DB";
            this.btnEnsure.Width = 260;
            this.btnEnsure.Height = 38;
            this.btnEnsure.Margin = new Padding(4);
            this.btnEnsure.UseVisualStyleBackColor = false;
            this.btnEnsure.BackColor = Color.FromArgb(0, 84, 147);
            this.btnEnsure.ForeColor = Color.White;
            this.btnEnsure.FlatStyle = FlatStyle.Flat;
            this.btnEnsure.Click += BtnEnsure_Click;
            this.btnSave.Text = "ذخیره تنظیمات";
            this.btnSave.Width = 150;
            this.btnSave.Height = 38;
            this.btnSave.Margin = new Padding(4);
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.BackColor = Color.FromArgb(0, 84, 147);
            this.btnSave.ForeColor = Color.White;
            this.btnSave.FlatStyle = FlatStyle.Flat;
            this.btnSave.Click += BtnSave_Click;
            this.lblStatus.AutoSize = true;
            this.lblStatus.Text = "";
            this.lblStatus.Margin = new Padding(12,10,4,4);
            // ---- ساختار کنترل‌ها ----
            this.Controls.Add(this.pnlTitle);
            this.Controls.Add(this.pnlFields);
            this.Controls.Add(this.pnlButtons);
            this.pnlTitle.Controls.Add(this.lblHeader);
            this.pnlFields.Controls.Add(this.rbLocal);
            this.pnlFields.Controls.Add(this.rbCloud);
            this.pnlFields.Controls.Add(this.lblLocalConn);
            this.pnlFields.Controls.Add(this.txtLocalConn);
            this.pnlFields.Controls.Add(this.lblCloudConn);
            this.pnlFields.Controls.Add(this.txtCloudConn);
            this.pnlFields.Controls.Add(this.lblEndpoint);
            this.pnlFields.Controls.Add(this.txtEndpoint);
            this.pnlFields.Controls.Add(this.lblTaxId);
            this.pnlFields.Controls.Add(this.txtTaxId);
            this.pnlFields.Controls.Add(this.lblThumb);
            this.pnlFields.Controls.Add(this.txtThumb);
            this.pnlButtons.Controls.Add(this.btnTest);
            this.pnlButtons.Controls.Add(this.btnEnsure);
            this.pnlButtons.Controls.Add(this.btnSave);
            this.pnlButtons.Controls.Add(this.lblStatus);
            // ---- ترتیب z-order (برای Dock صحیح) ----
            this.pnlTitle.SendToBack();
            this.pnlFields.SendToBack();
            this.pnlButtons.SendToBack();
            this.lblHeader.SendToBack();
            this.rbLocal.SendToBack();
            this.rbCloud.SendToBack();
            this.lblLocalConn.SendToBack();
            this.txtLocalConn.SendToBack();
            this.lblCloudConn.SendToBack();
            this.txtCloudConn.SendToBack();
            this.lblEndpoint.SendToBack();
            this.txtEndpoint.SendToBack();
            this.lblTaxId.SendToBack();
            this.txtTaxId.SendToBack();
            this.lblThumb.SendToBack();
            this.txtThumb.SendToBack();
            this.btnTest.SendToBack();
            this.btnEnsure.SendToBack();
            this.btnSave.SendToBack();
            this.lblStatus.SendToBack();
            // ---- فرم ----
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1150, 720);
            this.Name = "FrmSettings";
            this.Text = "تنظیمات سیستم";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Load += FrmSettings_Load;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

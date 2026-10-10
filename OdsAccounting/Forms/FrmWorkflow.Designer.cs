// فایل دیزاینر فرم - قابل ویرایش در Visual Studio Designer
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OdsAccounting
{
    partial class FrmWorkflow
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlTitle;
        private FlowLayoutPanel pnlFields;
        private FlowLayoutPanel pnlButtons;
        private SplitContainer splitMain;
        private Label lblHeader;
        private DataGridView dgvRequests;
        private DataGridView dgvSteps;
        private Label lblFilter;
        private ComboBox cmbFilter;
        private Label lblComment;
        private TextBox txtComment;
        private Button btnApprove;
        private Button btnReject;
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
            this.dgvRequests = new DataGridView();
            this.dgvSteps = new DataGridView();
            this.lblFilter = new Label();
            this.cmbFilter = new ComboBox();
            this.lblComment = new Label();
            this.txtComment = new TextBox();
            this.btnApprove = new Button();
            this.btnReject = new Button();
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
            this.lblHeader.Text = "گردش کار و تایید درخواست‌ها";
            this.lblHeader.ForeColor = Color.White;
            this.lblHeader.TextAlign = ContentAlignment.MiddleCenter;
            this.dgvRequests.Dock = DockStyle.Fill;
            this.dgvRequests.ReadOnly = true;
            this.dgvSteps.Dock = DockStyle.Fill;
            this.dgvSteps.ReadOnly = true;
            this.lblFilter.Text = "نمایش:";
            this.lblFilter.AutoSize = true;
            this.lblFilter.Margin = new Padding(4, 10, 4, 4);
            this.cmbFilter.Width = 200;
            this.cmbFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            this.lblComment.Text = "توضیح تایید/رد:";
            this.lblComment.AutoSize = true;
            this.lblComment.Margin = new Padding(4, 10, 4, 4);
            this.txtComment.Width = 360;
            this.btnApprove.Text = "تایید مرحله";
            this.btnApprove.Width = 150;
            this.btnApprove.Height = 38;
            this.btnApprove.Margin = new Padding(4);
            this.btnApprove.UseVisualStyleBackColor = false;
            this.btnApprove.BackColor = Color.FromArgb(0, 84, 147);
            this.btnApprove.ForeColor = Color.White;
            this.btnApprove.FlatStyle = FlatStyle.Flat;
            this.btnApprove.Click += BtnApprove_Click;
            this.btnReject.Text = "رد درخواست";
            this.btnReject.Width = 150;
            this.btnReject.Height = 38;
            this.btnReject.Margin = new Padding(4);
            this.btnReject.UseVisualStyleBackColor = false;
            this.btnReject.BackColor = Color.FromArgb(0, 84, 147);
            this.btnReject.ForeColor = Color.White;
            this.btnReject.FlatStyle = FlatStyle.Flat;
            this.btnReject.Click += BtnReject_Click;
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
            this.splitMain.Panel1.Controls.Add(this.dgvRequests);
            this.splitMain.Panel2.Controls.Add(this.dgvSteps);
            this.pnlFields.Controls.Add(this.lblFilter);
            this.pnlFields.Controls.Add(this.cmbFilter);
            this.pnlFields.Controls.Add(this.lblComment);
            this.pnlFields.Controls.Add(this.txtComment);
            this.pnlButtons.Controls.Add(this.btnApprove);
            this.pnlButtons.Controls.Add(this.btnReject);
            this.pnlButtons.Controls.Add(this.btnRefresh);
            // ---- ترتیب z-order (برای Dock صحیح) ----
            // ---- فرم ----
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1200, 740);
            this.Name = "FrmWorkflow";
            this.Text = "گردش کار و تایید";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Load += FrmWorkflow_Load;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

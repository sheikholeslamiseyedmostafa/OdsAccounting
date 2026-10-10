// فایل دیزاینر فرم - قابل ویرایش در Visual Studio Designer
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OdsAccounting
{
    partial class FrmDimensions
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlTitle;
        private FlowLayoutPanel pnlFields;
        private FlowLayoutPanel pnlButtons;
        private DataGridView dgvItems;
        private Label lblHeader;
        private Label lblCategory;
        private ComboBox cmbCategory;
        private Label lblCode;
        private TextBox txtCode;
        private Label lblName;
        private TextBox txtName;
        private Label lblNatId;
        private TextBox txtNationalId;
        private Label lblEco;
        private TextBox txtEconomicCode;
        private CheckBox chkActive;
        private Button btnNew;
        private Button btnSave;
        private Button btnDelete;
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
            this.dgvItems = new DataGridView();
            this.lblHeader = new Label();
            this.lblCategory = new Label();
            this.cmbCategory = new ComboBox();
            this.lblCode = new Label();
            this.txtCode = new TextBox();
            this.lblName = new Label();
            this.txtName = new TextBox();
            this.lblNatId = new Label();
            this.txtNationalId = new TextBox();
            this.lblEco = new Label();
            this.txtEconomicCode = new TextBox();
            this.chkActive = new CheckBox();
            this.btnNew = new Button();
            this.btnSave = new Button();
            this.btnDelete = new Button();
            this.btnRefresh = new Button();
            this.SuspendLayout();
            this.pnlTitle.Dock = DockStyle.Top;
            this.pnlTitle.Height = 56;
            this.pnlTitle.BackColor = Ui.Primary;
            this.pnlFields.Dock = DockStyle.Top;
            this.pnlFields.Height = 110;
            this.pnlFields.Padding = new Padding(8, 6, 8, 6);
            this.pnlFields.AutoScroll = false;
            this.pnlButtons.Dock = DockStyle.Top;
            this.pnlButtons.Height = 56;
            this.pnlButtons.Padding = new Padding(8, 6, 8, 6);
            this.pnlButtons.AutoScroll = false;
            this.dgvItems.Dock = DockStyle.Fill;
            this.dgvItems.ReadOnly = true;
            this.lblHeader.Dock = DockStyle.Fill;
            this.lblHeader.Text = "مراکز هزینه، پروژه‌ها و حساب‌های شناور";
            this.lblHeader.ForeColor = Color.White;
            this.lblHeader.TextAlign = ContentAlignment.MiddleCenter;
            this.lblCategory.Text = "نوع:";
            this.lblCategory.AutoSize = true;
            this.lblCategory.Margin = new Padding(4, 10, 4, 4);
            this.cmbCategory.Width = 200;
            this.cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            this.lblCode.Text = "کد:";
            this.lblCode.AutoSize = true;
            this.lblCode.Margin = new Padding(4, 10, 4, 4);
            this.txtCode.Width = 120;
            this.lblName.Text = "عنوان / نام:";
            this.lblName.AutoSize = true;
            this.lblName.Margin = new Padding(4, 10, 4, 4);
            this.txtName.Width = 260;
            this.lblNatId.Text = "شناسه ملی:";
            this.lblNatId.AutoSize = true;
            this.lblNatId.Margin = new Padding(4, 10, 4, 4);
            this.txtNationalId.Width = 160;
            this.lblEco.Text = "کد اقتصادی:";
            this.lblEco.AutoSize = true;
            this.lblEco.Margin = new Padding(4, 10, 4, 4);
            this.txtEconomicCode.Width = 160;
            this.chkActive.Text = "فعال";
            this.chkActive.AutoSize = true;
            this.chkActive.Margin = new Padding(4, 8, 4, 4);
            this.btnNew.Text = "جدید";
            this.btnNew.Width = 130;
            this.btnNew.Height = 38;
            this.btnNew.Margin = new Padding(4);
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.BackColor = Color.FromArgb(0, 84, 147);
            this.btnNew.ForeColor = Color.White;
            this.btnNew.FlatStyle = FlatStyle.Flat;
            this.btnNew.Click += BtnNew_Click;
            this.btnSave.Text = "ذخیره";
            this.btnSave.Width = 130;
            this.btnSave.Height = 38;
            this.btnSave.Margin = new Padding(4);
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.BackColor = Color.FromArgb(0, 84, 147);
            this.btnSave.ForeColor = Color.White;
            this.btnSave.FlatStyle = FlatStyle.Flat;
            this.btnSave.Click += BtnSave_Click;
            this.btnDelete.Text = "حذف";
            this.btnDelete.Width = 130;
            this.btnDelete.Height = 38;
            this.btnDelete.Margin = new Padding(4);
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.BackColor = Color.FromArgb(0, 84, 147);
            this.btnDelete.ForeColor = Color.White;
            this.btnDelete.FlatStyle = FlatStyle.Flat;
            this.btnDelete.Click += BtnDelete_Click;
            this.btnRefresh.Text = "بازخوانی";
            this.btnRefresh.Width = 130;
            this.btnRefresh.Height = 38;
            this.btnRefresh.Margin = new Padding(4);
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.BackColor = Color.FromArgb(0, 84, 147);
            this.btnRefresh.ForeColor = Color.White;
            this.btnRefresh.FlatStyle = FlatStyle.Flat;
            this.btnRefresh.Click += BtnRefresh_Click;
            // ---- ساختار کنترل‌ها ----
            this.Controls.Add(this.dgvItems);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.pnlFields);
            this.Controls.Add(this.pnlTitle);
            this.pnlTitle.Controls.Add(this.lblHeader);
            this.pnlFields.Controls.Add(this.lblCategory);
            this.pnlFields.Controls.Add(this.cmbCategory);
            this.pnlFields.Controls.Add(this.lblCode);
            this.pnlFields.Controls.Add(this.txtCode);
            this.pnlFields.Controls.Add(this.lblName);
            this.pnlFields.Controls.Add(this.txtName);
            this.pnlFields.Controls.Add(this.lblNatId);
            this.pnlFields.Controls.Add(this.txtNationalId);
            this.pnlFields.Controls.Add(this.lblEco);
            this.pnlFields.Controls.Add(this.txtEconomicCode);
            this.pnlFields.Controls.Add(this.chkActive);
            this.pnlButtons.Controls.Add(this.btnNew);
            this.pnlButtons.Controls.Add(this.btnSave);
            this.pnlButtons.Controls.Add(this.btnDelete);
            this.pnlButtons.Controls.Add(this.btnRefresh);
            // ---- ترتیب z-order (برای Dock صحیح) ----
            // ---- فرم ----
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1150, 700);
            this.Name = "FrmDimensions";
            this.Text = "مراکز هزینه، پروژه و حساب‌های شناور";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Load += FrmDimensions_Load;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

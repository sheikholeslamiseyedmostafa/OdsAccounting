using System.Drawing;
using System.Windows.Forms;

namespace OdsAccounting
{
    partial class FrmAddFinancialPeriod
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
            layoutMain = new TableLayoutPanel();
            lblCompany = new Label();
            txtCompanyName = new TextBox();
            lblPeriodName = new Label();
            txtPeriodName = new TextBox();
            lblStartDate = new Label();
            pnlStartDate = new Panel();
            txtStartDate = new TextBox();
            btnStartDateCalendar = new Button();
            lblEndDate = new Label();
            pnlEndDate = new Panel();
            txtEndDate = new TextBox();
            btnEndDateCalendar = new Button();
            lblOpenStatus = new Label();
            chkPeriodOpen = new CheckBox();
            lblDescription = new Label();
            txtDescription = new TextBox();
            lblColor = new Label();
            cmbColor = new ComboBox();
            lblActive = new Label();
            chkIsActive = new CheckBox();
            pnlButtons = new FlowLayoutPanel();
            btnSave = new Button();
            btnCancel = new Button();
            layoutMain.SuspendLayout();
            pnlStartDate.SuspendLayout();
            pnlEndDate.SuspendLayout();
            pnlButtons.SuspendLayout();
            SuspendLayout();
            //
            // layoutMain
            //
            layoutMain.ColumnCount = 2;
            layoutMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 23F));
            layoutMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 77F));
            layoutMain.Controls.Add(lblCompany, 0, 0);
            layoutMain.Controls.Add(txtCompanyName, 1, 0);
            layoutMain.Controls.Add(lblPeriodName, 0, 1);
            layoutMain.Controls.Add(txtPeriodName, 1, 1);
            layoutMain.Controls.Add(lblStartDate, 0, 2);
            layoutMain.Controls.Add(pnlStartDate, 1, 2);
            layoutMain.Controls.Add(lblEndDate, 0, 3);
            layoutMain.Controls.Add(pnlEndDate, 1, 3);
            layoutMain.Controls.Add(lblOpenStatus, 0, 4);
            layoutMain.Controls.Add(chkPeriodOpen, 1, 4);
            layoutMain.Controls.Add(lblDescription, 0, 5);
            layoutMain.Controls.Add(txtDescription, 1, 5);
            layoutMain.Controls.Add(lblColor, 0, 6);
            layoutMain.Controls.Add(cmbColor, 1, 6);
            layoutMain.Controls.Add(lblActive, 0, 7);
            layoutMain.Controls.Add(chkIsActive, 1, 7);
            layoutMain.Controls.Add(pnlButtons, 0, 8);
            layoutMain.SetColumnSpan(pnlButtons, 2);
            layoutMain.Dock = DockStyle.Fill;
            layoutMain.Location = new Point(0, 0);
            layoutMain.Name = "layoutMain";
            layoutMain.Padding = new Padding(12);
            layoutMain.RightToLeft = RightToLeft.Yes;
            layoutMain.RowCount = 9;
            layoutMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            layoutMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            layoutMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            layoutMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            layoutMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
            layoutMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            layoutMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            layoutMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
            layoutMain.Size = new Size(1040, 560);
            layoutMain.TabIndex = 0;
            //
            // lblCompany
            //
            lblCompany.AutoSize = false;
            lblCompany.Dock = DockStyle.Fill;
            lblCompany.Name = "lblCompany";
            lblCompany.Text = "شرکت :";
            lblCompany.TextAlign = ContentAlignment.MiddleRight;
            //
            // txtCompanyName
            //
            txtCompanyName.AutoSize = false;
            txtCompanyName.Dock = DockStyle.Fill;
            txtCompanyName.Name = "txtCompanyName";
            txtCompanyName.ReadOnly = true;
            txtCompanyName.TabIndex = 0;
            txtCompanyName.TextAlign = HorizontalAlignment.Right;
            //
            // lblPeriodName
            //
            lblPeriodName.AutoSize = false;
            lblPeriodName.Dock = DockStyle.Fill;
            lblPeriodName.Name = "lblPeriodName";
            lblPeriodName.Text = "نام دوره مالی (*) :";
            lblPeriodName.TextAlign = ContentAlignment.MiddleRight;
            //
            // txtPeriodName
            //
            txtPeriodName.AutoSize = false;
            txtPeriodName.Dock = DockStyle.Fill;
            txtPeriodName.MaxLength = 200;
            txtPeriodName.Name = "txtPeriodName";
            txtPeriodName.TabIndex = 1;
            txtPeriodName.TextAlign = HorizontalAlignment.Right;
            //
            // lblStartDate
            //
            lblStartDate.AutoSize = false;
            lblStartDate.Dock = DockStyle.Fill;
            lblStartDate.Name = "lblStartDate";
            lblStartDate.Text = "تاریخ شروع (*) :";
            lblStartDate.TextAlign = ContentAlignment.MiddleRight;
            //
            // pnlStartDate
            //
            pnlStartDate.Controls.Add(txtStartDate);
            pnlStartDate.Controls.Add(btnStartDateCalendar);
            pnlStartDate.Dock = DockStyle.Fill;
            pnlStartDate.Name = "pnlStartDate";
            pnlStartDate.TabIndex = 2;
            //
            // txtStartDate
            //
            txtStartDate.AutoSize = false;
            txtStartDate.Dock = DockStyle.Fill;
            txtStartDate.MaxLength = 10;
            txtStartDate.Name = "txtStartDate";
            txtStartDate.PlaceholderText = "yyyy/MM/dd";
            txtStartDate.TabIndex = 0;
            txtStartDate.TextAlign = HorizontalAlignment.Right;
            //
            // btnStartDateCalendar
            //
            btnStartDateCalendar.Dock = DockStyle.Right;
            btnStartDateCalendar.Name = "btnStartDateCalendar";
            btnStartDateCalendar.TabIndex = 1;
            btnStartDateCalendar.Text = "تقویم";
            btnStartDateCalendar.UseVisualStyleBackColor = true;
            btnStartDateCalendar.Width = 76;
            //
            // lblEndDate
            //
            lblEndDate.AutoSize = false;
            lblEndDate.Dock = DockStyle.Fill;
            lblEndDate.Name = "lblEndDate";
            lblEndDate.Text = "تاریخ پایان (*) :";
            lblEndDate.TextAlign = ContentAlignment.MiddleRight;
            //
            // pnlEndDate
            //
            pnlEndDate.Controls.Add(txtEndDate);
            pnlEndDate.Controls.Add(btnEndDateCalendar);
            pnlEndDate.Dock = DockStyle.Fill;
            pnlEndDate.Name = "pnlEndDate";
            pnlEndDate.TabIndex = 3;
            //
            // txtEndDate
            //
            txtEndDate.AutoSize = false;
            txtEndDate.Dock = DockStyle.Fill;
            txtEndDate.MaxLength = 10;
            txtEndDate.Name = "txtEndDate";
            txtEndDate.PlaceholderText = "yyyy/MM/dd";
            txtEndDate.TabIndex = 0;
            txtEndDate.TextAlign = HorizontalAlignment.Right;
            //
            // btnEndDateCalendar
            //
            btnEndDateCalendar.Dock = DockStyle.Right;
            btnEndDateCalendar.Name = "btnEndDateCalendar";
            btnEndDateCalendar.TabIndex = 1;
            btnEndDateCalendar.Text = "تقویم";
            btnEndDateCalendar.UseVisualStyleBackColor = true;
            btnEndDateCalendar.Width = 76;
            //
            // lblOpenStatus
            //
            lblOpenStatus.AutoSize = false;
            lblOpenStatus.Dock = DockStyle.Fill;
            lblOpenStatus.Name = "lblOpenStatus";
            lblOpenStatus.Text = "باز/بسته بودن دوره :";
            lblOpenStatus.TextAlign = ContentAlignment.MiddleRight;
            //
            // chkPeriodOpen
            //
            chkPeriodOpen.Appearance = Appearance.Button;
            chkPeriodOpen.AutoSize = false;
            chkPeriodOpen.Dock = DockStyle.Fill;
            chkPeriodOpen.FlatStyle = FlatStyle.Flat;
            chkPeriodOpen.Name = "chkPeriodOpen";
            chkPeriodOpen.Text = "دوره مالی باز است";
            chkPeriodOpen.CheckAlign = ContentAlignment.MiddleRight;
            chkPeriodOpen.TextAlign = ContentAlignment.MiddleRight;
            chkPeriodOpen.UseVisualStyleBackColor = false;
            chkPeriodOpen.Checked = true;
            chkPeriodOpen.TabIndex = 4;
            //
            // lblDescription
            //
            lblDescription.AutoSize = false;
            lblDescription.Dock = DockStyle.Fill;
            lblDescription.Name = "lblDescription";
            lblDescription.Text = "توضیحات :";
            lblDescription.TextAlign = ContentAlignment.TopRight;
            lblDescription.Padding = new Padding(0, 8, 0, 0);
            //
            // txtDescription
            //
            txtDescription.Dock = DockStyle.Fill;
            txtDescription.Multiline = true;
            txtDescription.Name = "txtDescription";
            txtDescription.ScrollBars = ScrollBars.Vertical;
            txtDescription.TabIndex = 5;
            txtDescription.TextAlign = HorizontalAlignment.Right;
            //
            // lblColor
            //
            lblColor.AutoSize = false;
            lblColor.Dock = DockStyle.Fill;
            lblColor.Name = "lblColor";
            lblColor.Text = "رنگ :";
            lblColor.TextAlign = ContentAlignment.MiddleRight;
            //
            // cmbColor
            //
            cmbColor.Dock = DockStyle.Fill;
            cmbColor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbColor.FormattingEnabled = true;
            cmbColor.Items.AddRange(new object[] { "بدون رنگ", "قرمز", "آبی", "سبز", "زرد" });
            cmbColor.Name = "cmbColor";
            cmbColor.TabIndex = 6;
            //
            // lblActive
            //
            lblActive.AutoSize = false;
            lblActive.Dock = DockStyle.Fill;
            lblActive.Name = "lblActive";
            lblActive.Text = "وضعیت :";
            lblActive.TextAlign = ContentAlignment.MiddleRight;
            //
            // chkIsActive
            //
            chkIsActive.AutoSize = true;
            chkIsActive.Checked = true;
            chkIsActive.CheckState = CheckState.Checked;
            chkIsActive.Dock = DockStyle.Right;
            chkIsActive.Name = "chkIsActive";
            chkIsActive.Text = "فعال";
            chkIsActive.TextAlign = ContentAlignment.MiddleRight;
            chkIsActive.UseVisualStyleBackColor = true;
            chkIsActive.TabIndex = 7;
            //
            // pnlButtons
            //
            pnlButtons.Controls.Add(btnSave);
            pnlButtons.Controls.Add(btnCancel);
            pnlButtons.Dock = DockStyle.Fill;
            pnlButtons.FlowDirection = FlowDirection.RightToLeft;
            pnlButtons.Name = "pnlButtons";
            pnlButtons.Padding = new Padding(0, 8, 0, 0);
            pnlButtons.RightToLeft = RightToLeft.Yes;
            pnlButtons.TabIndex = 8;
            //
            // btnSave
            //
            btnSave.DialogResult = DialogResult.None;
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(120, 40);
            btnSave.TabIndex = 0;
            btnSave.Text = "ثبت";
            btnSave.UseVisualStyleBackColor = true;
            //
            // btnCancel
            //
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(120, 40);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "انصراف";
            btnCancel.UseVisualStyleBackColor = true;
            //
            // FrmAddFinancialPeriod
            //
            AcceptButton = btnSave;
            AutoScaleDimensions = new SizeF(12F, 26F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(1040, 560);
            Controls.Add(layoutMain);
            Font = new Font("B Nazanin", 12F, FontStyle.Bold, GraphicsUnit.Point, 178);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(900, 560);
            Name = "FrmAddFinancialPeriod";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            StartPosition = FormStartPosition.CenterParent;
            Text = "افزودن سال مالی جدید";
            layoutMain.ResumeLayout(false);
            layoutMain.PerformLayout();
            pnlStartDate.ResumeLayout(false);
            pnlStartDate.PerformLayout();
            pnlEndDate.ResumeLayout(false);
            pnlEndDate.PerformLayout();
            pnlButtons.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel layoutMain;
        private Label lblCompany;
        private TextBox txtCompanyName;
        private Label lblPeriodName;
        private TextBox txtPeriodName;
        private Label lblStartDate;
        private Panel pnlStartDate;
        private TextBox txtStartDate;
        private Button btnStartDateCalendar;
        private Label lblEndDate;
        private Panel pnlEndDate;
        private TextBox txtEndDate;
        private Button btnEndDateCalendar;
        private Label lblOpenStatus;
        private CheckBox chkPeriodOpen;
        private Label lblDescription;
        private TextBox txtDescription;
        private Label lblColor;
        private ComboBox cmbColor;
        private Label lblActive;
        private CheckBox chkIsActive;
        private FlowLayoutPanel pnlButtons;
        private Button btnSave;
        private Button btnCancel;
    }
}

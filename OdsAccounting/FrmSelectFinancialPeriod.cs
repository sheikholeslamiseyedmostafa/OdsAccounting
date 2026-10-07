using System;
using System.Data;
using System.Windows.Forms;

namespace OdsAccounting
{
    public partial class FrmSelectFinancialPeriod : Form
    {
        private readonly FinancialPeriodRepository periodRepository = new FinancialPeriodRepository();
        private readonly string selectedCompanyName;
        private readonly MainForm? mainForm;

        public FrmSelectFinancialPeriod() : this(Properties.Settings.Default.SelectedCompany, null)
        {
        }

        public FrmSelectFinancialPeriod(string companyName) : this(companyName, null)
        {
        }

        public FrmSelectFinancialPeriod(string companyName, MainForm? mainForm)
        {
            InitializeComponent();
            selectedCompanyName = companyName?.Trim() ?? string.Empty;
            this.mainForm = mainForm;

            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowTemplate.Height = 40;
            dataGridView1.RightToLeft = RightToLeft.Yes;

            ApplyFontToAllControls(this);

            btnAddPeriod.Click += btnAddPeriod_Click;
            btnSelect.Click += btnSelect_Click;
            btnCancel.Click += btnCancel_Click;
            dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
        }

        private void FrmSelectFinancialPeriod_Load(object? sender, EventArgs e)
        {
            LoadFinancialPeriods();
        }

        public void LoadFinancialPeriods()
        {
            if (string.IsNullOrWhiteSpace(selectedCompanyName))
            {
                dataGridView1.DataSource = null;
                MessageBox.Show("ابتدا یک شرکت را انتخاب کنید.", "انتخاب سال مالی", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CloseCurrentTab();
                return;
            }

            string currentCompanyName = Properties.Settings.Default.SelectedCompany?.Trim() ?? string.Empty;
            if (!string.Equals(currentCompanyName, selectedCompanyName, StringComparison.Ordinal))
            {
                dataGridView1.DataSource = null;
                MessageBox.Show("شرکت جاری تغییر کرده است. فرم انتخاب سال مالی را دوباره باز کنید.", "انتخاب سال مالی", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CloseCurrentTab();
                return;
            }

            try
            {
                DataTable periods = periodRepository.LoadForCompany(selectedCompanyName);
                dataGridView1.DataSource = periods;
                ConfigureColumns();
            }
            catch (Exception ex)
            {
                dataGridView1.DataSource = null;
                MessageBox.Show(
                    "خطا در شناسایی یا بارگذاری دفاتر مالی شرکت انتخاب‌شده:\n" + ex.Message,
                    "خطا",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ConfigureColumns()
        {
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ColumnHeadersVisible = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;

            SetColumnHeader("RowNumber", "ردیف", 0, 12F, DataGridViewContentAlignment.MiddleRight);
            SetColumnHeader("FinancialPeriodName", "نام دوره مالی", 1, 38F, DataGridViewContentAlignment.MiddleRight);
            SetColumnHeader("CompanyName", "نام شرکت", 2, 25F, DataGridViewContentAlignment.MiddleRight);
            SetColumnHeader("Description", "توضیحات", 3, 25F, DataGridViewContentAlignment.MiddleRight);
        }

        private void SetColumnHeader(
            string columnName,
            string headerText,
            int displayIndex,
            float fillWeight,
            DataGridViewContentAlignment alignment)
        {
            if (!dataGridView1.Columns.Contains(columnName))
            {
                return;
            }

            DataGridViewColumn column = dataGridView1.Columns[columnName];
            column.HeaderText = headerText;
            column.Visible = true;
            column.DisplayIndex = displayIndex;
            column.FillWeight = fillWeight;
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
            column.DefaultCellStyle.Alignment = alignment;
            column.DefaultCellStyle.Font = Font;
            column.HeaderCell.Style.Font = Font;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void ApplyFontToAllControls(Control parent)
        {
            parent.Font = Font;

            if (parent is DataGridView grid)
            {
                grid.Font = Font;
                grid.DefaultCellStyle.Font = Font;
                grid.AlternatingRowsDefaultCellStyle.Font = Font;
                grid.ColumnHeadersDefaultCellStyle.Font = Font;
                grid.RowHeadersDefaultCellStyle.Font = Font;
                grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                grid.AlternatingRowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (parent is ToolStrip toolStrip)
            {
                toolStrip.Font = Font;
                foreach (ToolStripItem item in toolStrip.Items)
                {
                    item.Font = Font;
                }
            }

            foreach (Control child in parent.Controls)
            {
                ApplyFontToAllControls(child);
            }
        }

        private void btnAddPeriod_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedCompanyName))
            {
                MessageBox.Show("ابتدا یک شرکت را انتخاب کنید.", "افزودن دوره مالی", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using FrmAddFinancialPeriod form = new FrmAddFinancialPeriod(selectedCompanyName);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadFinancialPeriods();
            }
        }

        private void btnSelect_Click(object? sender, EventArgs e)
        {
            DataGridViewRow? selectedRow = GetSelectedRow();
            if (selectedRow == null)
            {
                MessageBox.Show("لطفاً یک دفتر مالی را از جدول انتخاب کنید.", "اخطار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string currentCompanyName = Properties.Settings.Default.SelectedCompany?.Trim() ?? string.Empty;
            if (!string.Equals(currentCompanyName, selectedCompanyName, StringComparison.Ordinal))
            {
                MessageBox.Show("شرکت جاری تغییر کرده است. فرم انتخاب سال مالی را دوباره باز کنید.", "انتخاب سال مالی", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CloseCurrentTab();
                return;
            }

            string financialPeriodName = GetFinancialPeriodName(selectedRow);
            if (string.IsNullOrWhiteSpace(financialPeriodName))
            {
                MessageBox.Show("عنوان دوره مالی انتخاب‌شده خالی است.", "اخطار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MainForm? targetMainForm = mainForm;
            if (targetMainForm == null || targetMainForm.IsDisposed)
            {
                foreach (Form openForm in Application.OpenForms)
                {
                    if (openForm is MainForm openMainForm)
                    {
                        targetMainForm = openMainForm;
                        break;
                    }
                }
            }

            if (targetMainForm != null && !targetMainForm.IsDisposed)
            {
                targetMainForm.SetSelectedYear(financialPeriodName);
            }

            CloseCurrentTab();
        }

        private DataGridViewRow? GetSelectedRow()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                return dataGridView1.SelectedRows[0];
            }

            if (dataGridView1.SelectedCells.Count > 0)
            {
                int rowIndex = dataGridView1.SelectedCells[0].RowIndex;
                if (rowIndex >= 0 && rowIndex < dataGridView1.Rows.Count)
                {
                    return dataGridView1.Rows[rowIndex];
                }
            }

            return null;
        }

        private string GetFinancialPeriodName(DataGridViewRow row)
        {
            if (!dataGridView1.Columns.Contains("FinancialPeriodName"))
            {
                return string.Empty;
            }

            object? value = row.Cells["FinancialPeriodName"].Value;
            return value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value)?.Trim() ?? string.Empty;
        }

        private void dataGridView1_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnSelect_Click(sender, EventArgs.Empty);
            }
        }

        private void btnCancel_Click(object? sender, EventArgs e)
        {
            CloseCurrentTab();
        }

        private void CloseCurrentTab()
        {
            if (Parent is TabPage tabPage && tabPage.Parent is TabControl tabControl)
            {
                tabControl.TabPages.Remove(tabPage);
                tabPage.Dispose();
            }
            else
            {
                Close();
            }
        }
    }
}

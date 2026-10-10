using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace OdsAccounting
{
    public partial class FrmSelectFinancialPeriod : Form
    {
        private string connectionString => AppDb.ConnectionString;
        private string selectedCompanyName;

        public FrmSelectFinancialPeriod() : this(Properties.Settings.Default.SelectedCompany)
        {
        }

        public FrmSelectFinancialPeriod(string companyName)
        {
            InitializeComponent();
            Ui.ApplyFont(this);
            selectedCompanyName = companyName?.Trim() ?? string.Empty;

            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.RowTemplate.Height = 35;

            btnSelect.Click += btnSelect_Click;
            btnCancel.Click += btnCancel_Click;
            dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
        }

        private void FrmSelectFinancialPeriod_Load(object sender, EventArgs e)
        {
            LoadFinancialPeriods();
        }

        // فقط دفاتر مالی متعلق به شرکت جاری بارگذاری می‌شوند.
        public void LoadFinancialPeriods()
        {
            if (string.IsNullOrWhiteSpace(selectedCompanyName))
            {
                dataGridView1.DataSource = null;
                MessageBox.Show("ابتدا یک شرکت را انتخاب کنید.", "انتخاب سال مالی", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CloseCurrentTab();
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(@"
                    SELECT financialPeriod.*
                    FROM [ods].[SC_FinancialPeriods] AS financialPeriod
                    INNER JOIN [ods].[SC_Companies] AS company
                        ON company.CompanyID = financialPeriod.CompanyID
                    WHERE company.CompanyName = @CompanyName", connection))
                {
                    command.Parameters.AddWithValue("@CompanyName", selectedCompanyName);

                    DataTable periods = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(periods);
                    }

                    dataGridView1.DataSource = periods;
                    ConfigureColumns();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "خطا در بارگذاری دفاتر مالی شرکت انتخاب‌شده:\n" + ex.Message,
                    "خطا",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ConfigureColumns()
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.HeaderText = GetPersianHeader(column.Name);
                column.Visible = !IsTechnicalColumn(column.Name);
            }
        }

        private string GetPersianHeader(string columnName)
        {
            switch (columnName.ToLowerInvariant())
            {
                case "financialperiodname":
                case "financialyearname":
                case "periodname":
                case "accountingperiodname":
                case "bookname":
                case "yearname":
                case "financialyear":
                case "fiscalyear":
                case "year":
                    return "سال مالی";
                case "periodtitle":
                case "title":
                case "name":
                    return "عنوان";
                case "startdate":
                case "periodstartdate":
                    return "تاریخ شروع";
                case "enddate":
                case "periodenddate":
                    return "تاریخ پایان";
                case "isactive":
                    return "فعال";
                case "description":
                    return "توضیحات";
                case "companyid":
                    return "شناسه شرکت";
                default:
                    return columnName;
            }
        }

        private bool IsTechnicalColumn(string columnName)
        {
            return columnName.Equals("CompanyID", StringComparison.OrdinalIgnoreCase)
                || columnName.EndsWith("ID", StringComparison.OrdinalIgnoreCase);
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = GetSelectedRow();
            if (selectedRow == null)
            {
                MessageBox.Show("لطفاً یک دفتر مالی را از جدول انتخاب کنید.", "اخطار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // اگر شرکت پس از باز شدن این فرم تغییر کرده باشد، انتخاب قدیمی پذیرفته نمی‌شود.
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
                MessageBox.Show("عنوان سال مالی انتخاب‌شده قابل تشخیص نیست.", "اخطار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (Application.OpenForms["MainForm"] is MainForm mainForm)
            {
                mainForm.SetSelectedYear(financialPeriodName);
            }

            CloseCurrentTab();
        }

        private DataGridViewRow GetSelectedRow()
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
            string[] preferredColumns =
            {
                "FinancialPeriodName",
                "FinancialYearName",
                "PeriodName",
                "AccountingPeriodName",
                "BookName",
                "YearName",
                "FinancialYear",
                "FiscalYear",
                "PeriodTitle",
                "Year",
                "Title",
                "Name"
            };

            foreach (string columnName in preferredColumns)
            {
                string value = GetCellValue(row, columnName);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                if (!column.Visible || IsTechnicalColumn(column.Name))
                {
                    continue;
                }

                string value = GetCellValue(row, column.Name);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return string.Empty;
        }

        private string GetCellValue(DataGridViewRow row, string columnName)
        {
            if (!dataGridView1.Columns.Contains(columnName))
            {
                return string.Empty;
            }

            object value = row.Cells[columnName].Value;
            return value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value) ?? string.Empty;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnSelect_Click(sender, EventArgs.Empty);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
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

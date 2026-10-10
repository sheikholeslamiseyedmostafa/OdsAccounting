using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace OdsAccounting
{
    public partial class FrmSelectCompany : Form
    {
        // رشته اتصال به دیتابیس
        private string connectionString => AppDb.ConnectionString;

        public FrmSelectCompany()
        {
            InitializeComponent();
            Ui.ApplyFont(this);

            // اتصال خودکار رویداد لود شدن فرم
            this.Load += FrmSelectCompany_Load;

            // تنظیمات اولیه جدول
            dataGridView1.MultiSelect = false; // جلوگیری از انتخاب چند سطر
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // انتخاب کل سطر
            dataGridView1.AllowUserToAddRows = false; // حذف سطر خالیِ پیش‌فرض در انتهای جدول

            // تنظیمات ظاهری (عرض کامل، ارتفاع ردیف، سایز ستون‌ها)
            dataGridView1.Dock = DockStyle.Fill; // چسبیدن جدول به تمام لبه‌های فرم
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // پر کردن عرض توسط ستون‌ها
            dataGridView1.AllowUserToResizeColumns = true; // امکان تغییر عرض ستون‌ها با موس
            dataGridView1.RowTemplate.Height = 35; // افزایش ارتفاع ردیف‌ها برای خوانایی بهتر

            // اتصال رویدادها
            dataGridView1.Sorted += (s, e) => UpdateRowNumbers();
            dataGridView1.ColumnHeaderMouseClick += DataGridView1_ColumnHeaderMouseClick;
            dataGridView1.DataBindingComplete += DataGridView1_DataBindingComplete; // برای رنگی کردن ردیف‌ها
        }

        private void FrmSelectCompany_Load(object sender, EventArgs e)
        {
            LoadCompanies();
        }

        // بارگذاری اطلاعات شرکت‌ها
        public void LoadCompanies()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT CompanyID, CompanyName, NationalID, EconomicCode, RegistrationNo, Phone, Address, Description, ColorCode, IsActive FROM [ods].[SC_Companies]";
                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;

                        // مخفی کردن ستون کلید اصلی
                        if (dataGridView1.Columns["CompanyID"] != null)
                            dataGridView1.Columns["CompanyID"].Visible = false;

                        // تنظیم عناوین فارسی ستون‌ها
                        if (dataGridView1.Columns["CompanyName"] != null) dataGridView1.Columns["CompanyName"].HeaderText = "نام شرکت";
                        if (dataGridView1.Columns["NationalID"] != null) dataGridView1.Columns["NationalID"].HeaderText = "شناسه ملی";
                        if (dataGridView1.Columns["EconomicCode"] != null) dataGridView1.Columns["EconomicCode"].HeaderText = "شماره اقتصادی";
                        if (dataGridView1.Columns["RegistrationNo"] != null) dataGridView1.Columns["RegistrationNo"].HeaderText = "شماره ثبت";
                        if (dataGridView1.Columns["Phone"] != null) dataGridView1.Columns["Phone"].HeaderText = "تلفن";
                        if (dataGridView1.Columns["Address"] != null) dataGridView1.Columns["Address"].HeaderText = "آدرس";
                        if (dataGridView1.Columns["Description"] != null) dataGridView1.Columns["Description"].HeaderText = "توضیحات";
                        if (dataGridView1.Columns["ColorCode"] != null) dataGridView1.Columns["ColorCode"].HeaderText = "رنگ";
                        if (dataGridView1.Columns["IsActive"] != null) dataGridView1.Columns["IsActive"].HeaderText = "فعال";

                        // افزودن ستون شماره ردیف
                        if (!dataGridView1.Columns.Contains("RowNumber"))
                        {
                            DataGridViewTextBoxColumn colRow = new DataGridViewTextBoxColumn();
                            colRow.Name = "RowNumber";
                            colRow.HeaderText = "ردیف";
                            colRow.ReadOnly = true;
                            colRow.Width = 60;
                            colRow.FillWeight = 30; // تا عرض کمتری نسبت به بقیه بگیرد
                            dataGridView1.Columns.Insert(0, colRow);
                        }

                        UpdateRowNumbers();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در بارگذاری اطلاعات شرکت‌ها:\n" + ex.Message, "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // متد تولید و آپدیت شماره ردیف‌ها
        private void UpdateRowNumbers()
        {
            if (dataGridView1.Columns.Contains("RowNumber"))
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (!dataGridView1.Rows[i].IsNewRow)
                    {
                        dataGridView1.Rows[i].Cells["RowNumber"].Value = (i + 1).ToString();
                    }
                }
            }
        }

        // متد رنگی کردن ردیف‌ها بر اساس ستون ColorCode
        private void DataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // ۱. مقداردهی ستون شماره ردیف بلافاصله پس از تکمیل بارگذاری اطلاعات
            UpdateRowNumbers();

            // ۲. رنگی کردن پس‌زمینه ردیف‌ها
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                string colorName = row.Cells["ColorCode"].Value?.ToString();
                Color rowColor = GetColorFromPersianName(colorName);
                row.DefaultCellStyle.BackColor = rowColor;
            }
        }

        // متد کمکی برای تبدیل نام رنگ فارسی به رنگ سیستمی ملایم (این متد حتماً باید بماند)
        private Color GetColorFromPersianName(string colorName)
        {
            if (string.IsNullOrWhiteSpace(colorName)) return Color.White;

            switch (colorName.Trim())
            {
                case "قرمز": return Color.FromArgb(255, 200, 200);
                case "آبی": return Color.FromArgb(200, 220, 255);
                case "سبز": return Color.FromArgb(200, 255, 200);
                case "زرد": return Color.LightYellow;
                case "نارنجی": return Color.Moccasin;
                case "خاکستری": return Color.LightGray;
                case "سفید": return Color.White;
                default:
                    try { return Color.FromName(colorName); }
                    catch { return Color.White; }
            }
        }

        // راست‌کلیک روی سرستون‌ها (با استثنا کردن نام شرکت)
        private void DataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip menu = new ContextMenuStrip();
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    // آی‌دی، ردیف و نام شرکت در لیست مخفی‌سازی نمایش داده نشوند
                    if (col.Name == "CompanyID" || col.Name == "RowNumber" || col.Name == "CompanyName") continue;

                    ToolStripMenuItem item = new ToolStripMenuItem(col.HeaderText);
                    item.Checked = col.Visible;
                    item.CheckOnClick = true;
                    item.Click += (s, args) =>
                    {
                        col.Visible = item.Checked;
                    };
                    menu.Items.Add(item);
                }
                menu.Show(Cursor.Position);
            }
        }

        // =========================================================
        // رویدادهای دکمه‌های نوار ابزار
        // =========================================================

        public void btnAddCompany_Click(object sender, EventArgs e)
        {
            FrmAddCompany frmAdd = new FrmAddCompany();
            if (frmAdd.ShowDialog() == DialogResult.OK)
            {
                LoadCompanies();
            }
        }

        public void btnEditCompany_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0 && dataGridView1.SelectedCells.Count == 0)
            {
                MessageBox.Show("لطفاً ابتدا یک شرکت را از جدول برای ویرایش انتخاب کنید.", "اخطار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int companyId = 0;
            if (dataGridView1.SelectedRows.Count > 0)
            {
                companyId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["CompanyID"].Value);
            }
            else if (dataGridView1.SelectedCells.Count > 0)
            {
                int rowIndex = dataGridView1.SelectedCells[0].RowIndex;
                companyId = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["CompanyID"].Value);
            }

            if (companyId <= 0) return;

            FrmAddCompany frmEdit = new FrmAddCompany(companyId);
            if (frmEdit.ShowDialog() == DialogResult.OK)
            {
                LoadCompanies();
            }
        }

        public void btnSelect_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0 && dataGridView1.SelectedCells.Count == 0)
            {
                MessageBox.Show("لطفاً ابتدا یک شرکت را از جدول انتخاب کنید.", "اخطار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string companyName = "";
            if (dataGridView1.SelectedRows.Count > 0)
            {
                companyName = dataGridView1.SelectedRows[0].Cells["CompanyName"].Value?.ToString();
            }
            else if (dataGridView1.SelectedCells.Count > 0)
            {
                int rowIndex = dataGridView1.SelectedCells[0].RowIndex;
                companyName = dataGridView1.Rows[rowIndex].Cells["CompanyName"].Value?.ToString();
            }

            if (string.IsNullOrWhiteSpace(companyName)) return;

            if (Application.OpenForms["MainForm"] is MainForm mainForm)
            {
                mainForm.SetSelectedCompany(companyName);
            }

            CloseCurrentTab();
        }

        public void btnCancel_Click(object sender, EventArgs e)
        {
            CloseCurrentTab();
        }

        private void CloseCurrentTab()
        {
            if (this.Parent is TabPage tabPage && tabPage.Parent is TabControl tabControl)
            {
                tabControl.TabPages.Remove(tabPage);
            }
            else
            {
                this.Close();
            }
        }
    }
}
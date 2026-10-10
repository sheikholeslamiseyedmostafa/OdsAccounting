using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace OdsAccounting
{
    public partial class FrmChartOfAccounts : Form
    {
        private int _selectedId;

        public FrmChartOfAccounts()
        {
            InitializeComponent();
            Ui.ApplyFont(this);
            cmbLevel.Items.AddRange(new object[] { "1 - کل", "2 - کل فرعی", "3 - معین", "4 - تفصیلی" });
            cmbNature.Items.AddRange(new object[] { "بدهکار", "بستانکار" });
            cmbLevel.SelectedIndex = 0;
            cmbNature.SelectedIndex = 0;
            dgvAccounts.CellClick += (s, e) => { if (e.RowIndex >= 0) LoadRow(e.RowIndex); };
        }

        private void FrmChartOfAccounts_Load(object sender, EventArgs e)
        {
            if (!Session.HasCompany) { Ui.Warn("ابتدا شرکت را از منوی اطلاعات پایه انتخاب کنید."); return; }
            ApplyPermissions();
            RefreshGrid();
        }

        private void ApplyPermissions()
        {
            bool edit = Session.CanEdit;
            btnSave.Enabled = edit;
            btnDelete.Enabled = edit;
            btnSeed.Enabled = edit;
        }

        private void RefreshGrid()
        {
            DataTable t = AppDb.Query(@"
                SELECT AccountId, Code AS [کد حساب], Title AS [عنوان حساب], [Level] AS [سطح],
                       CASE Nature WHEN 1 THEN N'بدهکار' ELSE N'بستانکار' END AS [ماهیت],
                       CASE WHEN IsFloating = 1 THEN N'بله' ELSE N'خیر' END AS [حساب شناور],
                       CASE WHEN IsActive = 1 THEN N'فعال' ELSE N'غیرفعال' END AS [وضعیت]
                FROM ods.SC_Accounts WHERE CompanyID = @c ORDER BY Code",
                new SqlParameter("@c", Session.CompanyId));
            dgvAccounts.DataSource = t;
            if (dgvAccounts.Columns.Contains("AccountId")) dgvAccounts.Columns["AccountId"].Visible = false;
        }

        private void LoadRow(int rowIndex)
        {
            DataGridViewRow row = dgvAccounts.Rows[rowIndex];
            _selectedId = Convert.ToInt32(row.Cells["AccountId"].Value);
            DataTable t = AppDb.Query("SELECT * FROM ods.SC_Accounts WHERE AccountId = @id", new SqlParameter("@id", _selectedId));
            if (t.Rows.Count == 0) return;
            DataRow r = t.Rows[0];
            txtCode.Text = Conv.Str(r["Code"]);
            txtTitle.Text = Conv.Str(r["Title"]);
            cmbLevel.SelectedIndex = Math.Max(0, Conv.Int(r["Level"]) - 1);
            cmbNature.SelectedIndex = Conv.Int(r["Nature"]) == 2 ? 1 : 0;
            chkFloating.Checked = Conv.Bool(r["IsFloating"]);
            chkActive.Checked = Conv.Bool(r["IsActive"]);
            txtCode.ReadOnly = true;
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            _selectedId = 0;
            txtCode.Text = "";
            txtTitle.Text = "";
            cmbLevel.SelectedIndex = 0;
            cmbNature.SelectedIndex = 0;
            chkFloating.Checked = false;
            chkActive.Checked = true;
            txtCode.ReadOnly = false;
            txtCode.Focus();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!Session.CanEdit) { Ui.Warn("دسترسی ویرایش ندارید."); return; }
            string code = txtCode.Text.Trim();
            string title = txtTitle.Text.Trim();
            if (code.Length == 0 || title.Length == 0) { Ui.Warn("کد و عنوان حساب الزامی است."); return; }
            foreach (char ch in code)
                if (!char.IsDigit(ch)) { Ui.Warn("کد حساب باید فقط عدد باشد."); return; }

            int level = cmbLevel.SelectedIndex + 1;
            int nature = cmbNature.SelectedIndex + 1;
            var p = new[]
            {
                new SqlParameter("@c", Session.CompanyId),
                new SqlParameter("@code", code),
                new SqlParameter("@title", title),
                new SqlParameter("@lvl", (byte)level),
                new SqlParameter("@nat", (byte)nature),
                new SqlParameter("@fl", chkFloating.Checked),
                new SqlParameter("@act", chkActive.Checked),
                new SqlParameter("@id", _selectedId)
            };
            try
            {
                if (_selectedId == 0)
                {
                    AppDb.Exec(@"INSERT INTO ods.SC_Accounts (CompanyID, Code, Title, [Level], Nature, IsFloating, IsActive)
                                 VALUES (@c, @code, @title, @lvl, @nat, @fl, @act)", p);
                    Session.Audit("ACCOUNT_CREATE", code);
                }
                else
                {
                    AppDb.Exec(@"UPDATE ods.SC_Accounts SET Title = @title, [Level] = @lvl, Nature = @nat,
                                 IsFloating = @fl, IsActive = @act WHERE AccountId = @id", p);
                    Session.Audit("ACCOUNT_UPDATE", code);
                }
                RefreshGrid();
                Ui.Info("حساب با موفقیت ذخیره شد.");
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                Ui.Error("این کد حساب قبلاً ثبت شده است.");
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (!Session.CanEdit) { Ui.Warn("دسترسی حذف ندارید."); return; }
            if (_selectedId == 0) { Ui.Warn("ابتدا یک حساب را انتخاب کنید."); return; }
            int used = Conv.Int(AppDb.Scalar("SELECT COUNT(*) FROM ods.SC_VoucherLines WHERE AccountId = @id", new SqlParameter("@id", _selectedId)));
            if (used > 0) { Ui.Warn("این حساب در اسناد استفاده شده و قابل حذف نیست. می‌توانید آن را غیرفعال کنید."); return; }
            if (!Ui.Confirm("حساب انتخاب‌شده حذف شود؟")) return;
            AppDb.Exec("DELETE FROM ods.SC_Accounts WHERE AccountId = @id", new SqlParameter("@id", _selectedId));
            Session.Audit("ACCOUNT_DELETE", txtCode.Text);
            BtnNew_Click(sender, e);
            RefreshGrid();
        }

        private void BtnSeed_Click(object sender, EventArgs e)
        {
            AppDb.Exec("EXEC ods.SC_SeedChartOfAccounts @CompanyId", new SqlParameter("@CompanyId", Session.CompanyId));
            RefreshGrid();
            Ui.Info("سرفصل‌های پیش‌فرض (در صورت خالی بودن سرفصل‌ها) ایجاد شد.");
        }

        private void BtnRefresh_Click(object sender, EventArgs e) => RefreshGrid();
    }
}

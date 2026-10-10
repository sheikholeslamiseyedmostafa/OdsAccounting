using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace OdsAccounting
{
    /// <summary>مدیریت کاربران، نقش‌ها، کلمه عبور و مشاهده گزارش فعالیت‌ها (فقط مدیر).</summary>
    public partial class FrmUsers : Form
    {
        private static readonly string[] RoleKeys = { "Admin", "Accountant", "Viewer" };
        private static readonly string[] RoleTitles = { "مدیر سیستم", "حسابدار", "مشاهده‌گر" };
        private int _userId;
        private string _userName = "";

        public FrmUsers()
        {
            InitializeComponent();
            Ui.ApplyFont(this);
            dgvUsers.CellClick += (s, e) => { if (e.RowIndex >= 0) LoadUser(e.RowIndex); };
        }

        private void FrmUsers_Load(object sender, EventArgs e)
        {
            cmbRole.Items.AddRange(RoleTitles);
            cmbRole.SelectedIndex = 1;
            bool admin = Session.IsAdmin;
            btnNew.Enabled = admin; btnSave.Enabled = admin; btnResetPass.Enabled = admin;
            if (!admin) Ui.Info("فقط مدیر سیستم می‌تواند کاربران را مدیریت کند. نمایش در حالت فقط‌خواندنی است.");
            RefreshAll();
        }

        private void RefreshAll()
        {
            dgvUsers.DataSource = AppDb.Query(@"
                SELECT UserID, UserLoginName AS [نام کاربری], ISNULL(FullName, N'') AS [نام کامل],
                       CASE [Role] WHEN 'Admin' THEN N'مدیر سیستم' WHEN 'Accountant' THEN N'حسابدار' ELSE N'مشاهده‌گر' END AS [نقش],
                       CASE WHEN IsActive = 1 THEN N'فعال' ELSE N'غیرفعال' END AS [وضعیت]
                FROM ods.SC_Users ORDER BY UserLoginName");
            dgvUsers.Columns["UserID"].Visible = false;

            dgvAudit.DataSource = AppDb.Query(@"
                SELECT TOP (300) LogDate AS [زمان], UserName AS [کاربر], [Action] AS [عملیات], ISNULL(Details, N'') AS [جزئیات]
                FROM ods.SC_AuditLogs ORDER BY LogId DESC");
        }

        private void LoadUser(int index)
        {
            DataGridViewRow row = dgvUsers.Rows[index];
            _userId = Conv.Int(row.Cells["UserID"].Value);
            DataTable t = AppDb.Query("SELECT UserLoginName, ISNULL(FullName, N'') AS FullName, [Role], IsActive FROM ods.SC_Users WHERE UserID = @id",
                new SqlParameter("@id", _userId));
            if (t.Rows.Count == 0) return;
            DataRow r = t.Rows[0];
            _userName = Conv.Str(r["UserLoginName"]);
            txtUser.Text = _userName;
            txtUser.ReadOnly = true;
            txtFull.Text = Conv.Str(r["FullName"]);
            string role = Conv.Str(r["Role"]);
            int idx = Array.IndexOf(RoleKeys, role);
            cmbRole.SelectedIndex = idx < 0 ? 1 : idx;
            chkActive.Checked = Conv.Bool(r["IsActive"]);
            txtPass.Text = "";
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            _userId = 0;
            _userName = "";
            txtUser.Text = ""; txtUser.ReadOnly = false;
            txtFull.Text = ""; txtPass.Text = "";
            cmbRole.SelectedIndex = 1;
            chkActive.Checked = true;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!Session.IsAdmin) { Ui.Warn("فقط مدیر سیستم دسترسی ویرایش دارد."); return; }
            string role = RoleKeys[cmbRole.SelectedIndex < 0 ? 1 : cmbRole.SelectedIndex];
            try
            {
                if (_userId == 0)
                {
                    string name = txtUser.Text.Trim();
                    if (name.Length < 3) { Ui.Warn("نام کاربری حداقل ۳ حرف باشد."); return; }
                    if (txtPass.Text.Length < 6) { Ui.Warn("کلمه عبور حداقل ۶ کاراکتر باشد."); return; }
                    AppDb.Exec(@"INSERT INTO ods.SC_Users (UserLoginName, PasswordHash, FullName, [Role], IsActive)
                                 VALUES (@u, @h, @f, @r, @a)",
                        new SqlParameter("@u", name), new SqlParameter("@h", PasswordHasher.Hash(txtPass.Text)),
                        new SqlParameter("@f", txtFull.Text.Trim()), new SqlParameter("@r", role), new SqlParameter("@a", chkActive.Checked));
                    Session.Audit("USER_CREATE", name + " / " + role);
                }
                else
                {
                    if (_userName == Session.UserName && (!chkActive.Checked || role != "Admin"))
                    { Ui.Warn("نمی‌توانید دسترسی یا فعال بودن کاربر جاری را کم کنید."); return; }
                    AppDb.Exec("UPDATE ods.SC_Users SET FullName = @f, [Role] = @r, IsActive = @a WHERE UserID = @id",
                        new SqlParameter("@f", txtFull.Text.Trim()), new SqlParameter("@r", role),
                        new SqlParameter("@a", chkActive.Checked), new SqlParameter("@id", _userId));
                    Session.Audit("USER_UPDATE", _userName + " / " + role);
                }
                RefreshAll();
                Ui.Info("ذخیره شد.");
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                Ui.Error("نام کاربری تکراری است.");
            }
        }

        private void BtnResetPass_Click(object sender, EventArgs e)
        {
            if (!Session.IsAdmin) { Ui.Warn("فقط مدیر سیستم دسترسی دارد."); return; }
            if (_userId == 0) { Ui.Warn("کاربری را انتخاب کنید."); return; }
            if (txtPass.Text.Length < 6) { Ui.Warn("کلمه عبور جدید حداقل ۶ کاراکتر باشد."); return; }
            AppDb.Exec("UPDATE ods.SC_Users SET PasswordHash = @h, UserLoginPassword = N'' WHERE UserID = @id",
                new SqlParameter("@h", PasswordHasher.Hash(txtPass.Text)), new SqlParameter("@id", _userId));
            Session.Audit("USER_PASSWORD_RESET", _userName);
            txtPass.Text = "";
            Ui.Info("کلمه عبور تغییر کرد.");
        }

        private void BtnRefresh_Click(object sender, EventArgs e) => RefreshAll();
    }
}

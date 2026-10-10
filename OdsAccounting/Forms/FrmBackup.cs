using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace OdsAccounting
{
    /// <summary>پشتیبان‌گیری و بازیابی پایگاه داده SQL Server (BACKUP / RESTORE).</summary>
    public partial class FrmBackup : Form
    {
        public FrmBackup()
        {
            InitializeComponent();
            Ui.ApplyFont(this);
        }

        private string DatabaseName => new SqlConnectionStringBuilder(AppDb.ConnectionString).InitialCatalog;

        private void FrmBackup_Load(object sender, EventArgs e)
        {
            txtPath.Text = Path.Combine(@"C:\ODSBackup", $"{DatabaseName}_{DateTime.Now:yyyyMMdd_HHmm}.bak");
            bool admin = Session.IsAdmin;
            btnBackup.Enabled = admin; btnRestore.Enabled = admin;
            if (AppDb.IsCloud)
                lblNote.Text = "حالت ابری: پشتیبان‌گیری را از امکانات سرویس ابری (Automated backup) انجام دهید.";
            RefreshHistory();
        }

        private bool ValidPath(out string path)
        {
            path = txtPath.Text.Trim();
            if (!path.EndsWith(".bak", StringComparison.OrdinalIgnoreCase))
            { Ui.Warn("مسیر باید با پسوند .bak باشد."); return false; }
            return true;
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (var dlg = new SaveFileDialog { Filter = "SQL Backup (*.bak)|*.bak", FileName = Path.GetFileName(txtPath.Text) })
                if (dlg.ShowDialog() == DialogResult.OK) txtPath.Text = dlg.FileName;
        }

        private void BtnBackup_Click(object sender, EventArgs e)
        {
            if (AppDb.IsCloud) { Ui.Warn("در حالت ابری از پشتیبان‌گیری سرویس ابری استفاده کنید."); return; }
            if (!ValidPath(out string path)) return;
            try
            {
                // BACKUP روی سرور SQL اجرا می‌شود؛ مسیر باید از دید سرویس SQL Server قابل دسترسی باشد.
                string dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
                string sql = $@"BACKUP DATABASE [{DatabaseName}] TO DISK = N'{path.Replace("'", "''")}'
                                WITH INIT, COMPRESSION, CHECKSUM, STATS = 10, NAME = N'ODS Accounting full backup'";
                using (var conn = AppDb.Open())
                using (var cmd = new SqlCommand(sql, conn) { CommandTimeout = 0 })
                    cmd.ExecuteNonQuery();
                Session.Audit("BACKUP", path);
                RefreshHistory();
                Ui.Info("پشتیبان‌گیری با موفقیت انجام شد.\n" + path);
            }
            catch (Exception ex)
            {
                Ui.Error("پشتیبان‌گیری ناموفق بود:\n" + ex.Message);
            }
        }

        private void BtnRestore_Click(object sender, EventArgs e)
        {
            if (AppDb.IsCloud) { Ui.Warn("بازیابی در حالت ابری از پنل سرویس انجام می‌شود."); return; }
            if (!ValidPath(out string path)) return;
            if (!File.Exists(path)) { Ui.Warn("فایل پشتیبان یافت نشد. (توجه: مسیر باید روی سرور SQL باشد)"); return; }
            if (!Ui.Confirm("هشدار: تمام اطلاعات فعلی با نسخه پشتیبان جایگزین می‌شود و همه کاربران قطع خواهند شد.\nادامه می‌دهید؟", "بازیابی اطلاعات"))
                return;
            try
            {
                // بازیابی باید از اتصال master انجام شود تا پایگاه داده در حال استفاده نباشد
                var master = new SqlConnectionStringBuilder(AppDb.ConnectionString) { InitialCatalog = "master" };
                string db = DatabaseName;
                string sql = $@"ALTER DATABASE [{db}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                                RESTORE DATABASE [{db}] FROM DISK = N'{path.Replace("'", "''")}' WITH REPLACE, STATS = 10;
                                ALTER DATABASE [{db}] SET MULTI_USER;";
                using (var conn = new SqlConnection(master.ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(sql, conn) { CommandTimeout = 0 }) cmd.ExecuteNonQuery();
                }
                Session.Audit("RESTORE", path);
                Ui.Info("بازیابی انجام شد. برنامه را دوباره اجرا کنید.");
            }
            catch (Exception ex)
            {
                Ui.Error("بازیابی ناموفق بود:\n" + ex.Message);
            }
        }

        private void RefreshHistory()
        {
            try
            {
                DataTable t = AppDb.Query(@"
                    SELECT TOP (50) bs.backup_finish_date AS [تاریخ], CAST(bs.backup_size / 1048576.0 AS DECIMAL(12,2)) AS [حجم MB],
                           CASE bs.type WHEN 'D' THEN N'کامل' WHEN 'I' THEN N'افزایشی' ELSE N'لاگ' END AS [نوع],
                           bmf.physical_device_name AS [مسیر فایل]
                    FROM msdb.dbo.backupset bs
                    JOIN msdb.dbo.backupmediafamily bmf ON bmf.media_set_id = bs.media_set_id
                    WHERE bs.database_name = @db
                    ORDER BY bs.backup_finish_date DESC",
                    new SqlParameter("@db", DatabaseName));
                dgvHistory.DataSource = t;
            }
            catch (SqlException)
            {
                dgvHistory.DataSource = null; // بدون دسترسی به msdb
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e) => RefreshHistory();
    }
}

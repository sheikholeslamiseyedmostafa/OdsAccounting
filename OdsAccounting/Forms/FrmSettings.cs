using System;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using OdsAccounting.Properties;

namespace OdsAccounting
{
    /// <summary>تنظیمات اتصال (لوکال/ابری)، مودیان و ساختار پایگاه داده.</summary>
    public partial class FrmSettings : Form
    {
        public FrmSettings()
        {
            InitializeComponent();
            Ui.ApplyFont(this);
        }

        private void FrmSettings_Load(object sender, EventArgs e)
        {
            var s = Settings.Default;
            rbLocal.Checked = !AppDb.IsCloud;
            rbCloud.Checked = AppDb.IsCloud;
            txtLocalConn.Text = s.LocalConnectionString;
            txtCloudConn.Text = s.CloudConnectionString;
            txtEndpoint.Text = s.MoadianEndpoint;
            txtTaxId.Text = s.MoadianTaxId;
            txtThumb.Text = s.MoadianCertThumbprint;
            bool admin = Session.IsAdmin;
            btnSave.Enabled = admin; btnEnsure.Enabled = admin;
        }

        private string SelectedConnection => rbCloud.Checked ? txtCloudConn.Text.Trim() : txtLocalConn.Text.Trim();

        private void BtnTest_Click(object sender, EventArgs e)
        {
            try
            {
                using (var conn = new SqlConnection(SelectedConnection))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("SELECT CAST(SERVERPROPERTY('ProductMajorVersion') AS int), CAST(SERVERPROPERTY('ProductVersion') AS nvarchar(50))", conn))
                    using (var r = cmd.ExecuteReader())
                    {
                        r.Read();
                        int major = r.GetInt32(0);
                        string ver = r.GetString(1);
                        lblStatus.Text = $"اتصال برقرار است. نسخه SQL Server: {ver}";
                        if (major < 17) Ui.Warn("نسخه SQL Server کمتر از 2025 است. برخی امکانات (نوع json) ممکن است در دسترس نباشد.");
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "اتصال ناموفق";
                Ui.Error(ex.Message);
            }
        }

        private void Persist()
        {
            var s = Settings.Default;
            s.DbMode = rbCloud.Checked ? AppDb.CloudMode : AppDb.LocalMode;
            s.LocalConnectionString = txtLocalConn.Text.Trim();
            s.CloudConnectionString = txtCloudConn.Text.Trim();
            s.MoadianEndpoint = txtEndpoint.Text.Trim();
            s.MoadianTaxId = txtTaxId.Text.Trim();
            s.MoadianCertThumbprint = txtThumb.Text.Trim();
            s.Save();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!Session.IsAdmin) { Ui.Warn("فقط مدیر سیستم می‌تواند تنظیمات را تغییر دهد."); return; }
            Persist();
            Session.Audit("SETTINGS_SAVE", Settings.Default.DbMode);
            lblStatus.Text = "تنظیمات ذخیره شد. برای اعمال حالت جدید برنامه را دوباره اجرا کنید.";
        }

        private void BtnEnsure_Click(object sender, EventArgs e)
        {
            if (!Session.IsAdmin) { Ui.Warn("فقط مدیر سیستم."); return; }
            Persist();
            try
            {
                AppDb.EnsureSchema();
                lblStatus.Text = "ساختار پایگاه داده به‌روز شد.";
                Ui.Info("ساختار پایگاه داده (ODS) ایجاد/به‌روزرسانی شد.");
            }
            catch (Exception ex)
            {
                Ui.Error("خطا در ایجاد ساختار:\n" + ex.Message);
            }
        }
    }
}

using System;
using System.Data;
using Microsoft.Data.SqlClient;
using OdsAccounting.Properties;

namespace OdsAccounting
{
    /// <summary>اطلاعات کاربر و شرکت جاری (پس از ورود و انتخاب شرکت).</summary>
    public static class Session
    {
        public static int UserId;
        public static string UserName = "";
        public static string FullName = "";
        public static string Role = "Viewer";

        private static int? _companyId;

        public static bool IsLoggedIn => !string.IsNullOrEmpty(UserName);
        public static bool IsAdmin => Role == "Admin";
        public static bool CanEdit => Role == "Admin" || Role == "Accountant";

        /// <summary>شناسه شرکت انتخاب‌شده (بر اساس نام ذخیره‌شده در تنظیمات).</summary>
        public static int CompanyId
        {
            get
            {
                if (!_companyId.HasValue)
                {
                    _companyId = 0;
                    string name = Settings.Default.SelectedCompany?.Trim();
                    if (!string.IsNullOrEmpty(name))
                    {
                        var v = AppDb.Scalar("SELECT CompanyID FROM ods.SC_Companies WHERE CompanyName = @n",
                            new SqlParameter("@n", name));
                        if (v != null && v != DBNull.Value) _companyId = Convert.ToInt32(v);
                    }
                }
                return _companyId.Value;
            }
        }

        public static bool HasCompany => CompanyId > 0;

        public static void ResetCompany() => _companyId = null;

        public static void Audit(string action, string details = null)
        {
            try
            {
                AppDb.Exec("INSERT INTO ods.SC_AuditLogs (UserName, CompanyID, [Action], Details) VALUES (@u, @c, @a, @d)",
                    new SqlParameter("@u", UserName ?? ""),
                    new SqlParameter("@c", HasCompany ? (object)CompanyId : DBNull.Value),
                    new SqlParameter("@a", action),
                    new SqlParameter("@d", (object)details ?? DBNull.Value));
            }
            catch
            {
                // ثبت لاگ نباید مانع کار کاربر شود
            }
        }
    }
}

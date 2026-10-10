using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using OdsAccounting.Properties;

namespace OdsAccounting
{
    /// <summary>
    /// دسترسی یکپارچه به SQL Server 2025 در دو حالت «لوکال» و «ابری».
    /// </summary>
    public static class AppDb
    {
        public const string CloudMode = "Cloud";
        public const string LocalMode = "Local";

        public static bool IsCloud =>
            string.Equals(Settings.Default.DbMode, CloudMode, StringComparison.OrdinalIgnoreCase);

        public static string ModeTitle => IsCloud ? "ابری" : "لوکال (شبکه داخلی)";

        public const string FallbackLocalConnection =
            @"Server=SMSHEIKH\SQL25;Database=ODS_AccountingDB;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;";

        /// <summary>رشته اتصال فعال؛ اگر تنظیمات خالی باشد، مقدار پیش‌فرض لوکال استفاده می‌شود.</summary>
        public static string ConnectionString
        {
            get
            {
                string cs = IsCloud ? Settings.Default.CloudConnectionString : Settings.Default.LocalConnectionString;
                if (string.IsNullOrWhiteSpace(cs) || cs.Contains("YOUR-SERVER"))
                    cs = IsCloud ? cs : FallbackLocalConnection;
                if (string.IsNullOrWhiteSpace(cs))
                    throw new InvalidOperationException("رشته اتصال به پایگاه داده تنظیم نشده است. از بخش تنظیمات سیستم آن را وارد کنید.");
                return cs;
            }
        }

        public static SqlConnection Open()
        {
            var conn = new SqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        public static DataTable Query(string sql, params SqlParameter[] parameters)
        {
            using (var conn = Open())
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                var table = new DataTable();
                using (var adapter = new SqlDataAdapter(cmd)) adapter.Fill(table);
                return table;
            }
        }

        public static object Scalar(string sql, params SqlParameter[] parameters)
        {
            using (var conn = Open())
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteScalar();
            }
        }

        public static int Exec(string sql, params SqlParameter[] parameters)
        {
            using (var conn = Open())
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>اجرای چند دستور در یک تراکنش.</summary>
        public static void InTransaction(Action<SqlConnection, SqlTransaction> work)
        {
            using (var conn = Open())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    work(conn, tran);
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// ایجاد/به‌روزرسانی امن ساختار پایگاه داده. اسکریپت تعبیه‌شده با GO تقسیم و اجرا می‌شود.
        /// </summary>
        public static void EnsureSchema()
        {
            string script;
            var asm = Assembly.GetExecutingAssembly();
            using (var stream = asm.GetManifestResourceStream("OdsAccounting.schema.sql"))
            using (var reader = new StreamReader(stream))
                script = reader.ReadToEnd();

            var batches = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            using (var conn = Open())
            {
                foreach (var batch in batches)
                {
                    if (string.IsNullOrWhiteSpace(batch)) continue;
                    using (var cmd = new SqlCommand(batch, conn)) cmd.ExecuteNonQuery();
                }
            }

            // اگر هیچ کاربری وجود ندارد، کاربر مدیر اولیه ساخته می‌شود (رمز را بلافاصله تغییر دهید)
            if (Convert.ToInt32(Scalar("SELECT COUNT(*) FROM ods.SC_Users")) == 0)
            {
                Exec("INSERT INTO ods.SC_Users (UserLoginName, PasswordHash, FullName, [Role], IsActive) VALUES (N'admin', @h, N'مدیر سیستم', N'Admin', 1)",
                    new SqlParameter("@h", PasswordHasher.Hash("admin123")));
            }
        }
    }
}

using System;
using System.Windows.Forms;

namespace OdsAccounting
{
    internal static class Program
    {
        /// <summary>نقطه ورود برنامه. ساختار پایگاه داده پیش از نمایش ورود بررسی/به‌روزرسانی می‌شود.</summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            try
            {
                AppDb.EnsureSchema();
            }
            catch (Exception ex)
            {
                MessageBox.Show("اتصال به پایگاه داده برقرار نشد یا ساختار آن ایجاد نشد:\n\n" + ex.Message +
                                "\n\nبرنامه با رشته اتصال فعلی ادامه می‌دهد؛ می‌توانید آن را بعداً در تنظیمات اصلاح کنید.",
                                "هشدار پایگاه داده", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            Application.Run(new Form1());
        }
    }
}

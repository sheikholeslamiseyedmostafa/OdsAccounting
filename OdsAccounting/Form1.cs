using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace OdsAccounting
{
    public partial class Form1 : Form
    {
        // متغیرهای مربوط به جابجایی نرم فرم
        private bool isDragging = false;
        private Point startCursorPoint;
        private Point startFormLocation;

        public Form1()
        {
            InitializeComponent();
            Ui.ApplyFont(this);

            // ۱. تبدیل دکمه ورود به دکمه پیش‌فرض برای کلید Enter
            this.AcceptButton = button2;

            // ۲. اجرای کدهای بازیابی اطلاعات هنگام باز شدن فرم
            this.Load += Form1_Load;
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            if (Properties.Settings.Default.RemUser)
            {
                txtUsername.Text = Properties.Settings.Default.SavedUser;
                checkBox1.Checked = true;
            }

            if (Properties.Settings.Default.RemPass)
            {
                txtPassword.Text = Properties.Settings.Default.SavedPass;
                checkBox2.Checked = true;
            }
        }

        // دکمه X در نوار عنوان
        private void button1_Click(object sender, EventArgs e) => Application.Exit();

        // دکمه خروج در پایین فرم
        private void button3_Click(object sender, EventArgs e) => Application.Exit();

        private void label2_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }

        // دکمه ورود: احراز هویت با هش امن PBKDF2 (رمزهای قدیمی متنی در اولین ورود به هش تبدیل می‌شوند)
        private void button2_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            if (username.Length == 0 || password.Length == 0)
            {
                MessageBox.Show("لطفاً نام کاربری و کلمۀ عبور را وارد کنید.", "اخطار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataTable t = AppDb.Query(@"SELECT UserLoginName, UserLoginPassword, PasswordHash, ISNULL(FullName, N'') AS FullName, [Role], IsActive
                                            FROM ods.SC_Users WHERE UserLoginName = @u",
                    new SqlParameter("@u", username));

                if (t.Rows.Count == 0 || !Convert.ToBoolean(t.Rows[0]["IsActive"]))
                {
                    MessageBox.Show("نام کاربری یا کلمۀ عبور اشتباه است.", "خطا در ورود", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DataRow row = t.Rows[0];
                string hash = row["PasswordHash"] as string ?? "";
                string legacy = row["UserLoginPassword"] as string ?? "";
                bool ok;
                if (hash.Length > 0)
                {
                    ok = PasswordHasher.Verify(password, hash);
                }
                else
                {
                    ok = legacy.Length > 0 && legacy == password;
                    if (ok)
                    {
                        // مهاجرت: هش امن جایگزین رمز متنی قدیمی شود
                        AppDb.Exec("UPDATE ods.SC_Users SET PasswordHash = @h, UserLoginPassword = N'' WHERE UserLoginName = @u",
                            new SqlParameter("@h", PasswordHasher.Hash(password)),
                            new SqlParameter("@u", Convert.ToString(row["UserLoginName"])));
                    }
                }

                if (!ok)
                {
                    MessageBox.Show("نام کاربری یا کلمۀ عبور اشتباه است.", "خطا در ورود", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Session.UserName = Convert.ToString(row["UserLoginName"]);
                Session.FullName = Convert.ToString(row["FullName"]);
                Session.Role = Convert.ToString(row["Role"]);

                // مدیریت تنظیمات یادآوری
                Properties.Settings.Default.SavedUser = checkBox1.Checked ? username : "";
                Properties.Settings.Default.RemUser = checkBox1.Checked;
                Properties.Settings.Default.SavedPass = checkBox2.Checked ? password : "";
                Properties.Settings.Default.RemPass = checkBox2.Checked;
                Properties.Settings.Default.Save();

                Session.Audit("LOGIN");

                MainForm mainForm = new MainForm();
                mainForm.FormClosed += (s, args) => Application.Exit();
                mainForm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ارتباط با سرور پایگاه داده برقرار نشد:\n\n" + ex.Message +
                                "\n\nاز بخش تنظیمات (پس از ورود) یا فایل App.config رشته اتصال را بررسی کنید.",
                                "خطای ارتباطی", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // کدهای جابجایی نرم فرم
        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                startCursorPoint = Cursor.Position;
                startFormLocation = this.Location;
            }
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentScreenPos = Cursor.Position;
                this.Location = new Point(
                    startFormLocation.X + (currentScreenPos.X - startCursorPoint.X),
                    startFormLocation.Y + (currentScreenPos.Y - startCursorPoint.Y));
            }
        }

        private void label1_MouseUp(object sender, MouseEventArgs e) => isDragging = false;
    }
}

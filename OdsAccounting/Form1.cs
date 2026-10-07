using Microsoft.Data.SqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;

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

            // ۱. تبدیل دکمه ورود به دکمه پیش‌‌فرض برای کلید Enter
            this.AcceptButton = button2;

            // ۲. اجرای کدهای بازیابی اطلاعات هنگام باز شدن فرم
            this.Load += Form1_Load;
        }

        // خواندن اطلاعات ذخیره‌شده (با اصلاح علامت سؤال برای رفع هشدار دات‌نت)
        private void Form1_Load(object? sender, EventArgs e)
        {
            if (Properties.Settings.Default.RemUser)
            {
                txtUsername.Text = Properties.Settings.Default.SavedUser;
                checkBox1.Checked = true; // تیک یادآوری نام کاربری
            }

            if (Properties.Settings.Default.RemPass)
            {
                txtPassword.Text = Properties.Settings.Default.SavedPass;
                checkBox2.Checked = true; // تیک یادآوری کلمه عبور
            }
        }

        // دکمه X در نوار عنوان
        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // دکمه خروج در پایین فرم
        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label2_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }

        // دکمه ورود
        private void button2_Click(object sender, EventArgs e)
        {
            // بررسی خالی نبودن فیلدها
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("لطفاً نام کاربری و کلمۀ عبور را وارد کنید.", "اخطار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connectionString = "Server=SMSHEIKH\\SQL25;Database=ODS_AccountingDB;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM [ods].[SC_Users] WHERE UserLoginName = @user AND UserLoginPassword = @pass AND IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", txtUsername.Text);
                        cmd.Parameters.AddWithValue("@pass", txtPassword.Text);

                        int count = (int)cmd.ExecuteScalar();

                        if (count > 0)
                        {
                            // مدیریت تنظیمات یادآوری
                            if (checkBox1.Checked)
                            {
                                Properties.Settings.Default.SavedUser = txtUsername.Text;
                                Properties.Settings.Default.RemUser = true;
                            }
                            else
                            {
                                Properties.Settings.Default.SavedUser = "";
                                Properties.Settings.Default.RemUser = false;
                            }

                            if (checkBox2.Checked)
                            {
                                Properties.Settings.Default.SavedPass = txtPassword.Text;
                                Properties.Settings.Default.RemPass = true;
                            }
                            else
                            {
                                Properties.Settings.Default.SavedPass = "";
                                Properties.Settings.Default.RemPass = false;
                            }

                            // ذخیره نهایی تنظیمات
                            Properties.Settings.Default.Save();

                            // انتقال به محیط اصلی نرم‌افزار
                            MainForm mainForm = new MainForm();
                            mainForm.FormClosed += (s, args) => Application.Exit();
                            mainForm.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("نام کاربری یا کلمۀ عبور اشتباه است.", "خطا در ورود", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ارتباط با سرور پایگاه داده برقرار نشد:\n\n" + ex.Message, "خطای ارتباطی", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    startFormLocation.Y + (currentScreenPos.Y - startCursorPoint.Y)
                );
            }
        }

        private void label1_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }
    }
}
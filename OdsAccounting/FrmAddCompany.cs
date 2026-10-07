using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace OdsAccounting
{
    public partial class FrmAddCompany : Form
    {
        private int? companyId = null; // اگر مقدار داشته باشد یعنی در حالت ویرایش هستیم
        private string connectionString = "Server=SMSHEIKH\\SQL25;Database=ODS_AccountingDB;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;";

        // سازنده حالت ثبت جدید
        public FrmAddCompany()
        {
            InitializeComponent();
            this.Text = "افزودن شرکت جدید"; // عنوان فرم در حالت افزودن
            InitValidations();
            LoadExistingCompanyNames();
        }

        // سازنده حالت ویرایش
        public FrmAddCompany(int id)
        {
            InitializeComponent();
            this.Text = "ویرایش شرکت"; // عنوان فرم در حالت ویرایش
            companyId = id;
            InitValidations();
            LoadExistingCompanyNames();
            LoadCompanyDataForEdit();
        }

        // تنظیم محدودیت‌ها و رویدادهای کنترلی ورودی‌ها
        private void InitValidations()
        {
            // ۱. شناسه ملی: حداکثر ۱۱ رقم و فقط عدد
            txtNationalID.MaxLength = 11;
            txtNationalID.KeyPress += NumericOnly_KeyPress;

            // ۲. شماره اقتصادی، شماره ثبت و تلفن: فقط عدد
            txtEconomicCode.KeyPress += NumericOnly_KeyPress;
            txtRegistrationNo.KeyPress += NumericOnly_KeyPress;
            txtPhone.KeyPress += NumericOnly_KeyPress;
        }

        // متد مشترک برای جلوگیری از ورود حروف در فیلدهای عددی
        private void NumericOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // اگر کاراکتر غیرعدد باشد، تایپ نمی‌شود
            }
        }

        // بارگذاری نام شرکت‌های موجود برای پیشنهاد خودکار (Autocomplete)
        private void LoadExistingCompanyNames()
        {
            try
            {
                AutoCompleteStringCollection autoSource = new AutoCompleteStringCollection();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT CompanyName FROM [ods].[SC_Companies]";
                    if (companyId.HasValue)
                    {
                        query += " WHERE CompanyID != @CurrentId"; // در حالت ویرایش، نام خودش مستثنی شود
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (companyId.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@CurrentId", companyId.Value);
                        }

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string name = reader["CompanyName"]?.ToString();
                                if (!string.IsNullOrEmpty(name))
                                {
                                    autoSource.Add(name);
                                }
                            }
                        }
                    }
                }

                // تنظیمات AutoComplete برای پیشنهاد متن به صورت SuggestAppend
                txtCompanyName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                txtCompanyName.AutoCompleteSource = AutoCompleteSource.CustomSource;
                txtCompanyName.AutoCompleteCustomSource = autoSource;
            }
            catch
            {
                // خطاهای احتمالی خواندن دیتابیس در این بخش نادیده گرفته می‌شوند تا فرم کرش نکند
            }
        }

        // بازخوانی اطلاعات برای ویرایش
        private void LoadCompanyDataForEdit()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM [ods].[SC_Companies] WHERE CompanyID = @Id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", companyId.Value);
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtCompanyName.Text = reader["CompanyName"]?.ToString() ?? "";
                                txtNationalID.Text = reader["NationalID"]?.ToString() ?? "";
                                txtEconomicCode.Text = reader["EconomicCode"]?.ToString() ?? "";
                                txtRegistrationNo.Text = reader["RegistrationNo"]?.ToString() ?? "";
                                txtPhone.Text = reader["Phone"]?.ToString() ?? "";
                                txtAddress.Text = reader["Address"]?.ToString() ?? "";
                                txtDescription.Text = reader["Description"]?.ToString() ?? "";

                                string color = reader["ColorCode"]?.ToString();
                                if (!string.IsNullOrEmpty(color))
                                    cmbColor.SelectedItem = color;

                                chkIsActive.Checked = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در بارگذاری اطلاعات شرکت:\n" + ex.Message, "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string companyName = txtCompanyName.Text.Trim();
            string nationalID = txtNationalID.Text.Trim();
            string economicCode = txtEconomicCode.Text.Trim();
            string registrationNo = txtRegistrationNo.Text.Trim();

            // ۱. بررسی خالی نبودن نام شرکت
            if (string.IsNullOrWhiteSpace(companyName))
            {
                MessageBox.Show("لطفاً نام شرکت را وارد کنید.", "اخطار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCompanyName.Focus();
                return;
            }

            // ۲. بررسی طول شناسه ملی
            if (!string.IsNullOrEmpty(nationalID) && nationalID.Length > 11)
            {
                MessageBox.Show("شناسه ملی نمی‌تواند بیشتر از ۱۱ رقم باشد.", "اخطار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNationalID.Focus();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // ۳. کنترل نام تکراری (نام شرکت به هیچ وجه نباید تکراری باشد - مسدود کردن قطعی)
                    string checkNameQuery = "SELECT CompanyName FROM [ods].[SC_Companies] WHERE CompanyName = @Name";
                    if (companyId.HasValue) checkNameQuery += " AND CompanyID != @Id";

                    using (SqlCommand cmdName = new SqlCommand(checkNameQuery, conn))
                    {
                        cmdName.Parameters.AddWithValue("@Name", companyName);
                        if (companyId.HasValue) cmdName.Parameters.AddWithValue("@Id", companyId.Value);

                        object result = cmdName.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            MessageBox.Show($"شرکتی با نام «{companyName}» از قبل در سیستم ثبت شده است. نام شرکت نمی‌تواند تکراری باشد.", "خطا در نام شرکت", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtCompanyName.Focus();
                            return;
                        }
                    }

                    // ۴. کنترل شناسه ملی تکراری (همراه با ذکر نام شرکت و استثناء کردن رکورد خودش در حالت ویرایش)
                    if (!string.IsNullOrEmpty(nationalID))
                    {
                        string checkNatQuery = "SELECT CompanyName FROM [ods].[SC_Companies] WHERE NationalID = @NatID";
                        if (companyId.HasValue) checkNatQuery += " AND CompanyID != @Id";

                        using (SqlCommand cmdNat = new SqlCommand(checkNatQuery, conn))
                        {
                            cmdNat.Parameters.AddWithValue("@NatID", nationalID);
                            if (companyId.HasValue) cmdNat.Parameters.AddWithValue("@Id", companyId.Value);

                            object existingCompany = cmdNat.ExecuteScalar();
                            if (existingCompany != null && existingCompany != DBNull.Value)
                            {
                                string dupCompanyName = existingCompany.ToString();
                                DialogResult dialogResult = MessageBox.Show($"شناسه ملی وارد شده متعلق به شرکت «{dupCompanyName}» است و تکراری می‌باشد.\nآیا مایل به ثبت اطلاعات با شناسه ملی تکراری هستید؟", "هشدار تکراری بودن شناسه ملی", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (dialogResult == DialogResult.No)
                                {
                                    txtNationalID.Focus();
                                    return;
                                }
                            }
                        }
                    }

                    // ۵. کنترل شماره اقتصادی تکراری (همراه با ذکر نام شرکت و استثناء کردن رکورد خودش در حالت ویرایش)
                    if (!string.IsNullOrEmpty(economicCode))
                    {
                        string checkEcoQuery = "SELECT CompanyName FROM [ods].[SC_Companies] WHERE EconomicCode = @EcoCode";
                        if (companyId.HasValue) checkEcoQuery += " AND CompanyID != @Id";

                        using (SqlCommand cmdEco = new SqlCommand(checkEcoQuery, conn))
                        {
                            cmdEco.Parameters.AddWithValue("@EcoCode", economicCode);
                            if (companyId.HasValue) cmdEco.Parameters.AddWithValue("@Id", companyId.Value);

                            object existingCompany = cmdEco.ExecuteScalar();
                            if (existingCompany != null && existingCompany != DBNull.Value)
                            {
                                string dupCompanyName = existingCompany.ToString();
                                DialogResult dialogResult = MessageBox.Show($"شماره اقتصادی وارد شده متعلق به شرکت «{dupCompanyName}» است و تکراری می‌باشد.\nآیا مایل به ثبت اطلاعات با شماره اقتصادی تکراری هستید؟", "هشدار تکراری بودن شماره اقتصادی", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (dialogResult == DialogResult.No)
                                {
                                    txtEconomicCode.Focus();
                                    return;
                                }
                            }
                        }
                    }

                    // ۶. کنترل شماره ثبت تکراری (همراه با ذکر نام شرکت و استثناء کردن رکورد خودش در حالت ویرایش)
                    if (!string.IsNullOrEmpty(registrationNo))
                    {
                        string checkRegQuery = "SELECT CompanyName FROM [ods].[SC_Companies] WHERE RegistrationNo = @RegNo";
                        if (companyId.HasValue) checkRegQuery += " AND CompanyID != @Id";

                        using (SqlCommand cmdReg = new SqlCommand(checkRegQuery, conn))
                        {
                            cmdReg.Parameters.AddWithValue("@RegNo", registrationNo);
                            if (companyId.HasValue) cmdReg.Parameters.AddWithValue("@Id", companyId.Value);

                            object existingCompany = cmdReg.ExecuteScalar();
                            if (existingCompany != null && existingCompany != DBNull.Value)
                            {
                                string dupCompanyName = existingCompany.ToString();
                                DialogResult dialogResult = MessageBox.Show($"شماره ثبت وارد شده متعلق به شرکت «{dupCompanyName}» است و تکراری می‌باشد.\nآیا مایل به ثبت اطلاعات با شماره ثبت تکراری هستید؟", "هشدار تکراری بودن شماره ثبت", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (dialogResult == DialogResult.No)
                                {
                                    txtRegistrationNo.Focus();
                                    return;
                                }
                            }
                        }
                    }

                    // ۷. دستورات ثبت (INSERT) یا ویرایش (UPDATE)
                    string query = "";
                    if (companyId == null)
                    {
                        query = @"INSERT INTO [ods].[SC_Companies] 
                        (CompanyName, NationalID, EconomicCode, RegistrationNo, Phone, Address, Description, ColorCode, IsActive) 
                        VALUES 
                        (@Name, @NatID, @EcoCode, @RegNo, @Phone, @Address, @Desc, @Color, @IsActive)";
                    }
                    else
                    {
                        query = @"UPDATE [ods].[SC_Companies] SET 
                        CompanyName = @Name, 
                        NationalID = @NatID, 
                        EconomicCode = @EcoCode, 
                        RegistrationNo = @RegNo, 
                        Phone = @Phone, 
                        Address = @Address, 
                        Description = @Desc, 
                        ColorCode = @Color, 
                        IsActive = @IsActive 
                        WHERE CompanyID = @Id";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", companyName);
                        cmd.Parameters.AddWithValue("@NatID", nationalID);
                        cmd.Parameters.AddWithValue("@EcoCode", economicCode);
                        cmd.Parameters.AddWithValue("@RegNo", registrationNo);
                        cmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());
                        cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
                        cmd.Parameters.AddWithValue("@Desc", txtDescription.Text.Trim());

                        string selectedColor = cmbColor.SelectedItem != null ? cmbColor.SelectedItem.ToString() : "";
                        cmd.Parameters.AddWithValue("@Color", selectedColor);
                        cmd.Parameters.AddWithValue("@IsActive", chkIsActive.Checked);

                        if (companyId != null)
                        {
                            cmd.Parameters.AddWithValue("@Id", companyId.Value);
                        }

                        cmd.ExecuteNonQuery();
                    }
                }

                string successMessage = companyId == null ? "شرکت جدید با موفقیت ثبت شد." : "تغییرات شرکت با موفقیت ذخیره شد.";
                MessageBox.Show(successMessage, "تایید", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در ذخیره اطلاعات:\n" + ex.Message, "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
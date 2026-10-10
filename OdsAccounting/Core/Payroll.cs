using System;

namespace OdsAccounting
{
    /// <summary>
    /// محاسبات حقوق. نرخ‌ها نمونه‌اند و باید مطابق قوانین جاری (بیمه تامین اجتماعی و جدول مالیات حقوق) تنظیم شوند.
    /// </summary>
    public static class PayrollMath
    {
        public const decimal EmployeeInsuranceRate = 0.07m;   // سهم کارمند
        public const decimal EmployerInsuranceRate = 0.20m;   // سهم کارفرما
        public const decimal MonthlyTaxFreeLimit = 0m;        // معاف مالیاتی ماهانه (نمونه)
        public const decimal SampleTaxRate = 0.10m;           // نرخ مالیات نمونه

        public static decimal EmployeeInsurance(decimal insurableBase) => Math.Round(insurableBase * EmployeeInsuranceRate, 0);
        public static decimal EmployerInsurance(decimal insurableBase) => Math.Round(insurableBase * EmployerInsuranceRate, 0);

        public static decimal IncomeTax(decimal taxableIncome)
        {
            decimal taxable = taxableIncome - MonthlyTaxFreeLimit;
            return taxable <= 0 ? 0 : Math.Round(taxable * SampleTaxRate, 0);
        }
    }
}

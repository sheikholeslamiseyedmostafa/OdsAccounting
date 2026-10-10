# نرم‌افزار حسابداری ODS (OdsAccounting)

نرم‌افزار حسابداری فارسی تحت ویندوز (WinForms / C# / .NET 10) با پایگاه داده **SQL Server 2025**.
قابل استفاده در **حالت لوکال (شبکه داخلی)** و **حالت ابری** (اتصال به SQL Server ابری/Azure SQL از طریق رشته اتصال).

## پیش‌نیازها
- Visual Studio 2022/2026 با workload «Desktop development with C#» (.NET 10 SDK)
- SQL Server 2025 (Developer/Express/Standard) — لوکال یا ابری
- فونت **B Nazanin** نصب‌شده روی ویندوز (در نبود آن، Tahoma جایگزین می‌شود)

## راه‌اندازی
1. فایل `OdsAccounting.slnx` را در Visual Studio باز کنید.
2. در SQL Server (SSMS/sqlcmd) فایل `OdsAccounting/Database/01_CreateDatabase.sql` را یک‌بار اجرا کنید (ساخت پایگاه داده `ODS_AccountingDB`).
3. رشته اتصال را در `Properties/Settings.settings` یا بعد از ورود از بخش **تنظیمات سیستم** تنظیم کنید:
   - `LocalConnectionString` برای حالت لوکال، مثال: `Server=.\SQL2025;Database=ODS_AccountingDB;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;`
   - `CloudConnectionString` برای حالت ابری، مثال: `Server=tcp:<server>.database.windows.net,1433;Database=ODS_AccountingDB;User ID=...;Password=...;Encrypt=True;`
   - `DbMode` را `Local` یا `Cloud` بگذارید.
4. برنامه را اجرا کنید. هنگام اولین اجرا ساختار جداول (`ods.*`) به‌صورت امن ساخته می‌شود و کاربر اولیه **admin / admin123** ایجاد می‌شود. **بلافاصله رمز را تغییر دهید.**

## امکانات و محل پیاده‌سازی
| قابلیت | فرم / فایل |
|---|---|
| چندشرکته (ایجاد، ویرایش، انتخاب شرکت و سال مالی) | `FrmSelectCompany`، `FrmAddCompany`، `FrmSelectFinancialPeriod` |
| سرفصل حساب‌ها (سطح ۱ تا ۴، ماهیت، حساب شناور) | `Forms/FrmChartOfAccounts` |
| حساب شناور (مشتری، تامین‌کننده، بانک، پرسنل، سایر) | `Forms/FrmDimensions` |
| مرکز هزینه و پروژه | `Forms/FrmDimensions` (جدول `ods.SC_CostCenters`، نوع Kind) |
| اسناد حسابداری با کنترل تراز و ارسال به گردش کار | `Forms/FrmJournal` |
| فاکتور فروش/خرید/برگشت و ارسال به سامانه مودیان | `Forms/FrmInvoice`، `Core/Moadian.cs` |
| گزارش‌ها و انواع تراز (سطح، مرکز هزینه، پروژه، شناور) و کاردکس، خروجی CSV | `Forms/FrmTrialBalance` |
| حقوق و دستمزد (پرسنل، بیمه، مالیات، محاسبه ماهانه، سند حقوق) | `Forms/FrmPayroll`، `Core/Payroll.cs` |
| پشتیبان‌گیری و بازیابی (BACKUP / RESTORE) و تاریخچه | `Forms/FrmBackup` |
| گردش کار تاییدیه چندمرحله‌ای (سند، فاکتور، حقوق) | `Forms/FrmWorkflow` |
| ایجاد کاربر، نقش (مدیر/حسابدار/مشاهده‌گر)، امنیت و لاگ فعالیت | `Forms/FrmUsers`، `Core/Security.cs`، `Core/Session.cs` |
| هوش مصنوعی (دستیار گفتگو) | `FrmAI` |
| میز کار (شاخص‌های کلیدی) | `Forms/FrmDashboard` |
| منوهای کناری با آیکن | `Forms/MainForm` |

### امنیت
- کلمه عبور با **PBKDF2-SHA256 (100,000 تکرار) و نمک تصادفی** ذخیره می‌شود؛ رمزهای قدیمی متنی در اولین ورود به هش تبدیل می‌شوند.
- دسترسی‌ها بر اساس نقش: `Admin` (همه‌چیز)، `Accountant` (ثبت و تایید طبق گردش کار)، `Viewer` (فقط مشاهده).
- تمام عملیات مهم در `ods.SC_AuditLogs` ثبت می‌شود.
- در شبکه داخلی، SQL Server را روی سرور مرکزی با دسترسی محدود قرار دهید؛ برای حالت ابری از `Encrypt=True` استفاده کنید.

## قابلیت ویرایش در Visual Studio
- همه فرم‌ها دارای فایل `Designer.cs` استاندارد هستند؛ کنترل‌ها را در دیزاینر ویرایش کنید.
- فونت برنامه یکسان است: **B Nazanin، بولد، اندازه ۱۰ تا ۱۲** (تابع `Ui.ApplyFont`).

## محدودیت‌ها و نکات مهم (لطفاً بخوانید)
- **سامانه مودیان:** ساختار بسته فاکتور، ثبت لاگ و وضعیت فاکتور پیاده‌سازی شده است. ارسال واقعی نیازمند **گواهی امضای دیجیتال**، **شناسه مؤدی/حافظه مالیاتی** و طبق مستندات جاری سازمان امور مالیاتی است؛ فیلدها و امضای بسته (JWS) را با مستندات رسمی تطبیق دهید. آدرس API در تنظیمات قابل تغییر است.
- **نرخ‌های حقوق:** نرخ بیمه (۷٪ کارمند / ۲۰٪ کارفرما) و مالیات نمونه (۱۰٪ بدون معافیت) برای نمایش است؛ مطابق قوانین سال جاری در `Core/Payroll.cs` تنظیم کنید.
- **پشتیبان‌گیری:** مسیر فایل `.bak` باید از دید سرویس SQL Server قابل دسترسی باشد. در حالت ابری از پشتیبان‌گیری خودکار سرویس ابری استفاده کنید.
- این نسخه در محیط توسعه‌ای بدون Visual Studio/.NET SDK **کامپایل نشده است**؛ ابتدا پروژه را Build کنید و خطاهای احتمالی را گزارش دهید.
- اسکریپت `Database/schema.sql` از SQL Server 2025 (نوع داده `json`) استفاده می‌کند.

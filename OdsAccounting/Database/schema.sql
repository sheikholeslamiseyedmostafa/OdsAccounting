/* ==========================================================================
   ODS Accounting - SQL Server 2025 schema (idempotent)
   این اسکریپت توسط برنامه هنگام اجرا به صورت امن (بدون تخریب داده) اجرا می‌شود.
   ========================================================================== */
SET NOCOUNT ON;
GO
IF SCHEMA_ID(N'ods') IS NULL EXEC(N'CREATE SCHEMA ods');
GO
/* ---------- شرکت‌ها و سال‌های مالی (سازگار با نسخه قبلی) ---------- */
IF OBJECT_ID(N'ods.SC_Companies') IS NULL
CREATE TABLE ods.SC_Companies (
    CompanyID      INT IDENTITY(1,1) PRIMARY KEY,
    CompanyName    NVARCHAR(200) NOT NULL,
    NationalID     NVARCHAR(20)  NULL,
    EconomicCode   NVARCHAR(30)  NULL,
    RegistrationNo NVARCHAR(30)  NULL,
    Phone          NVARCHAR(30)  NULL,
    Address        NVARCHAR(500) NULL,
    Description    NVARCHAR(500) NULL,
    ColorCode      NVARCHAR(20)  NULL,
    IsActive       BIT NOT NULL CONSTRAINT DF_SC_Companies_Active DEFAULT 1
);
GO
IF OBJECT_ID(N'ods.SC_FinancialPeriods') IS NULL
CREATE TABLE ods.SC_FinancialPeriods (
    PeriodID   INT IDENTITY(1,1) PRIMARY KEY,
    CompanyID  INT NOT NULL REFERENCES ods.SC_Companies(CompanyID),
    PeriodName NVARCHAR(50) NOT NULL,
    StartDate  DATE NOT NULL,
    EndDate    DATE NOT NULL,
    IsClosed   BIT NOT NULL DEFAULT 0
);
GO
/* ---------- کاربران و امنیت ---------- */
IF OBJECT_ID(N'ods.SC_Users') IS NULL
CREATE TABLE ods.SC_Users (
    UserID            INT IDENTITY(1,1) PRIMARY KEY,
    UserLoginName     NVARCHAR(100) NOT NULL UNIQUE,
    UserLoginPassword NVARCHAR(200) NULL,
    IsActive          BIT NOT NULL DEFAULT 1
);
GO
IF COL_LENGTH(N'ods.SC_Users', N'PasswordHash') IS NULL ALTER TABLE ods.SC_Users ADD PasswordHash NVARCHAR(300) NULL;
IF COL_LENGTH(N'ods.SC_Users', N'FullName')     IS NULL ALTER TABLE ods.SC_Users ADD FullName NVARCHAR(150) NULL;
IF COL_LENGTH(N'ods.SC_Users', N'Role')         IS NULL ALTER TABLE ods.SC_Users ADD [Role] NVARCHAR(30) NOT NULL CONSTRAINT DF_SC_Users_Role DEFAULT N'Accountant';
IF COL_LENGTH(N'ods.SC_Users', N'CreatedAt')    IS NULL ALTER TABLE ods.SC_Users ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_SC_Users_Created DEFAULT SYSDATETIME();
GO
IF OBJECT_ID(N'ods.SC_AuditLogs') IS NULL
CREATE TABLE ods.SC_AuditLogs (
    LogId     BIGINT IDENTITY(1,1) PRIMARY KEY,
    LogDate   DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    UserName  NVARCHAR(100) NOT NULL,
    CompanyID INT NULL,
    [Action]  NVARCHAR(100) NOT NULL,
    Details   NVARCHAR(MAX) NULL
);
GO
IF OBJECT_ID(N'ods.SC_AIChatLogs') IS NULL
CREATE TABLE ods.SC_AIChatLogs (
    LogId          INT IDENTITY(1,1) PRIMARY KEY,
    UserMessage    NVARCHAR(MAX) NULL,
    SystemResponse NVARCHAR(MAX) NULL,
    LogDate        DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);
GO
/* ---------- حساب‌های سرفصل و حساب شناور ---------- */
IF OBJECT_ID(N'ods.SC_Accounts') IS NULL
CREATE TABLE ods.SC_Accounts (
    AccountId  INT IDENTITY(1,1) PRIMARY KEY,
    CompanyID  INT NOT NULL REFERENCES ods.SC_Companies(CompanyID),
    Code       NVARCHAR(20)  NOT NULL,
    Title      NVARCHAR(200) NOT NULL,
    [Level]    TINYINT NOT NULL,               -- 1 کل، 2 کل فرعی، 3 معین، 4 تفصیلی
    Nature     TINYINT NOT NULL DEFAULT 1,     -- 1 بدهکار، 2 بستانکار
    IsFloating BIT NOT NULL DEFAULT 0,         -- آیا این حساب از حساب شناور استفاده می‌کند
    IsActive   BIT NOT NULL DEFAULT 1,
    CONSTRAINT UQ_SC_Accounts UNIQUE (CompanyID, Code)
);
GO
IF OBJECT_ID(N'ods.SC_FloatingEntities') IS NULL
CREATE TABLE ods.SC_FloatingEntities (
    EntityId     INT IDENTITY(1,1) PRIMARY KEY,
    CompanyID    INT NOT NULL REFERENCES ods.SC_Companies(CompanyID),
    Code         NVARCHAR(30)  NOT NULL,
    Name         NVARCHAR(200) NOT NULL,
    EntityType   NVARCHAR(40)  NOT NULL,        -- مشتری، تامین‌کننده، بانک، پرسنل، سایر
    NationalID   NVARCHAR(20)  NULL,
    EconomicCode NVARCHAR(30)  NULL,
    IsActive     BIT NOT NULL DEFAULT 1,
    CONSTRAINT UQ_SC_FloatingEntities UNIQUE (CompanyID, Code)
);
GO
/* ---------- مرکز هزینه (Kind=1) و پروژه (Kind=2) ---------- */
IF OBJECT_ID(N'ods.SC_CostCenters') IS NULL
CREATE TABLE ods.SC_CostCenters (
    CostCenterId INT IDENTITY(1,1) PRIMARY KEY,
    CompanyID    INT NOT NULL REFERENCES ods.SC_Companies(CompanyID),
    Kind         TINYINT NOT NULL,              -- 1 مرکز هزینه، 2 پروژه
    Code         NVARCHAR(30)  NOT NULL,
    Title        NVARCHAR(200) NOT NULL,
    ParentId     INT NULL REFERENCES ods.SC_CostCenters(CostCenterId),
    IsActive     BIT NOT NULL DEFAULT 1,
    CONSTRAINT UQ_SC_CostCenters UNIQUE (CompanyID, Kind, Code)
);
GO
/* ---------- اسناد حسابداری ---------- */
IF OBJECT_ID(N'ods.SC_Vouchers') IS NULL
CREATE TABLE ods.SC_Vouchers (
    VoucherId   INT IDENTITY(1,1) PRIMARY KEY,
    CompanyID   INT NOT NULL REFERENCES ods.SC_Companies(CompanyID),
    VoucherNo   INT NOT NULL,
    VoucherDate DATE NOT NULL,
    Description NVARCHAR(500) NULL,
    Status      TINYINT NOT NULL DEFAULT 0,    -- 0 پیش‌نویس، 1 در انتظار تایید، 2 قطعی، 3 رد شده
    CreatedBy   NVARCHAR(100) NULL,
    CreatedAt   DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    PostedAt    DATETIME2 NULL,
    CONSTRAINT UQ_SC_Vouchers UNIQUE (CompanyID, VoucherNo)
);
GO
IF OBJECT_ID(N'ods.SC_VoucherLines') IS NULL
CREATE TABLE ods.SC_VoucherLines (
    LineId       BIGINT IDENTITY(1,1) PRIMARY KEY,
    VoucherId    INT NOT NULL REFERENCES ods.SC_Vouchers(VoucherId) ON DELETE CASCADE,
    [LineNo]       INT NOT NULL,
    AccountId    INT NOT NULL REFERENCES ods.SC_Accounts(AccountId),
    EntityId     INT NULL REFERENCES ods.SC_FloatingEntities(EntityId),
    CostCenterId INT NULL REFERENCES ods.SC_CostCenters(CostCenterId),
    ProjectId    INT NULL REFERENCES ods.SC_CostCenters(CostCenterId),
    Description  NVARCHAR(300) NULL,
    Debit        DECIMAL(19,0) NOT NULL DEFAULT 0,
    Credit       DECIMAL(19,0) NOT NULL DEFAULT 0,
    CONSTRAINT CK_SC_VoucherLines_OneSide CHECK (Debit >= 0 AND Credit >= 0 AND (Debit = 0 OR Credit = 0))
);
GO
/* ---------- فاکتور و ارسال به سامانه مودیان ---------- */
IF OBJECT_ID(N'ods.SC_Invoices') IS NULL
CREATE TABLE ods.SC_Invoices (
    InvoiceId          INT IDENTITY(1,1) PRIMARY KEY,
    CompanyID          INT NOT NULL REFERENCES ods.SC_Companies(CompanyID),
    InvoiceNo          INT NOT NULL,
    InvoiceDate        DATE NOT NULL,
    InvoiceType        TINYINT NOT NULL DEFAULT 1, -- 1 فروش، 2 خرید، 3 برگشت از فروش
    EntityId           INT NULL REFERENCES ods.SC_FloatingEntities(EntityId),
    SubTotal           DECIMAL(19,0) NOT NULL DEFAULT 0,
    Discount           DECIMAL(19,0) NOT NULL DEFAULT 0,
    VatAmount          DECIMAL(19,0) NOT NULL DEFAULT 0,
    TotalAmount        DECIMAL(19,0) NOT NULL DEFAULT 0,
    Status             TINYINT NOT NULL DEFAULT 0, -- 0 پیش‌نویس، 1 ارسال‌شده، 2 تایید، 3 رد، 4 خطا
    MoadianUid         NVARCHAR(60)  NULL,
    MoadianReferenceNo NVARCHAR(100) NULL,
    MoadianStatus      NVARCHAR(50)  NULL,
    CreatedAt          DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT UQ_SC_Invoices UNIQUE (CompanyID, InvoiceNo)
);
GO
IF OBJECT_ID(N'ods.SC_InvoiceLines') IS NULL
CREATE TABLE ods.SC_InvoiceLines (
    LineId     INT IDENTITY(1,1) PRIMARY KEY,
    InvoiceId  INT NOT NULL REFERENCES ods.SC_Invoices(InvoiceId) ON DELETE CASCADE,
    [LineNo]     INT NOT NULL,
    ItemCode   NVARCHAR(50)  NULL,
    ItemName   NVARCHAR(200) NOT NULL,
    Unit       NVARCHAR(30)  NULL,
    Quantity   DECIMAL(19,3) NOT NULL DEFAULT 1,
    UnitPrice  DECIMAL(19,0) NOT NULL DEFAULT 0,
    Discount   DECIMAL(19,0) NOT NULL DEFAULT 0,
    VatRate    DECIMAL(5,2)  NOT NULL DEFAULT 10,
    VatAmount  DECIMAL(19,0) NOT NULL DEFAULT 0,
    LineTotal  DECIMAL(19,0) NOT NULL DEFAULT 0
);
GO
IF OBJECT_ID(N'ods.SC_MoadianLogs') IS NULL
CREATE TABLE ods.SC_MoadianLogs (
    LogId        BIGINT IDENTITY(1,1) PRIMARY KEY,
    InvoiceId    INT NOT NULL REFERENCES ods.SC_Invoices(InvoiceId) ON DELETE CASCADE,
    SentAt       DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    RequestJson  JSON NULL,                       -- نوع داده json در SQL Server 2025
    HttpStatus   INT NULL,
    Success      BIT NOT NULL DEFAULT 0,
    ResponseText NVARCHAR(MAX) NULL,
    Message      NVARCHAR(1000) NULL
);
GO
/* ---------- حقوق و دستمزد ---------- */
IF OBJECT_ID(N'ods.SC_Employees') IS NULL
CREATE TABLE ods.SC_Employees (
    EmployeeId          INT IDENTITY(1,1) PRIMARY KEY,
    CompanyID           INT NOT NULL REFERENCES ods.SC_Companies(CompanyID),
    Code                NVARCHAR(30)  NOT NULL,
    FullName            NVARCHAR(150) NOT NULL,
    NationalID          NVARCHAR(20)  NULL,
    JobTitle            NVARCHAR(100) NULL,
    BaseSalary          DECIMAL(19,0) NOT NULL DEFAULT 0,
    HousingAllowance    DECIMAL(19,0) NOT NULL DEFAULT 0,
    FoodAllowance       DECIMAL(19,0) NOT NULL DEFAULT 0,
    ChildAllowance      DECIMAL(19,0) NOT NULL DEFAULT 0,
    InsuranceApplicable BIT NOT NULL DEFAULT 1,
    IsActive            BIT NOT NULL DEFAULT 1,
    CONSTRAINT UQ_SC_Employees UNIQUE (CompanyID, Code)
);
GO
IF OBJECT_ID(N'ods.SC_PayrollRuns') IS NULL
CREATE TABLE ods.SC_PayrollRuns (
    RunId        INT IDENTITY(1,1) PRIMARY KEY,
    CompanyID    INT NOT NULL REFERENCES ods.SC_Companies(CompanyID),
    PayrollYear  SMALLINT NOT NULL,
    PayrollMonth TINYINT NOT NULL,
    CreatedAt    DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT UQ_SC_PayrollRuns UNIQUE (CompanyID, PayrollYear, PayrollMonth)
);
GO
IF OBJECT_ID(N'ods.SC_PayrollItems') IS NULL
CREATE TABLE ods.SC_PayrollItems (
    ItemId            INT IDENTITY(1,1) PRIMARY KEY,
    RunId             INT NOT NULL REFERENCES ods.SC_PayrollRuns(RunId) ON DELETE CASCADE,
    EmployeeId        INT NOT NULL REFERENCES ods.SC_Employees(EmployeeId),
    Gross             DECIMAL(19,0) NOT NULL DEFAULT 0,
    EmployeeInsurance DECIMAL(19,0) NOT NULL DEFAULT 0,
    EmployerInsurance DECIMAL(19,0) NOT NULL DEFAULT 0,
    IncomeTax         DECIMAL(19,0) NOT NULL DEFAULT 0,
    NetPay            DECIMAL(19,0) NOT NULL DEFAULT 0
);
GO
/* ---------- گردش کار (تایید مراحل) ---------- */
IF OBJECT_ID(N'ods.SC_WorkflowSteps') IS NULL
CREATE TABLE ods.SC_WorkflowSteps (
    StepId       INT IDENTITY(1,1) PRIMARY KEY,
    CompanyID    INT NOT NULL REFERENCES ods.SC_Companies(CompanyID),
    RequestType  NVARCHAR(30)  NOT NULL,         -- Voucher / Invoice / Payroll
    StepOrder    TINYINT NOT NULL,
    StepTitle    NVARCHAR(100) NOT NULL,
    ApproverRole NVARCHAR(30)  NOT NULL          -- Admin / Accountant
);
GO
IF OBJECT_ID(N'ods.SC_WorkflowRequests') IS NULL
CREATE TABLE ods.SC_WorkflowRequests (
    RequestId    INT IDENTITY(1,1) PRIMARY KEY,
    CompanyID    INT NOT NULL REFERENCES ods.SC_Companies(CompanyID),
    RequestType  NVARCHAR(30)  NOT NULL,
    RefId        INT NOT NULL,
    Title        NVARCHAR(300) NOT NULL,
    RequestedBy  NVARCHAR(100) NOT NULL,
    RequestedAt  DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CurrentStep  TINYINT NOT NULL DEFAULT 1,
    Status       TINYINT NOT NULL DEFAULT 0,     -- 0 در انتظار، 1 تایید نهایی، 2 رد
    LastActionBy NVARCHAR(100) NULL,
    LastActionAt DATETIME2 NULL,
    Comment      NVARCHAR(500) NULL
);
GO
/* ---------- ایندکس‌ها ---------- */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SC_VoucherLines_Voucher')
    CREATE INDEX IX_SC_VoucherLines_Voucher ON ods.SC_VoucherLines(VoucherId) INCLUDE (AccountId, Debit, Credit);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SC_Vouchers_Date')
    CREATE INDEX IX_SC_Vouchers_Date ON ods.SC_Vouchers(CompanyID, VoucherDate, Status);
GO
/* ---------- رویه ایجاد سرفصل‌ها و گردش‌کار پیش‌فرض برای یک شرکت ---------- */
CREATE OR ALTER PROCEDURE ods.SC_SeedChartOfAccounts @CompanyId INT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM ods.SC_Accounts WHERE CompanyID = @CompanyId) RETURN;
    INSERT INTO ods.SC_Accounts (CompanyID, Code, Title, [Level], Nature, IsFloating)
    VALUES
    (@CompanyId, N'1',    N'دارایی‌ها',                       1, 1, 0),
    (@CompanyId, N'11',   N'دارایی‌های جاری',                 2, 1, 0),
    (@CompanyId, N'1101', N'صندوق',                           3, 1, 0),
    (@CompanyId, N'1102', N'بانک‌ها',                         3, 1, 1),
    (@CompanyId, N'1103', N'حساب‌های دریافتنی تجاری',         3, 1, 1),
    (@CompanyId, N'1104', N'موجودی کالا',                     3, 1, 0),
    (@CompanyId, N'2',    N'بدهی‌ها',                         1, 2, 0),
    (@CompanyId, N'21',   N'بدهی‌های جاری',                   2, 2, 0),
    (@CompanyId, N'2101', N'حساب‌های پرداختنی تجاری',         3, 2, 1),
    (@CompanyId, N'2102', N'مالیات بر ارزش افزوده پرداختنی',  3, 2, 0),
    (@CompanyId, N'2103', N'حقوق و دستمزد پرداختنی',          3, 2, 1),
    (@CompanyId, N'3',    N'حقوق صاحبان سهام',                1, 2, 0),
    (@CompanyId, N'4',    N'درآمدها',                         1, 2, 0),
    (@CompanyId, N'4101', N'درآمد فروش',                      3, 2, 0),
    (@CompanyId, N'5',    N'هزینه‌ها',                        1, 1, 0),
    (@CompanyId, N'5101', N'بهای تمام شده کالای فروش رفته',   3, 1, 0),
    (@CompanyId, N'5201', N'هزینه حقوق و دستمزد',             3, 1, 0),
    (@CompanyId, N'5202', N'هزینه‌های عمومی و اداری',         3, 1, 0);
    INSERT INTO ods.SC_WorkflowSteps (CompanyID, RequestType, StepOrder, StepTitle, ApproverRole)
    VALUES (@CompanyId, N'Voucher', 1, N'تایید حسابدار', N'Accountant'),
           (@CompanyId, N'Voucher', 2, N'تایید مدیر', N'Admin'),
           (@CompanyId, N'Invoice', 1, N'تایید حسابدار', N'Accountant'),
           (@CompanyId, N'Payroll', 1, N'تایید مدیر', N'Admin');
END
GO

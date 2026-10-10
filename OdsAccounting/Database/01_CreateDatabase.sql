-- اجرا در SQL Server 2025 (SSMS یا sqlcmd) — یک بار برای ساخت پایگاه داده
IF DB_ID(N'ODS_AccountingDB') IS NULL
    CREATE DATABASE ODS_AccountingDB;
GO

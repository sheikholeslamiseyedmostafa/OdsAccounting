using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace OdsAccounting
{
    /// <summary>
    /// Reads and writes fiscal periods using the actual table and column metadata exposed by SQL Server.
    /// </summary>
    internal sealed class FinancialPeriodRepository
    {
        internal const string DefaultConnectionString = "Server=SMSHEIKH\\SQL25;Database=ODS_AccountingDB;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;";

        private const string CompanySchemaName = "ods";
        private const string CompanyTableName = "SC_Companies";
        private const string CompanyNameColumnName = "CompanyName";
        private static readonly PersianCalendar JalaliCalendar = new PersianCalendar();

        private readonly string connectionString;

        internal FinancialPeriodRepository() : this(DefaultConnectionString)
        {
        }

        internal FinancialPeriodRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        internal DataTable LoadForCompany(string companyName)
        {
            if (string.IsNullOrWhiteSpace(companyName))
            {
                throw new ArgumentException("نام شرکت برای بارگذاری دوره‌های مالی الزامی است.", nameof(companyName));
            }

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            FinancialPeriodSchema schema = DiscoverSchema(connection);
            using SqlCommand command = CreateSelectCommand(connection, schema, companyName.Trim());
            DataTable periods = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                adapter.Fill(periods);
            }

            return periods;
        }

        internal void CreatePeriod(
            string companyName,
            string periodName,
            DateTime startDate,
            DateTime endDate,
            bool isOpen,
            bool isActive,
            string description,
            string color)
        {
            if (string.IsNullOrWhiteSpace(companyName))
            {
                throw new ArgumentException("نام شرکت برای ثبت دوره مالی الزامی است.", nameof(companyName));
            }

            if (string.IsNullOrWhiteSpace(periodName))
            {
                throw new ArgumentException("نام دوره مالی الزامی است.", nameof(periodName));
            }

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            FinancialPeriodSchema schema = DiscoverSchema(connection);
            EnsureInsertFieldsAvailable(schema);

            List<InsertValue> values = new List<InsertValue>();
            AddValue(values, schema.PeriodNameColumn, periodName.Trim());
            AddValue(values, schema.StartDateColumn!, ConvertDateForColumn(startDate, schema.StartDateColumn!));
            AddValue(values, schema.EndDateColumn!, ConvertDateForColumn(endDate, schema.EndDateColumn!));
            AddValue(values, schema.OpenStateColumn!.Column, schema.OpenStateColumn.IsClosedFlag ? !isOpen : isOpen);
            AddValue(values, schema.ActiveColumn!, isActive);

            string normalizedDescription = description?.Trim() ?? string.Empty;
            if (schema.DescriptionColumn != null)
            {
                AddValue(values, schema.DescriptionColumn, normalizedDescription);
            }
            else if (!string.IsNullOrWhiteSpace(normalizedDescription))
            {
                throw new InvalidOperationException("در جدول دوره‌های مالی ستون توضیحات پیدا نشد؛ توضیحات واردشده قابل ذخیره نیست.");
            }

            string normalizedColor = color?.Trim() ?? string.Empty;
            if (schema.ColorColumn != null)
            {
                AddValue(values, schema.ColorColumn, normalizedColor);
            }
            else if (!string.IsNullOrWhiteSpace(normalizedColor) && !string.Equals(normalizedColor, "بدون رنگ", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("در جدول دوره‌های مالی ستون رنگ پیدا نشد؛ رنگ انتخاب‌شده قابل ذخیره نیست.");
            }

            if (!string.IsNullOrWhiteSpace(schema.Relationship.CompanyNameColumnOnSource))
            {
                AddValue(values, schema.Relationship.CompanyNameColumnOnSourceMetadata!, companyName.Trim());
            }
            else
            {
                Dictionary<string, object> companyKeyValues = LoadCompanyKeyValues(connection, schema, companyName.Trim());
                foreach (CompanyColumnPair pair in schema.Relationship.ColumnPairs)
                {
                    if (!companyKeyValues.TryGetValue(pair.CompanyColumnName, out object? companyKeyValue))
                    {
                        throw new InvalidOperationException($"مقدار ستون کلید شرکت [{pair.CompanyColumnName}] از پایگاه داده خوانده نشد.");
                    }

                    AddValue(values, schema.PeriodTable.Columns[pair.PeriodColumnName], companyKeyValue);
                }
            }

            List<string> missingRequiredColumns = schema.PeriodTable.Columns.Values
                .Where(column => !column.IsNullable && !column.IsIdentity && !column.IsComputed && !column.HasDefault && column.SystemTypeId != 189)
                .Where(column => !values.Any(value => string.Equals(value.Column.Name, column.Name, StringComparison.OrdinalIgnoreCase)))
                .Select(column => column.Name)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (missingRequiredColumns.Count > 0)
            {
                throw new InvalidOperationException(
                    "این ستون‌های اجباری جدول دوره مالی در فرم فعلی مقدار ندارند و مقدار پیش‌فرض هم ندارند: " +
                    string.Join("، ", missingRequiredColumns));
            }

            using SqlCommand insertCommand = CreateInsertCommand(connection, schema, values);
            insertCommand.ExecuteNonQuery();
        }

        private static void EnsureInsertFieldsAvailable(FinancialPeriodSchema schema)
        {
            List<string> missing = new List<string>();
            if (schema.StartDateColumn == null) missing.Add("تاریخ شروع");
            if (schema.EndDateColumn == null) missing.Add("تاریخ پایان");
            if (schema.OpenStateColumn == null) missing.Add("باز/بسته بودن دوره");
            if (schema.ActiveColumn == null) missing.Add("وضعیت فعال");

            if (missing.Count > 0)
            {
                throw new InvalidOperationException(
                    "ستون متناظر با این اطلاعات در شِمای واقعی جدول دوره مالی پیدا نشد: " + string.Join("، ", missing));
            }
        }

        private static object ConvertDateForColumn(DateTime date, ColumnMetadata column)
        {
            switch (column.SystemTypeId)
            {
                case 40: // date
                case 58: // smalldatetime
                case 61: // datetime
                case 42: // datetime2
                    return date.Date;
                case 43: // datetimeoffset
                    return new DateTimeOffset(date.Date, TimeSpan.Zero);
                case 35: // text
                case 99: // ntext
                case 167: // varchar
                case 175: // char
                case 231: // nvarchar
                case 239: // nchar
                    return FormatPersianDate(date);
                default:
                    throw new InvalidOperationException(
                        $"نوع داده ستون تاریخ [{column.Name}] با شناسه {column.SystemTypeId} پشتیبانی نمی‌شود.");
            }
        }

        private static string FormatPersianDate(DateTime date)
        {
            return JalaliCalendar.GetYear(date).ToString("0000", CultureInfo.InvariantCulture) + "/" +
                   JalaliCalendar.GetMonth(date).ToString("00", CultureInfo.InvariantCulture) + "/" +
                   JalaliCalendar.GetDayOfMonth(date).ToString("00", CultureInfo.InvariantCulture);
        }

        private static void AddValue(List<InsertValue> values, ColumnMetadata column, object value)
        {
            if (values.Any(existing => string.Equals(existing.Column.Name, column.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"ستون [{column.Name}] بیش از یک بار برای ثبت مقداردهی شده است.");
            }

            values.Add(new InsertValue(column, value));
        }

        private static SqlCommand CreateInsertCommand(SqlConnection connection, FinancialPeriodSchema schema, List<InsertValue> values)
        {
            string columnList = string.Join(", ", values.Select(value => QuoteIdentifier(value.Column.Name)));
            string parameterList = string.Join(", ", values.Select((_, index) => "@value" + index));
            string query = $"INSERT INTO {QuoteTable(schema.PeriodTable.SchemaName, schema.PeriodTable.TableName)} ({columnList}) VALUES ({parameterList});";

            SqlCommand command = new SqlCommand(query, connection);
            for (int index = 0; index < values.Count; index++)
            {
                command.Parameters.AddWithValue("@value" + index, values[index].Value);
            }

            return command;
        }

        private static Dictionary<string, object> LoadCompanyKeyValues(
            SqlConnection connection,
            FinancialPeriodSchema schema,
            string companyName)
        {
            string[] keyColumns = schema.Relationship.ColumnPairs
                .Select(pair => pair.CompanyColumnName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            if (keyColumns.Length == 0)
            {
                throw new InvalidOperationException("رابطه دوره مالی با شرکت فاقد کلید قابل‌خواندن است.");
            }

            string selectList = string.Join(", ", keyColumns.Select((column, index) =>
                $"company.{QuoteIdentifier(column)} AS {QuoteIdentifier("CompanyKey" + index)}"));
            string companyTable = QuoteTable(schema.CompanyTable.SchemaName, schema.CompanyTable.TableName);
            string companyNameColumn = QuoteIdentifier(schema.CompanyNameColumn.Name);
            string query = $@"
                SELECT TOP (2) {selectList}
                FROM {companyTable} AS company
                WHERE CAST(company.{companyNameColumn} AS nvarchar(4000)) = @CompanyName;";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add("@CompanyName", SqlDbType.NVarChar, 4000).Value = companyName;

            using SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                throw new InvalidOperationException($"شرکت «{companyName}» در جدول شرکت‌ها پیدا نشد.");
            }

            Dictionary<string, object> values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            for (int index = 0; index < keyColumns.Length; index++)
            {
                values[keyColumns[index]] = reader.GetValue(index);
            }

            if (reader.Read())
            {
                throw new InvalidOperationException($"نام شرکت «{companyName}» در جدول شرکت‌ها یکتا نیست؛ دوره مالی به شرکت مشخصی متصل نشد.");
            }

            return values;
        }

        private static FinancialPeriodSchema DiscoverSchema(SqlConnection connection)
        {
            int companyObjectId = GetCompanyTableObjectId(connection);
            Dictionary<int, TableMetadata> tables = LoadRelevantTableMetadata(connection, companyObjectId);
            if (!tables.TryGetValue(companyObjectId, out TableMetadata? companyTable))
            {
                throw new InvalidOperationException("ساختار جدول شرکت‌ها از پایگاه داده خوانده نشد.");
            }

            if (!companyTable.Columns.TryGetValue(CompanyNameColumnName, out ColumnMetadata? companyNameColumn))
            {
                throw new InvalidOperationException($"ستون [{CompanyNameColumnName}] در جدول [{CompanySchemaName}].[{CompanyTableName}] پیدا نشد.");
            }

            List<CompanyKeyMetadata> companyKeys = LoadCompanyKeyColumns(connection, companyObjectId);
            LoadForeignKeyRelationships(connection, companyObjectId, tables);
            return ResolveFinancialPeriodSchema(tables, companyTable, companyNameColumn, companyKeys);
        }

        private static int GetCompanyTableObjectId(SqlConnection connection)
        {
            const string query = @"
                SELECT tableObject.object_id
                FROM sys.tables AS tableObject
                INNER JOIN sys.schemas AS tableSchema ON tableSchema.schema_id = tableObject.schema_id
                WHERE tableSchema.name = @SchemaName
                    AND tableObject.name = @TableName
                    AND tableObject.is_ms_shipped = 0;";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add("@SchemaName", SqlDbType.NVarChar, 128).Value = CompanySchemaName;
            command.Parameters.Add("@TableName", SqlDbType.NVarChar, 128).Value = CompanyTableName;

            object? result = command.ExecuteScalar();
            if (result == null || result == DBNull.Value)
            {
                throw new InvalidOperationException($"جدول [{CompanySchemaName}].[{CompanyTableName}] در پایگاه داده پیدا نشد.");
            }

            return Convert.ToInt32(result);
        }

        private static Dictionary<int, TableMetadata> LoadRelevantTableMetadata(SqlConnection connection, int companyObjectId)
        {
            const string query = @"
                SELECT
                    tableObject.object_id AS ObjectId,
                    tableSchema.name AS SchemaName,
                    tableObject.name AS TableName,
                    tableColumn.name AS ColumnName,
                    tableColumn.system_type_id AS SystemTypeId,
                    tableColumn.column_id AS ColumnId,
                    tableColumn.is_nullable AS IsNullable,
                    tableColumn.is_identity AS IsIdentity,
                    tableColumn.is_computed AS IsComputed,
                    tableColumn.default_object_id AS DefaultObjectId
                FROM sys.tables AS tableObject
                INNER JOIN sys.schemas AS tableSchema ON tableSchema.schema_id = tableObject.schema_id
                INNER JOIN sys.columns AS tableColumn ON tableColumn.object_id = tableObject.object_id
                WHERE tableObject.is_ms_shipped = 0
                    AND (
                        tableObject.object_id = @CompanyObjectId
                        OR LOWER(tableObject.name) LIKE N'%financial%'
                        OR LOWER(tableObject.name) LIKE N'%fiscal%'
                        OR LOWER(tableObject.name) LIKE N'%period%'
                        OR LOWER(tableObject.name) LIKE N'%year%'
                        OR LOWER(tableObject.name) LIKE N'%book%'
                        OR LOWER(tableObject.name) LIKE N'%ledger%'
                        OR LOWER(tableObject.name) LIKE N'%accounting%'
                        OR EXISTS
                        (
                            SELECT 1
                            FROM sys.foreign_key_columns AS companyForeignKeyColumn
                            WHERE companyForeignKeyColumn.parent_object_id = tableObject.object_id
                                AND companyForeignKeyColumn.referenced_object_id = @CompanyObjectId
                        )
                    )
                ORDER BY tableObject.object_id, tableColumn.column_id;";

            Dictionary<int, TableMetadata> tables = new Dictionary<int, TableMetadata>();
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add("@CompanyObjectId", SqlDbType.Int).Value = companyObjectId;

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int objectId = reader.GetInt32(reader.GetOrdinal("ObjectId"));
                if (!tables.TryGetValue(objectId, out TableMetadata? table))
                {
                    table = new TableMetadata(
                        objectId,
                        reader.GetString(reader.GetOrdinal("SchemaName")),
                        reader.GetString(reader.GetOrdinal("TableName")));
                    tables.Add(objectId, table);
                }

                ColumnMetadata column = new ColumnMetadata(
                    reader.GetString(reader.GetOrdinal("ColumnName")),
                    Convert.ToInt32(reader["SystemTypeId"]),
                    reader.GetInt32(reader.GetOrdinal("ColumnId")),
                    Convert.ToBoolean(reader["IsNullable"]),
                    Convert.ToBoolean(reader["IsIdentity"]),
                    Convert.ToBoolean(reader["IsComputed"]),
                    Convert.ToInt32(reader["DefaultObjectId"]) != 0);
                table.Columns[column.Name] = column;
            }

            return tables;
        }

        private static List<CompanyKeyMetadata> LoadCompanyKeyColumns(SqlConnection connection, int companyObjectId)
        {
            const string query = @"
                SELECT
                    companyColumn.name AS ColumnName,
                    companyColumn.system_type_id AS SystemTypeId,
                    MAX(CONVERT(int, companyIndex.is_primary_key)) AS IsPrimaryKey
                FROM sys.indexes AS companyIndex
                INNER JOIN sys.index_columns AS indexColumn
                    ON indexColumn.object_id = companyIndex.object_id
                    AND indexColumn.index_id = companyIndex.index_id
                INNER JOIN sys.columns AS companyColumn
                    ON companyColumn.object_id = indexColumn.object_id
                    AND companyColumn.column_id = indexColumn.column_id
                WHERE companyIndex.object_id = @CompanyObjectId
                    AND companyIndex.is_unique = 1
                    AND companyIndex.is_disabled = 0
                    AND companyIndex.is_hypothetical = 0
                    AND indexColumn.key_ordinal = 1
                    AND NOT EXISTS
                    (
                        SELECT 1
                        FROM sys.index_columns AS additionalKeyColumn
                        WHERE additionalKeyColumn.object_id = indexColumn.object_id
                            AND additionalKeyColumn.index_id = indexColumn.index_id
                            AND additionalKeyColumn.key_ordinal > 1
                    )
                GROUP BY companyColumn.name, companyColumn.system_type_id
                ORDER BY MAX(CONVERT(int, companyIndex.is_primary_key)) DESC, companyColumn.name;";

            List<CompanyKeyMetadata> keys = new List<CompanyKeyMetadata>();
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add("@CompanyObjectId", SqlDbType.Int).Value = companyObjectId;

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                keys.Add(new CompanyKeyMetadata(
                    reader.GetString(reader.GetOrdinal("ColumnName")),
                    Convert.ToInt32(reader["SystemTypeId"]),
                    Convert.ToInt32(reader["IsPrimaryKey"]) != 0));
            }

            return keys;
        }

        private static void LoadForeignKeyRelationships(
            SqlConnection connection,
            int companyObjectId,
            Dictionary<int, TableMetadata> tables)
        {
            const string query = @"
                SELECT
                    foreignKey.object_id AS ForeignKeyObjectId,
                    foreignKey.name AS ForeignKeyName,
                    foreignKeyColumn.parent_object_id AS ParentObjectId,
                    parentColumn.name AS ParentColumnName,
                    companyColumn.name AS CompanyColumnName,
                    foreignKeyColumn.constraint_column_id AS ConstraintColumnId
                FROM sys.foreign_key_columns AS foreignKeyColumn
                INNER JOIN sys.foreign_keys AS foreignKey
                    ON foreignKey.object_id = foreignKeyColumn.constraint_object_id
                INNER JOIN sys.columns AS parentColumn
                    ON parentColumn.object_id = foreignKeyColumn.parent_object_id
                    AND parentColumn.column_id = foreignKeyColumn.parent_column_id
                INNER JOIN sys.columns AS companyColumn
                    ON companyColumn.object_id = foreignKeyColumn.referenced_object_id
                    AND companyColumn.column_id = foreignKeyColumn.referenced_column_id
                WHERE foreignKeyColumn.referenced_object_id = @CompanyObjectId
                ORDER BY foreignKey.object_id, foreignKeyColumn.constraint_column_id;";

            Dictionary<int, CompanyRelationship> relationships = new Dictionary<int, CompanyRelationship>();
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add("@CompanyObjectId", SqlDbType.Int).Value = companyObjectId;

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int parentObjectId = reader.GetInt32(reader.GetOrdinal("ParentObjectId"));
                if (!tables.TryGetValue(parentObjectId, out TableMetadata? parentTable))
                {
                    continue;
                }

                int foreignKeyObjectId = reader.GetInt32(reader.GetOrdinal("ForeignKeyObjectId"));
                if (!relationships.TryGetValue(foreignKeyObjectId, out CompanyRelationship? relationship))
                {
                    string foreignKeyName = reader.GetString(reader.GetOrdinal("ForeignKeyName"));
                    relationship = new CompanyRelationship(foreignKeyName, 300, "FK " + foreignKeyName);
                    relationships.Add(foreignKeyObjectId, relationship);
                    parentTable.CompanyRelationships.Add(relationship);
                }

                relationship.ColumnPairs.Add(new CompanyColumnPair(
                    reader.GetString(reader.GetOrdinal("ParentColumnName")),
                    reader.GetString(reader.GetOrdinal("CompanyColumnName"))));
            }
        }

        private static FinancialPeriodSchema ResolveFinancialPeriodSchema(
            Dictionary<int, TableMetadata> tables,
            TableMetadata companyTable,
            ColumnMetadata companyNameColumn,
            List<CompanyKeyMetadata> companyKeys)
        {
            List<FinancialPeriodSchema> candidates = new List<FinancialPeriodSchema>();

            foreach (TableMetadata table in tables.Values)
            {
                if (table.ObjectId == companyTable.ObjectId)
                {
                    continue;
                }

                int tableNameScore = GetTableNameScore(table.TableName);
                if (tableNameScore == 0)
                {
                    continue;
                }

                ColumnMetadata? periodNameColumn = FindBestColumn(table, GetPeriodNameColumnScore);
                if (periodNameColumn == null)
                {
                    continue;
                }

                ColumnMetadata? descriptionColumn = FindBestColumn(table, GetDescriptionColumnScore);
                ColumnMetadata? startDateColumn = FindBestColumn(table, GetStartDateColumnScore);
                ColumnMetadata? endDateColumn = FindBestColumn(table, GetEndDateColumnScore);
                ColumnMetadata? activeColumn = FindBestColumn(table, GetActiveColumnScore, column => column.SystemTypeId == 104);
                OpenStateMetadata? openStateColumn = FindOpenStateColumn(table);
                ColumnMetadata? colorColumn = FindBestColumn(table, GetColorColumnScore, IsTextColumn);

                List<CompanyRelationship> companyRelationships = GetCompanyRelationships(table, companyNameColumn, companyKeys);
                int periodNameScore = GetPeriodNameColumnScore(periodNameColumn.Name);
                int columnSupportBonus =
                    (descriptionColumn == null ? 0 : 1000) +
                    (startDateColumn == null ? 0 : 10000) +
                    (endDateColumn == null ? 0 : 10000) +
                    (activeColumn == null ? 0 : 10000) +
                    (openStateColumn == null ? 0 : 10000);

                foreach (CompanyRelationship relationship in companyRelationships)
                {
                    int score = (tableNameScore * 10000) + (periodNameScore * 10) + relationship.Score + columnSupportBonus;
                    candidates.Add(new FinancialPeriodSchema(
                        table,
                        companyTable,
                        companyNameColumn,
                        periodNameColumn,
                        descriptionColumn,
                        startDateColumn,
                        endDateColumn,
                        activeColumn,
                        openStateColumn,
                        colorColumn,
                        relationship,
                        score));
                }
            }

            if (candidates.Count == 0)
            {
                throw new InvalidOperationException(
                    "در پایگاه داده، جدول دوره مالی با ستون نام دوره و رابطه قابل‌تأیید به جدول " +
                    $"[{CompanySchemaName}].[{CompanyTableName}] پیدا نشد. نام جدول، ستون‌ها و کلیدهای واقعی شرکت از متادیتای SQL Server بررسی شدند؛ " +
                    "اما شِمای پایگاه داده در مخزن پروژه موجود نیست تا نام دیگری به‌صورت قطعی فرض شود.");
            }

            int highestScore = candidates.Max(candidate => candidate.Score);
            List<FinancialPeriodSchema> bestCandidates = candidates.Where(candidate => candidate.Score == highestScore).ToList();
            if (bestCandidates.Count > 1)
            {
                string matchingTables = string.Join(", ", bestCandidates.Select(candidate =>
                    $"[{candidate.PeriodTable.SchemaName}].[{candidate.PeriodTable.TableName}] ({candidate.Relationship.Description})"));
                throw new InvalidOperationException("چند ساختار هم‌امتیاز برای دوره مالی پیدا شد و انتخاب امن ممکن نیست: " + matchingTables);
            }

            return bestCandidates[0];
        }

        private static ColumnMetadata? FindBestColumn(
            TableMetadata table,
            Func<string, int> scoreSelector,
            Func<ColumnMetadata, bool>? predicate = null)
        {
            return table.Columns.Values
                .Where(column => predicate == null || predicate(column))
                .Select(column => new { Column = column, Score = scoreSelector(column.Name) })
                .Where(item => item.Score > 0)
                .OrderByDescending(item => item.Score)
                .ThenBy(item => item.Column.ColumnId)
                .Select(item => item.Column)
                .FirstOrDefault();
        }

        private static List<CompanyRelationship> GetCompanyRelationships(
            TableMetadata table,
            ColumnMetadata companyNameColumn,
            List<CompanyKeyMetadata> companyKeys)
        {
            if (table.CompanyRelationships.Count > 0)
            {
                return table.CompanyRelationships;
            }

            List<CompanyRelationship> matchingKeys = new List<CompanyRelationship>();
            foreach (CompanyKeyMetadata companyKey in companyKeys)
            {
                if (!table.Columns.TryGetValue(companyKey.ColumnName, out ColumnMetadata? periodCompanyColumn) ||
                    !AreCompatibleKeyTypes(periodCompanyColumn.SystemTypeId, companyKey.SystemTypeId))
                {
                    continue;
                }

                string description = $"{periodCompanyColumn.Name} → {companyKey.ColumnName}";
                CompanyRelationship relationship = new CompanyRelationship(description, companyKey.IsPrimaryKey ? 260 : 240, description);
                relationship.ColumnPairs.Add(new CompanyColumnPair(periodCompanyColumn.Name, companyKey.ColumnName));
                matchingKeys.Add(relationship);
            }

            if (matchingKeys.Count > 0)
            {
                return matchingKeys;
            }

            if (table.Columns.TryGetValue(CompanyNameColumnName, out ColumnMetadata? directCompanyNameColumn) &&
                AreCompatibleKeyTypes(directCompanyNameColumn.SystemTypeId, companyNameColumn.SystemTypeId))
            {
                CompanyRelationship relationship = new CompanyRelationship(CompanyNameColumnName, 200, "اتصال مستقیم با نام شرکت")
                {
                    CompanyNameColumnOnSource = directCompanyNameColumn.Name,
                    CompanyNameColumnOnSourceMetadata = directCompanyNameColumn
                };
                return new List<CompanyRelationship> { relationship };
            }

            return new List<CompanyRelationship>();
        }

        private static bool AreCompatibleKeyTypes(int firstTypeId, int secondTypeId)
        {
            if (firstTypeId == secondTypeId) return true;

            bool firstIsInteger = firstTypeId is 48 or 52 or 56 or 127;
            bool secondIsInteger = secondTypeId is 48 or 52 or 56 or 127;
            if (firstIsInteger && secondIsInteger) return true;

            return IsTextType(firstTypeId) && IsTextType(secondTypeId);
        }

        private static bool IsTextColumn(ColumnMetadata column)
        {
            return IsTextType(column.SystemTypeId);
        }

        private static bool IsTextType(int systemTypeId)
        {
            return systemTypeId is 35 or 99 or 167 or 175 or 231 or 239;
        }

        private static int GetTableNameScore(string tableName)
        {
            string name = NormalizeName(tableName);
            bool hasFinancialTerm = name.Contains("financial", StringComparison.Ordinal) ||
                                    name.Contains("fiscal", StringComparison.Ordinal) ||
                                    name.Contains("accounting", StringComparison.Ordinal);
            bool hasPeriodTerm = name.Contains("period", StringComparison.Ordinal);
            bool hasYearTerm = name.Contains("year", StringComparison.Ordinal);
            bool hasBookTerm = name.Contains("book", StringComparison.Ordinal);

            if (hasPeriodTerm && hasFinancialTerm) return 100;
            if (hasYearTerm && hasFinancialTerm) return 95;
            if (hasBookTerm && hasFinancialTerm) return 90;
            if (hasPeriodTerm) return 80;
            if (hasYearTerm) return 70;
            if (hasBookTerm || name.Contains("ledger", StringComparison.Ordinal)) return 60;
            return 0;
        }

        private static int GetPeriodNameColumnScore(string columnName)
        {
            string name = NormalizeName(columnName);
            return name switch
            {
                "financialperiodname" => 1200,
                "fiscalperiodname" => 1180,
                "accountingperiodname" => 1160,
                "finperiodname" => 1140,
                "fperiodname" => 1130,
                "periodname" => 1100,
                "financialyearname" => 1080,
                "fiscalyearname" => 1060,
                "accountingyearname" => 1040,
                "finyearname" => 1020,
                "fyearname" => 1010,
                "yearname" => 1000,
                "bookname" => 950,
                "financialperiodtitle" => 900,
                "fiscalperiodtitle" => 890,
                "accountingperiodtitle" => 885,
                "periodtitle" => 880,
                "financialyear" => 850,
                "fiscalyear" => 840,
                "accountingyear" => 830,
                "finperiod" => 820,
                "fperiod" => 810,
                "period" => 800,
                "finyear" => 770,
                "fyear" => 760,
                "year" => 750,
                "title" => 650,
                "name" => 550,
                _ => 0
            };
        }

        private static int GetDescriptionColumnScore(string columnName)
        {
            string name = NormalizeName(columnName);
            return name switch
            {
                "financialperioddescription" => 1200,
                "fiscalperioddescription" => 1180,
                "accountingperioddescription" => 1160,
                "perioddescription" => 1150,
                "financialperioddesc" => 1140,
                "fiscalperioddesc" => 1130,
                "perioddesc" => 1120,
                "financialyeardescription" => 1100,
                "fiscalyeardescription" => 1080,
                "description" => 1000,
                "notes" => 800,
                "details" => 700,
                "memo" => 650,
                "comment" => 610,
                "comments" => 600,
                _ => 0
            };
        }

        private static int GetStartDateColumnScore(string columnName)
        {
            string name = NormalizeName(columnName);
            return name switch
            {
                "financialperiodstartdate" => 1200,
                "fiscalperiodstartdate" => 1180,
                "periodstartdate" => 1160,
                "financialperiodstartdatejalali" => 1150,
                "periodstartdatejalali" => 1140,
                "financialyearstartdate" => 1120,
                "fiscalyearstartdate" => 1100,
                "yearstartdate" => 1080,
                "startdatejalali" => 1060,
                "persianstartdate" => 1050,
                "startdate" => 1040,
                "financialstartdate" => 1030,
                "fiscalstartdate" => 1020,
                "fromdate" => 1000,
                "datestart" => 980,
                "begindate" => 950,
                "periodstart" => 900,
                "start" => 700,
                _ => 0
            };
        }

        private static int GetEndDateColumnScore(string columnName)
        {
            string name = NormalizeName(columnName);
            return name switch
            {
                "financialperiodenddate" => 1200,
                "fiscalperiodenddate" => 1180,
                "periodenddate" => 1160,
                "financialperiodenddatejalali" => 1150,
                "periodenddatejalali" => 1140,
                "financialyearenddate" => 1120,
                "fiscalyearenddate" => 1100,
                "yearenddate" => 1080,
                "enddatejalali" => 1060,
                "persianenddate" => 1050,
                "enddate" => 1040,
                "financialenddate" => 1030,
                "fiscalenddate" => 1020,
                "todate" => 1000,
                "dateend" => 980,
                "finishdate" => 950,
                "periodend" => 900,
                "end" => 700,
                _ => 0
            };
        }

        private static int GetActiveColumnScore(string columnName)
        {
            string name = NormalizeName(columnName);
            return name switch
            {
                "isactive" => 1200,
                "isperiodactive" => 1150,
                "active" => 1000,
                _ => 0
            };
        }

        private static OpenStateMetadata? FindOpenStateColumn(TableMetadata table)
        {
            return table.Columns.Values
                .Where(column => column.SystemTypeId == 104)
                .Select(column =>
                {
                    (int score, bool isClosedFlag) = GetOpenStateColumnScore(column.Name);
                    return new { Column = column, Score = score, IsClosedFlag = isClosedFlag };
                })
                .Where(item => item.Score > 0)
                .OrderByDescending(item => item.Score)
                .ThenBy(item => item.Column.ColumnId)
                .Select(item => new OpenStateMetadata(item.Column, item.IsClosedFlag))
                .FirstOrDefault();
        }

        private static (int Score, bool IsClosedFlag) GetOpenStateColumnScore(string columnName)
        {
            string name = NormalizeName(columnName);
            return name switch
            {
                "isopen" => (1200, false),
                "isperiodopen" => (1180, false),
                "periodisopen" => (1160, false),
                "financialperiodisopen" => (1140, false),
                "isfinancialperiodopen" => (1130, false),
                "isopenperiod" => (1120, false),
                "periodopen" => (900, false),
                "open" => (890, false),
                "isclosed" => (1100, true),
                "isperiodclosed" => (1080, true),
                "periodisclosed" => (1060, true),
                "financialperiodisclosed" => (1040, true),
                "isfinancialperiodclosed" => (1030, true),
                "isclosedperiod" => (1020, true),
                "periodclosed" => (850, true),
                "closed" => (840, true),
                _ => (0, false)
            };
        }

        private static int GetColorColumnScore(string columnName)
        {
            string name = NormalizeName(columnName);
            return name switch
            {
                "colorcode" => 1200,
                "financialperiodcolor" => 1180,
                "periodcolor" => 1160,
                "color" => 1000,
                _ => 0
            };
        }

        private static string NormalizeName(string value)
        {
            return new string(value.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
        }

        private static SqlCommand CreateSelectCommand(SqlConnection connection, FinancialPeriodSchema schema, string companyName)
        {
            string periodTable = QuoteTable(schema.PeriodTable.SchemaName, schema.PeriodTable.TableName);
            string periodNameExpression = $"CAST(period.{QuoteIdentifier(schema.PeriodNameColumn.Name)} AS nvarchar(4000))";
            string descriptionExpression = schema.DescriptionColumn == null
                ? "CAST(NULL AS nvarchar(max))"
                : $"CAST(period.{QuoteIdentifier(schema.DescriptionColumn.Name)} AS nvarchar(max))";

            string fromClause;
            string companyNameExpression;
            string companyNameFilter;
            if (!string.IsNullOrWhiteSpace(schema.Relationship.CompanyNameColumnOnSource))
            {
                string sourceCompanyName = $"period.{QuoteIdentifier(schema.Relationship.CompanyNameColumnOnSource)}";
                companyNameExpression = $"CAST({sourceCompanyName} AS nvarchar(4000))";
                fromClause = $"FROM {periodTable} AS period";
                companyNameFilter = $"CAST({sourceCompanyName} AS nvarchar(4000)) = @CompanyName";
            }
            else
            {
                string joinCondition = string.Join(" AND ", schema.Relationship.ColumnPairs.Select(pair =>
                    $"period.{QuoteIdentifier(pair.PeriodColumnName)} = company.{QuoteIdentifier(pair.CompanyColumnName)}"));
                string companyTable = QuoteTable(schema.CompanyTable.SchemaName, schema.CompanyTable.TableName);
                string companyNameColumn = QuoteIdentifier(schema.CompanyNameColumn.Name);
                fromClause = $"FROM {periodTable} AS period INNER JOIN {companyTable} AS company ON {joinCondition}";
                companyNameExpression = $"CAST(company.{companyNameColumn} AS nvarchar(4000))";
                companyNameFilter = $"CAST(company.{companyNameColumn} AS nvarchar(4000)) = @CompanyName";
            }

            string query = $@"
                SELECT
                    ROW_NUMBER() OVER (ORDER BY {periodNameExpression}) AS [RowNumber],
                    {periodNameExpression} AS [FinancialPeriodName],
                    {companyNameExpression} AS [CompanyName],
                    {descriptionExpression} AS [Description]
                {fromClause}
                WHERE {companyNameFilter}
                ORDER BY {periodNameExpression};";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add("@CompanyName", SqlDbType.NVarChar, 4000).Value = companyName;
            return command;
        }

        private static string QuoteTable(string schemaName, string tableName)
        {
            return QuoteIdentifier(schemaName) + "." + QuoteIdentifier(tableName);
        }

        private static string QuoteIdentifier(string identifier)
        {
            return "[" + identifier.Replace("]", "]]", StringComparison.Ordinal) + "]";
        }

        private sealed class TableMetadata
        {
            public TableMetadata(int objectId, string schemaName, string tableName)
            {
                ObjectId = objectId;
                SchemaName = schemaName;
                TableName = tableName;
            }

            public int ObjectId { get; }
            public string SchemaName { get; }
            public string TableName { get; }
            public Dictionary<string, ColumnMetadata> Columns { get; } = new Dictionary<string, ColumnMetadata>(StringComparer.OrdinalIgnoreCase);
            public List<CompanyRelationship> CompanyRelationships { get; } = new List<CompanyRelationship>();
        }

        private sealed class ColumnMetadata
        {
            public ColumnMetadata(
                string name,
                int systemTypeId,
                int columnId,
                bool isNullable,
                bool isIdentity,
                bool isComputed,
                bool hasDefault)
            {
                Name = name;
                SystemTypeId = systemTypeId;
                ColumnId = columnId;
                IsNullable = isNullable;
                IsIdentity = isIdentity;
                IsComputed = isComputed;
                HasDefault = hasDefault;
            }

            public string Name { get; }
            public int SystemTypeId { get; }
            public int ColumnId { get; }
            public bool IsNullable { get; }
            public bool IsIdentity { get; }
            public bool IsComputed { get; }
            public bool HasDefault { get; }
        }

        private sealed class CompanyKeyMetadata
        {
            public CompanyKeyMetadata(string columnName, int systemTypeId, bool isPrimaryKey)
            {
                ColumnName = columnName;
                SystemTypeId = systemTypeId;
                IsPrimaryKey = isPrimaryKey;
            }

            public string ColumnName { get; }
            public int SystemTypeId { get; }
            public bool IsPrimaryKey { get; }
        }

        private sealed class CompanyColumnPair
        {
            public CompanyColumnPair(string periodColumnName, string companyColumnName)
            {
                PeriodColumnName = periodColumnName;
                CompanyColumnName = companyColumnName;
            }

            public string PeriodColumnName { get; }
            public string CompanyColumnName { get; }
        }

        private sealed class CompanyRelationship
        {
            public CompanyRelationship(string name, int score, string description)
            {
                Name = name;
                Score = score;
                Description = description;
            }

            public string Name { get; }
            public int Score { get; }
            public string Description { get; }
            public string? CompanyNameColumnOnSource { get; set; }
            public ColumnMetadata? CompanyNameColumnOnSourceMetadata { get; set; }
            public List<CompanyColumnPair> ColumnPairs { get; } = new List<CompanyColumnPair>();
        }

        private sealed class OpenStateMetadata
        {
            public OpenStateMetadata(ColumnMetadata column, bool isClosedFlag)
            {
                Column = column;
                IsClosedFlag = isClosedFlag;
            }

            public ColumnMetadata Column { get; }
            public bool IsClosedFlag { get; }
        }

        private sealed class FinancialPeriodSchema
        {
            public FinancialPeriodSchema(
                TableMetadata periodTable,
                TableMetadata companyTable,
                ColumnMetadata companyNameColumn,
                ColumnMetadata periodNameColumn,
                ColumnMetadata? descriptionColumn,
                ColumnMetadata? startDateColumn,
                ColumnMetadata? endDateColumn,
                ColumnMetadata? activeColumn,
                OpenStateMetadata? openStateColumn,
                ColumnMetadata? colorColumn,
                CompanyRelationship relationship,
                int score)
            {
                PeriodTable = periodTable;
                CompanyTable = companyTable;
                CompanyNameColumn = companyNameColumn;
                PeriodNameColumn = periodNameColumn;
                DescriptionColumn = descriptionColumn;
                StartDateColumn = startDateColumn;
                EndDateColumn = endDateColumn;
                ActiveColumn = activeColumn;
                OpenStateColumn = openStateColumn;
                ColorColumn = colorColumn;
                Relationship = relationship;
                Score = score;
            }

            public TableMetadata PeriodTable { get; }
            public TableMetadata CompanyTable { get; }
            public ColumnMetadata CompanyNameColumn { get; }
            public ColumnMetadata PeriodNameColumn { get; }
            public ColumnMetadata? DescriptionColumn { get; }
            public ColumnMetadata? StartDateColumn { get; }
            public ColumnMetadata? EndDateColumn { get; }
            public ColumnMetadata? ActiveColumn { get; }
            public OpenStateMetadata? OpenStateColumn { get; }
            public ColumnMetadata? ColorColumn { get; }
            public CompanyRelationship Relationship { get; }
            public int Score { get; }
        }

        private sealed class InsertValue
        {
            public InsertValue(ColumnMetadata column, object value)
            {
                Column = column;
                Value = value;
            }

            public ColumnMetadata Column { get; }
            public object Value { get; }
        }
    }
}

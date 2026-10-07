using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace OdsAccounting
{
    public partial class FrmSelectFinancialPeriod : Form
    {
        private const string CompanySchemaName = "ods";
        private const string CompanyTableName = "SC_Companies";
        private const string CompanyNameColumnName = "CompanyName";

        private readonly string connectionString = "Server=SMSHEIKH\\SQL25;Database=ODS_AccountingDB;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;";
        private readonly string selectedCompanyName;
        private readonly MainForm? mainForm;

        public FrmSelectFinancialPeriod() : this(Properties.Settings.Default.SelectedCompany, null)
        {
        }

        public FrmSelectFinancialPeriod(string companyName) : this(companyName, null)
        {
        }

        public FrmSelectFinancialPeriod(string companyName, MainForm? mainForm)
        {
            InitializeComponent();
            selectedCompanyName = companyName?.Trim() ?? string.Empty;
            this.mainForm = mainForm;

            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowTemplate.Height = 40;
            dataGridView1.RightToLeft = RightToLeft.Yes;

            ApplyFontToAllControls(this);

            btnSelect.Click += btnSelect_Click;
            btnCancel.Click += btnCancel_Click;
            dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
        }

        private void FrmSelectFinancialPeriod_Load(object? sender, EventArgs e)
        {
            LoadFinancialPeriods();
        }

        /// <summary>
        /// Loads fiscal periods by resolving the actual period table and company relationship
        /// from SQL Server catalog metadata, rather than assuming a table or foreign-key name.
        /// </summary>
        public void LoadFinancialPeriods()
        {
            if (string.IsNullOrWhiteSpace(selectedCompanyName))
            {
                dataGridView1.DataSource = null;
                MessageBox.Show("ابتدا یک شرکت را انتخاب کنید.", "انتخاب سال مالی", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CloseCurrentTab();
                return;
            }

            string currentCompanyName = Properties.Settings.Default.SelectedCompany?.Trim() ?? string.Empty;
            if (!string.Equals(currentCompanyName, selectedCompanyName, StringComparison.Ordinal))
            {
                dataGridView1.DataSource = null;
                MessageBox.Show("شرکت جاری تغییر کرده است. فرم انتخاب سال مالی را دوباره باز کنید.", "انتخاب سال مالی", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CloseCurrentTab();
                return;
            }

            try
            {
                using SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

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

                FinancialPeriodSchema periodSchema = ResolveFinancialPeriodSchema(tables, companyTable, companyNameColumn, companyKeys);
                using SqlCommand command = CreatePeriodsCommand(connection, companyTable, companyNameColumn, periodSchema, selectedCompanyName);

                DataTable periods = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(periods);
                }

                dataGridView1.DataSource = periods;
                ConfigureColumns();
            }
            catch (Exception ex)
            {
                dataGridView1.DataSource = null;
                MessageBox.Show(
                    "خطا در شناسایی یا بارگذاری دفاتر مالی شرکت انتخاب‌شده:\n" + ex.Message,
                    "خطا",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
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
                    tableColumn.column_id AS ColumnId
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
                    reader.GetInt32(reader.GetOrdinal("ColumnId")));
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

                ColumnMetadata? periodNameColumn = table.Columns.Values
                    .Select(column => new { Column = column, Score = GetPeriodNameColumnScore(column.Name) })
                    .Where(item => item.Score > 0)
                    .OrderByDescending(item => item.Score)
                    .ThenBy(item => item.Column.ColumnId)
                    .Select(item => item.Column)
                    .FirstOrDefault();

                if (periodNameColumn == null)
                {
                    continue;
                }

                ColumnMetadata? descriptionColumn = table.Columns.Values
                    .Select(column => new { Column = column, Score = GetDescriptionColumnScore(column.Name) })
                    .Where(item => item.Score > 0)
                    .OrderByDescending(item => item.Score)
                    .ThenBy(item => item.Column.ColumnId)
                    .Select(item => item.Column)
                    .FirstOrDefault();

                List<CompanyRelationship> companyRelationships = GetCompanyRelationships(table, companyTable, companyNameColumn, companyKeys);
                int periodNameScore = GetPeriodNameColumnScore(periodNameColumn.Name);
                int descriptionBonus = descriptionColumn == null ? 0 : 1;

                foreach (CompanyRelationship relationship in companyRelationships)
                {
                    int score = (tableNameScore * 10000) + (periodNameScore * 10) + relationship.Score + descriptionBonus;
                    candidates.Add(new FinancialPeriodSchema(table, periodNameColumn.Name, descriptionColumn?.Name, relationship, score));
                }
            }

            if (candidates.Count == 0)
            {
                throw new InvalidOperationException(
                    "در پایگاه داده، جدول دوره مالی با ستون نام دوره و رابطه قابل‌تأیید به جدول " +
                    $"[{CompanySchemaName}].[{CompanyTableName}] پیدا نشد. نام جدول، ستون‌های آن و کلیدهای واقعی شرکت از متادیتای SQL Server بررسی شدند؛ " +
                    "اما شِمای پایگاه داده در مخزن پروژه موجود نیست تا نام دیگری به‌صورت قطعی فرض شود.");
            }

            int highestScore = candidates.Max(candidate => candidate.Score);
            List<FinancialPeriodSchema> bestCandidates = candidates
                .Where(candidate => candidate.Score == highestScore)
                .ToList();

            if (bestCandidates.Count > 1)
            {
                string matchingTables = string.Join(", ", bestCandidates.Select(candidate =>
                    $"[{candidate.Table.SchemaName}].[{candidate.Table.TableName}] ({candidate.Relationship.Description})"));
                throw new InvalidOperationException("چند ساختار هم‌امتیاز برای دوره مالی پیدا شد و انتخاب امن ممکن نیست: " + matchingTables);
            }

            return bestCandidates[0];
        }

        private static List<CompanyRelationship> GetCompanyRelationships(
            TableMetadata table,
            TableMetadata companyTable,
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
                CompanyRelationship relationship = new CompanyRelationship(
                    description,
                    companyKey.IsPrimaryKey ? 260 : 240,
                    description);
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
                return new List<CompanyRelationship>
                {
                    new CompanyRelationship(
                        CompanyNameColumnName,
                        200,
                        "اتصال مستقیم با نام شرکت")
                    {
                        CompanyNameColumnOnSource = directCompanyNameColumn.Name
                    }
                };
            }

            return new List<CompanyRelationship>();
        }

        private static bool AreCompatibleKeyTypes(int firstTypeId, int secondTypeId)
        {
            if (firstTypeId == secondTypeId)
            {
                return true;
            }

            bool firstIsInteger = firstTypeId is 48 or 52 or 56 or 127;
            bool secondIsInteger = secondTypeId is 48 or 52 or 56 or 127;
            if (firstIsInteger && secondIsInteger)
            {
                return true;
            }

            bool firstIsText = firstTypeId is 35 or 99 or 167 or 175 or 231 or 239;
            bool secondIsText = secondTypeId is 35 or 99 or 167 or 175 or 231 or 239;
            return firstIsText && secondIsText;
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

        private static string NormalizeName(string value)
        {
            return new string(value.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
        }

        private static SqlCommand CreatePeriodsCommand(
            SqlConnection connection,
            TableMetadata companyTable,
            ColumnMetadata companyNameColumn,
            FinancialPeriodSchema periodSchema,
            string companyName)
        {
            string periodTableName = QuoteTable(periodSchema.Table.SchemaName, periodSchema.Table.TableName);
            string periodNameExpression = $"CAST(period.{QuoteIdentifier(periodSchema.PeriodNameColumn)} AS nvarchar(4000))";
            string descriptionExpression = periodSchema.DescriptionColumn == null
                ? "CAST(NULL AS nvarchar(max))"
                : $"CAST(period.{QuoteIdentifier(periodSchema.DescriptionColumn)} AS nvarchar(max))";

            string fromClause;
            string companyNameExpression;
            string companyNameFilter;

            if (!string.IsNullOrWhiteSpace(periodSchema.Relationship.CompanyNameColumnOnSource))
            {
                string sourceCompanyName = $"period.{QuoteIdentifier(periodSchema.Relationship.CompanyNameColumnOnSource)}";
                companyNameExpression = $"CAST({sourceCompanyName} AS nvarchar(4000))";
                fromClause = $"FROM {periodTableName} AS period";
                companyNameFilter = $"CAST({sourceCompanyName} AS nvarchar(4000)) = @CompanyName";
            }
            else
            {
                if (periodSchema.Relationship.ColumnPairs.Count == 0)
                {
                    throw new InvalidOperationException("رابطه جدول دوره مالی با جدول شرکت‌ها ستون قابل‌استفاده‌ای ندارد.");
                }

                string joinCondition = string.Join(" AND ", periodSchema.Relationship.ColumnPairs.Select(pair =>
                    $"period.{QuoteIdentifier(pair.PeriodColumnName)} = company.{QuoteIdentifier(pair.CompanyColumnName)}"));
                string companyTableName = QuoteTable(companyTable.SchemaName, companyTable.TableName);
                fromClause = $"FROM {periodTableName} AS period INNER JOIN {companyTableName} AS company ON {joinCondition}";
                companyNameExpression = $"CAST(company.{QuoteIdentifier(companyNameColumn.Name)} AS nvarchar(4000))";
                companyNameFilter = $"CAST(company.{QuoteIdentifier(companyNameColumn.Name)} AS nvarchar(4000)) = @CompanyName";
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

        private void ConfigureColumns()
        {
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ColumnHeadersVisible = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;

            SetColumnHeader("RowNumber", "ردیف", 0, 12F, DataGridViewContentAlignment.MiddleCenter);
            SetColumnHeader("FinancialPeriodName", "نام دوره مالی", 1, 38F, DataGridViewContentAlignment.MiddleRight);
            SetColumnHeader("CompanyName", "نام شرکت", 2, 25F, DataGridViewContentAlignment.MiddleRight);
            SetColumnHeader("Description", "توضیحات", 3, 25F, DataGridViewContentAlignment.MiddleRight);
        }

        private void SetColumnHeader(
            string columnName,
            string headerText,
            int displayIndex,
            float fillWeight,
            DataGridViewContentAlignment alignment)
        {
            if (!dataGridView1.Columns.Contains(columnName))
            {
                return;
            }

            DataGridViewColumn column = dataGridView1.Columns[columnName];
            column.HeaderText = headerText;
            column.Visible = true;
            column.DisplayIndex = displayIndex;
            column.FillWeight = fillWeight;
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
            column.DefaultCellStyle.Alignment = alignment;
            column.DefaultCellStyle.Font = Font;
            column.HeaderCell.Style.Font = Font;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void ApplyFontToAllControls(Control parent)
        {
            parent.Font = Font;

            if (parent is DataGridView grid)
            {
                grid.Font = Font;
                grid.DefaultCellStyle.Font = Font;
                grid.AlternatingRowsDefaultCellStyle.Font = Font;
                grid.ColumnHeadersDefaultCellStyle.Font = Font;
                grid.RowHeadersDefaultCellStyle.Font = Font;
                grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (parent is ToolStrip toolStrip)
            {
                toolStrip.Font = Font;
                foreach (ToolStripItem item in toolStrip.Items)
                {
                    item.Font = Font;
                }
            }

            foreach (Control child in parent.Controls)
            {
                ApplyFontToAllControls(child);
            }
        }

        private void btnSelect_Click(object? sender, EventArgs e)
        {
            DataGridViewRow? selectedRow = GetSelectedRow();
            if (selectedRow == null)
            {
                MessageBox.Show("لطفاً یک دفتر مالی را از جدول انتخاب کنید.", "اخطار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string currentCompanyName = Properties.Settings.Default.SelectedCompany?.Trim() ?? string.Empty;
            if (!string.Equals(currentCompanyName, selectedCompanyName, StringComparison.Ordinal))
            {
                MessageBox.Show("شرکت جاری تغییر کرده است. فرم انتخاب سال مالی را دوباره باز کنید.", "انتخاب سال مالی", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CloseCurrentTab();
                return;
            }

            string financialPeriodName = GetFinancialPeriodName(selectedRow);
            if (string.IsNullOrWhiteSpace(financialPeriodName))
            {
                MessageBox.Show("عنوان دوره مالی انتخاب‌شده خالی است.", "اخطار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MainForm? targetMainForm = mainForm;
            if (targetMainForm == null || targetMainForm.IsDisposed)
            {
                foreach (Form openForm in Application.OpenForms)
                {
                    if (openForm is MainForm openMainForm)
                    {
                        targetMainForm = openMainForm;
                        break;
                    }
                }
            }

            if (targetMainForm != null && !targetMainForm.IsDisposed)
            {
                targetMainForm.SetSelectedYear(financialPeriodName);
            }

            CloseCurrentTab();
        }

        private DataGridViewRow? GetSelectedRow()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                return dataGridView1.SelectedRows[0];
            }

            if (dataGridView1.SelectedCells.Count > 0)
            {
                int rowIndex = dataGridView1.SelectedCells[0].RowIndex;
                if (rowIndex >= 0 && rowIndex < dataGridView1.Rows.Count)
                {
                    return dataGridView1.Rows[rowIndex];
                }
            }

            return null;
        }

        private string GetFinancialPeriodName(DataGridViewRow row)
        {
            if (!dataGridView1.Columns.Contains("FinancialPeriodName"))
            {
                return string.Empty;
            }

            object? value = row.Cells["FinancialPeriodName"].Value;
            return value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value)?.Trim() ?? string.Empty;
        }

        private void dataGridView1_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnSelect_Click(sender, EventArgs.Empty);
            }
        }

        private void btnCancel_Click(object? sender, EventArgs e)
        {
            CloseCurrentTab();
        }

        private void CloseCurrentTab()
        {
            if (Parent is TabPage tabPage && tabPage.Parent is TabControl tabControl)
            {
                tabControl.TabPages.Remove(tabPage);
                tabPage.Dispose();
            }
            else
            {
                Close();
            }
        }

        private sealed class ColumnMetadata
        {
            public ColumnMetadata(string name, int systemTypeId, int columnId)
            {
                Name = name;
                SystemTypeId = systemTypeId;
                ColumnId = columnId;
            }

            public string Name { get; }
            public int SystemTypeId { get; }
            public int ColumnId { get; }
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
            public List<CompanyColumnPair> ColumnPairs { get; } = new List<CompanyColumnPair>();
        }

        private sealed class FinancialPeriodSchema
        {
            public FinancialPeriodSchema(
                TableMetadata table,
                string periodNameColumn,
                string? descriptionColumn,
                CompanyRelationship relationship,
                int score)
            {
                Table = table;
                PeriodNameColumn = periodNameColumn;
                DescriptionColumn = descriptionColumn;
                Relationship = relationship;
                Score = score;
            }

            public TableMetadata Table { get; }
            public string PeriodNameColumn { get; }
            public string? DescriptionColumn { get; }
            public CompanyRelationship Relationship { get; }
            public int Score { get; }
        }
    }
}

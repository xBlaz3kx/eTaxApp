using Dapper;
using eTaxesApp.Shared.Constants;
using Microsoft.Data.Sqlite;
using SqlKata;
using LinqToDB.Mapping;
using LinqToDB;
using NLog;
using SqlKata.Compilers;
using SqlKata.Execution;


namespace eTaxesApp.Repositories.Sqlite
{
    public class FinancialRecordSqLiteRepository : IFinancialRecordRepository
    {
        private const string tableName = "FinancialRecords";
        private SqliteConnection _connection;
        private QueryFactory queryFactory;
        private NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public FinancialRecordSqLiteRepository(SqliteConnection connection)
        {
            _connection = connection;

            // Create the table if it does not exist
            string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS FinancialRecords (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Date DATETIME NOT NULL,
                Type TEXT NOT NULL,
                Amount REAL NOT NULL,
                Currency TEXT NOT NULL,
                Description TEXT
            );";
            connection.Execute(createTableQuery);

            queryFactory = new QueryFactory(connection, new SqliteCompiler());
        }

        public async Task<bool> AddRecordAsync(FinancialRecord record)
        {
            var res = await _connection.ExecuteAsync(
                "INSERT INTO FinancialRecords (Date,Amount,Type, Currency, Description)" +
                "VALUES (@Date, @Amount, @Type, @Currency, @Description);", record);

            return res == 0;
        }

        public Task<IEnumerable<FinancialRecord>> GetRecordsAsync(DateTime? from, DateTime? to,
            IEnumerable<RecordType>? types)
        {
            var query = queryFactory.Query().From(tableName);

            if (from != null)
            {
                query.Where("Date", ">=", from);
            }

            if (to != null)
            {
                query.Where("Date", "<=", to);
            }

            if (types != null && types.Any())
            {
                query.WhereIn("Type", types);
            }

            return query.OrderBy("Date").GetAsync<FinancialRecord>();
        }

        public Task<IEnumerable<SummaryRecord>> GetSummaryAsync(DateTime? from, DateTime? to,
            IEnumerable<RecordType>? types)
        {
            var query = queryFactory.Query().SelectRaw("Type as Type, SUM(`Amount`) as Amount").From(tableName);

            if (from != null)
            {
                query.Where("Date", ">=", from);
            }

            if (to != null)
            {
                query.Where("Date", "<=", to);
            }

            if (types != null && types.Any())
            {
                query.WhereIn("Type", types);
            }


            return query.GroupBy("Type").OrderBy("Type").GetAsync<SummaryRecord>();
        }
    }
};
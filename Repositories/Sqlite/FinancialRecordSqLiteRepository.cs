using Dapper;
using eTaxesApp.Shared.Constants;
using Microsoft.Data.Sqlite;
using SqlKata;
using LinqToDB.Mapping;
using LinqToDB;


namespace eTaxesApp.Repositories.Sqlite
{
    public class Db() : LinqToDB.Data.DataConnection("Data Source=eTaxes.db")
    {
        public ITable<FinancialRecord> Records => this.GetTable<FinancialRecord>();
    }

    public class FinancialRecordSqLiteRepository : IFinancialRecordRepository
    {
        private SqliteConnection _connection;
        private NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public FinancialRecordSqLiteRepository(SqliteConnection connection)
        {
            _connection = connection;
            string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS FinancialRecords (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Date TEXT NOT NULL,
                Type TEXT NOT NULL,
                Amount REAL NOT NULL,
                Currency TEXT NOT NULL,
                Description TEXT
            );";
            
            _connection.Execute(createTableQuery);
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
            var query = new Query().From("FinancialRecords");

            if (from != null)
            {
                query.Where("Date", ">=", from);
            }

            if (to != null)
            {
                query.Where("Date", "<=", to);
            }

            if (types != null)
            {
                query.WhereIn("Type", types);
            }

            return _connection.QueryAsync<FinancialRecord>(query.OrderBy("Date").ToString()!);
        }

        public Task<IEnumerable<SummaryRecord>> GetSummaryAsync(DateTime? from, DateTime? to,
            IEnumerable<RecordType>? types)
        {
            var query = new Query().Select("Type").SelectRaw("SUM(`Amount`) as Amount").From("FinancialRecords");

            if (from != null)
            {
                query.Where("Date", ">=", from);
            }

            if (to != null)
            {
                query.Where("Date", "<=", to);
            }

            if (types != null)
            {
                query.WhereIn("Type", types);
            }

            query.GroupBy("Type");

            return _connection.QueryAsync<SummaryRecord>(sql: query.OrderBy("Type").ToString()!);
        }
    }
};
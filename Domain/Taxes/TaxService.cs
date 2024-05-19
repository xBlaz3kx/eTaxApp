using eTaxesApp.Repositories;
using eTaxesApp.Shared.Constants;
using eTaxesApp.Shared.Entities;
using eTaxesApp.Shared.Exceptions;
using FinancialRecord = eTaxesApp.Shared.Entities.FinancialRecord;
using SummaryRecord = eTaxesApp.Shared.Entities.SummaryRecord;

namespace eTaxesApp.Domain.Taxes;

public interface ITaxService
{
    public Task<bool> AddRecordsAsync(IEnumerable<FinancialRecord> records);

    public Task<IEnumerable<FinancialRecord>>
        GetRecordsAsync(DateTime? from, DateTime? to, IEnumerable<RecordType> types);

    public Task<FinancialReport> CreateTaxReportAsync(DateTime? from, DateTime? to,
        IEnumerable<RecordType> types);
}

public class TaxService(IFinancialRecordRepository financialRecordRepository) : ITaxService
{
    private NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    public async Task<bool> AddRecordsAsync(IEnumerable<FinancialRecord> records)
    {
        _logger.Info("Adding records to the database.");

        try
        {
            // Add records to the database
            var tasks = records.Select(record => new Repositories.FinancialRecord
                {
                    Date = record.Date,
                    Type = RecordTypeExtensions.FromString(record.Type),
                    Amount = record.Amount,
                    Currency = record.Currency,
                    Description = record.Description,
                    // Metadata = record.Metadata
                })
                .Select(financialRecordRepository.AddRecordAsync)
                .Cast<Task>()
                .ToList();

            await Task.WhenAll(tasks);
        }
        catch (InvalidTypeException e)
        {
            _logger.Error(e, "Failed to parse the record type");
            return false;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Failed to add records to the database.");
            return false;
        }

        return true;
    }


    public async Task<IEnumerable<FinancialRecord>> GetRecordsAsync(DateTime? from, DateTime? to,
        IEnumerable<RecordType> types)
    {
        _logger.Info("Getting records from the database.");

        // Validate that the from date is not in the future 
        if (from != null && DateTime.Compare((DateTime)from, DateTime.Now) > 0)
        {
            throw new InvalidDateRangeException();
        }

        // Validate that the from date is before to date
        if (from != null && to != null && DateTime.Compare((DateTime)from, (DateTime)to) > 0)
        {
            throw new InvalidDateRangeException();
        }

        // Query the database
        var records = await financialRecordRepository.GetRecordsAsync(from, to, types);

        // Transform the database models to the domain models
        return records.Select(record => new FinancialRecord(record.Date,
            RecordTypeExtensions.ToString(record.Type),
            record.Amount,
            record.Currency,
            record.Description)
        ).ToList();
    }

    public async Task<FinancialReport> CreateTaxReportAsync(DateTime? from, DateTime? to,
        IEnumerable<RecordType> types)
    {
        _logger.Info("Creating the financial report.");

        // Validate that the from date is not in the future 
        if (from != null && DateTime.Compare((DateTime)from, DateTime.Now) > 0)
        {
            throw new InvalidDateRangeException();
        }

        // Validate that the from date is before to date
        if (from != null && to != null && DateTime.Compare((DateTime)from, (DateTime)to) > 0)
        {
            throw new InvalidDateRangeException();
        }

        // Get records from the database
        var financialRecords = await financialRecordRepository.GetSummaryAsync(from, to, types);

        var totalTaxOwed = new decimal(0.0);
        var tax = new decimal(0.0);

        // Create output list
        var financialRecordsOutput = new List<SummaryRecord>();

        // Calculate the total tax owed and tax for each type of record
        foreach (var record in financialRecords)
        {
            switch (record.Type)
            {
                case RecordType.Dividend:
                    // Dividend tax is 25%
                    tax = record.Amount * new decimal(0.25);
                    totalTaxOwed += tax;
                    break;
                case RecordType.Income:
                    // Income tax is 22%
                    tax = record.Amount * new decimal(0.22);
                    totalTaxOwed += tax;
                    break;
                case RecordType.Property:
                    // Property tax is 2%
                    tax = record.Amount * new decimal(0.02);
                    totalTaxOwed += tax;
                    break;
                case RecordType.Stock:
                    // Profit from stock is taxed at 25%
                    tax = record.Amount * new decimal(0.25);
                    totalTaxOwed += tax;

                    // todo take into account the stock holding date
                    // If the stock was held for less than five years, the tax is 25%
                    // If the stock was held from 5 to 10 years, the tax is 20%
                    // If the stock was held from 10 to 15 years, the tax is 15%
                    // If the stock was held more than 15 years, the tax is 0%

                    break;
                default:
                    break;
            }

            financialRecordsOutput.Add(new SummaryRecord(Amount: tax, Type: record.Type));
        }

        // We'll assume that everything is in EUR for now
        return new FinancialReport(financialRecordsOutput, totalTaxOwed, "EUR");
    }
}
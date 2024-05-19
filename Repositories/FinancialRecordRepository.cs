using eTaxesApp.Shared.Constants;
using LinqToDB.Mapping;

namespace eTaxesApp.Repositories;

[Table("FinancialRecords")]
public class FinancialRecord
{
    [PrimaryKey, Identity] public int Id { get; init; }

    [Column("Date"), NotNull] public DateTime Date { get; init; }

    [Column("Type"), NotNull] public RecordType Type { get; init; }

    [Column("Amount"), NotNull] public decimal Amount { get; init; }
    [Column("Currency"), NotNull] public string Currency { get; init; }

    [Column("Description")] public string Description { get; init; }

    [Column("Metadata")] public object Metadata { get; init; }
}

public record SummaryRecord(RecordType Type, decimal Amount);

public interface IFinancialRecordRepository
{
    public Task<bool> AddRecordAsync(FinancialRecord record);

    public Task<IEnumerable<FinancialRecord>> GetRecordsAsync(DateTime? from, DateTime? to,
        IEnumerable<RecordType>? types);

    public Task<IEnumerable<SummaryRecord>> GetSummaryAsync(DateTime? from, DateTime? to,
        IEnumerable<RecordType>? types);
}
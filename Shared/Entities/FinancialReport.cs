using eTaxesApp.Shared.Constants;

namespace eTaxesApp.Shared.Entities;

public record SummaryRecord
{
    public SummaryRecord(RecordType Type, decimal Amount)
    {
        this.Type = Type;
        this.Amount = Amount;
    }

    public RecordType Type { get; init; }
    public decimal Amount { get; init; }
}

public record FinancialReport(
    // List of amount of tax per type
    List<SummaryRecord> Records,
    // Total tax owed by the user
    decimal TotalTaxOwed,
    // Tax currency based on ISO 4217
    string TaxCurrency
);
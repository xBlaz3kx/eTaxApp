using eTaxesApp.Repositories;

namespace eTaxesApp.Shared.Entities;

public record FinancialReport(
    // List of amount of tax per type
    List<SummaryRecord> Records,
    // Total tax owed by the user
    decimal TotalTaxOwed,
    // Tax currency based on ISO 4217
    string TaxCurrency
);
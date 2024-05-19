using eTaxesApp.Shared.Constants;

namespace eTaxesApp.Shared.Entities;

public record FinancialRecord(
    // When a transaction was made
    DateTime Date,
    // Type of transaction
    string Type,
    // Amount of money involved
    decimal Amount,
    // Currency based on ISO 4217
    string Currency,
    // Description of the transaction
    string Description,
    // Additional information about the transaction
    object Metadata
);
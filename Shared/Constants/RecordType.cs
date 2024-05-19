using eTaxesApp.Shared.Exceptions;

namespace eTaxesApp.Shared.Constants;

public enum RecordType
{
    Income,
    Property,
    Dividend,
    Stock
}

public static class RecordTypeExtensions
{
    public static string ToString(RecordType type) =>
        type switch
        {
            RecordType.Income => "Income",
            RecordType.Property => "Property",
            RecordType.Dividend => "Dividend",
            RecordType.Stock => "Stock",
            _ => throw new InvalidTypeException()
        };

    public static RecordType FromString(string type) =>
        type switch
        {
            "Income" => RecordType.Income,
            "Property" => RecordType.Property,
            "Dividend" => RecordType.Dividend,
            "Stock" => RecordType.Stock,
            _ => throw new InvalidTypeException()
        };
}
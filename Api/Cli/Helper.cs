using eTaxesApp.Shared.Constants;

namespace eTaxesApp.API.cli;

public static class Helper
{
    public static IEnumerable<RecordType> ParseRecordTypes(IEnumerable<string>? types)
    {
        return types.Select(type => Enum.Parse<RecordType>(type));
    }
}
using CommandLine;

namespace eTaxesApp.API.cli;

[Verb("export", HelpText = "Export financial records in the requested format.")]
public class ExportCommand
{
    [Option('o', "out", Required = false, HelpText = "Destination of the file.")]
    public string? Output { get; set; }

    [Option("from", Required = false, HelpText = "Date from")]
    public DateTime? From { get; set; }

    [Option("to", Required = false, HelpText = "Date to")]
    public DateTime? To { get; set; }

    [Option("types", Required = false, Separator = ',', HelpText = "Types of data to be exported")]
    public IEnumerable<string> Types { get; set; }
}
using CommandLine;

namespace eTaxesApp.API.cli;

[Verb("calculate", HelpText = "Calculate the amount of taxes to be paid in a time period.")]
public class CalculateCommand
{
    [Value(0, Required = true, HelpText = "Destination of the file.")]
    public string Output { get; set; }

    [Option("from", Required = false, HelpText = "Date from")]
    public DateTime? From { get; set; }

    [Option("to", Required = false, HelpText = "Date to")]
    public DateTime? To { get; set; }

    [Option("types", Required = false, Separator = ',', HelpText = "Types of data to be taken into account")]
    public IEnumerable<string> Types { get; set; }
}
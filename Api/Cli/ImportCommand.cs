namespace eTaxesApp.API.cli;

using CommandLine;

[Verb("import", HelpText = "Import financial records from the given files.")]
public class ImportCommand
{
    [Option('f', "files", Required = true, Separator = ',', HelpText = "Input files to be processed.")]
    public IEnumerable<string> InputFiles { get; set; }
}
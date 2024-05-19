using CommandLine;
using eTaxesApp.API.cli;
using eTaxesApp.Domain.Taxes;
using eTaxesApp.Repositories;
using eTaxesApp.Repositories.Sqlite;
using Microsoft.Data.Sqlite;
using NLog;
using NLog.Layouts;
using SQLite;

namespace eTaxesApp;

public class App
{
    public static async Task Main(string[] args)
    {
        // Setup the logger
        var logger = SetupLogger();

        try
        {
            // Create the SQLite connection
            var sqlConnection = CreateConnection();

            // Create the repository layer
            IFinancialRecordRepository repository = new FinancialRecordSqLiteRepository(sqlConnection);

            // Create the service layer
            ITaxService service = new TaxService(repository);

            // Create CLI controller handler
            ICliCommandController controller = new CommandController(service);

            // Bind the CLI Controller to CLI parser
            Parser.Default.ParseArguments<ImportCommand, ExportCommand, CalculateCommand>(args)
                .WithParsed((ImportCommand options) => controller.Import(options.InputFiles))
                .WithParsed((ExportCommand options) => controller.Export(options.Output, options.From, options.To,
                    Helper.ParseRecordTypes(options.Types)))
                .WithParsed((CalculateCommand options) => controller.Calculate(options.Output, options.From,
                    options.To,
                    Helper.ParseRecordTypes(options.Types)));

            // Dispose the database connection
            await sqlConnection.DisposeAsync();
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to create the database connection.");
        }
    }

    private static SqliteConnection CreateConnection()
    {
        // Create the SQLite connection
        var connectionStringBuilder = new SqliteConnectionStringBuilder
        {
            DataSource = "eTax.db"
        };

        var sqlConnection = new SqliteConnection(connectionStringBuilder.ToString());
        sqlConnection.Open();

        return sqlConnection;
    }

    private static Logger SetupLogger()
    {
        // Create a logger
        var config = new NLog.Config.LoggingConfiguration();

        var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
        // Set JSON layout for logs 
        var jsonLayout = new JsonLayout
        {
            ExcludeEmptyProperties = true,
            Attributes =
            {
                new JsonAttribute("time", "${longdate}"),
                new JsonAttribute("level", "${level}"),
                new JsonAttribute("message", "${message}"),
                new JsonAttribute("properties", new JsonLayout { IncludeEventProperties = true, MaxRecursionLimit = 2 },
                    encode: false),
                new JsonAttribute("exception", new JsonLayout
                    {
                        IndentJson = true,
                        ExcludeEmptyProperties = true,
                        Attributes =
                        {
                            new JsonAttribute("type", "${exception:format=type}"),
                            new JsonAttribute("message", "${exception:format=message}"),
                            new JsonAttribute("stacktrace", "${exception:format=tostring}"),
                        }
                    },
                    encode: false) // don't escape layout
            }
        };
        logconsole.Layout = jsonLayout;

        // Rules for mapping loggers to targets            
        config.AddRule(LogLevel.Info, LogLevel.Error, logconsole);

        LogManager.Configuration = config;
        LogManager.ReconfigExistingLoggers();
        return LogManager.GetCurrentClassLogger();
    }
}
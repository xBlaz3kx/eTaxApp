using System.Collections;
using eTaxesApp.Domain.Taxes;
using eTaxesApp.Shared.Constants;
using eTaxesApp.Shared.Entities;
using Newtonsoft.Json;
using NLog;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace eTaxesApp.API.cli;

public interface ICliCommandController
{
    public void Import(IEnumerable<string> files);

    public void Export(string? output, DateTime? from, DateTime? to, IEnumerable<RecordType> types);

    public void Calculate(string output, DateTime? from, DateTime? to, IEnumerable<RecordType> types);
}

public class CommandController(ITaxService taxService) : ICliCommandController
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void Import(IEnumerable<string> files)
    {
        _logger.Info("Importing financial records from the given files.");

        IEnumerable<FinancialRecord> records = new List<FinancialRecord>();

        // Deserialize all the files specified. Any unparsed files will be skipped.
        var financialRecords = records.ToList();

        foreach (var file in files)
        {
            try
            {
                using StreamReader r = new StreamReader(file);
                string json = r.ReadToEnd();
                List<FinancialRecord> fileRecords = JsonConvert.DeserializeObject<List<FinancialRecord>>(json) ??
                                                    throw new InvalidOperationException();

                records = financialRecords.Union(fileRecords);
            }
            catch (InvalidOperationException e)
            {
                _logger.Error(e, "Failed to serialize the records");
            }
            catch (Exception e)
            {
                _logger.Error(e, "Failed to import records from file.");
            }
        }

        // Add the results using the service to the database.
        var task = taxService.AddRecordsAsync(records);
        task.Wait();

        if (task.Result)
        {
            _logger.Info("Successfully imported records.");
            return;
        }

        _logger.Error("Failed to import records.");
    }

    public async void Export(string? output, DateTime? from, DateTime? to, IEnumerable<RecordType> types)
    {
        _logger.Info("Exporting financial records.");

        try
        {
            // Get all records
            var records = await taxService.GetRecordsAsync(from, to, types);

            // Set default output file, if none provided
            if (output == null)
            {
                output = "./records.json";
            }

            // Write records to file
            string jsonString = JsonConvert.SerializeObject(records);
            File.WriteAllText(output, jsonString);
        }
        catch (Exception e)
        {
            _logger.Error(e, "Failed to export records.");
            return;
        }

        _logger.Info("Successfully exported records.");
    }

    public async void Calculate(string output, DateTime? from, DateTime? to, IEnumerable<RecordType> types)
    {
        _logger.Info("Calculating taxes.");

        try
        {
            // Create the tax report
            var reportRecords = await taxService.CreateTaxReportAsync(from, to, types);

            // Write the report to a file
            string jsonString = JsonConvert.SerializeObject(reportRecords);
            File.WriteAllText(output, jsonString);
        }
        catch (Exception e)
        {
            _logger.Error(e, "Failed to calculate taxes.");
            return;
        }

        _logger.Info("Successfully calculated taxes.");
    }
}
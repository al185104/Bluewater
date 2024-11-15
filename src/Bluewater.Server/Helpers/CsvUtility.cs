
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Reflection;

namespace Bluewater.Server.Helpers;

public static class CsvUtility
{    
    public static async Task<string> ExportToCSV<T>(List<T> records, string fileName = "export.csv", string[]? excludeColumns = null)
    {
        try
        {
            // Get the user's Downloads folder path
            var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            // Ensure the Downloads folder exists
            if (!Directory.Exists(downloadsPath))
            {
                Directory.CreateDirectory(downloadsPath);
            }

            // Combine the file name with the Downloads path
            var filePath = Path.Combine(downloadsPath, fileName);

            // Configure CsvHelper
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            };

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, config))
            {
                // Get all properties of type T
                var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => excludeColumns == null || !excludeColumns.Contains(p.Name, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

                // Write headers for the included properties
                foreach (var property in properties)
                {
                    csv.WriteField(property.Name);
                }
                await csv.NextRecordAsync();

                // Write each record's property values
                foreach (var record in records)
                {
                    foreach (var property in properties)
                    {
                        var value = property.GetValue(record);
                        csv.WriteField(value);
                    }
                    await csv.NextRecordAsync();
                }
            }

            return $"CSV file has been successfully exported to: {filePath}";
        }
        catch (Exception ex)
        {
            return $"Error exporting CSV file: {ex.Message}";
        }
    }

    public static async Task<(List<T>?, string)> ImportFromCSV<T>(string filePath) where T : new()
    {
        try
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null, // Ignore header validation errors
                MissingFieldFound = null // Ignore missing fields errors
            };

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                // Read and validate headers
                csv.Read();
                csv.ReadHeader();
                var headers = csv.HeaderRecord;

                // Verify headers match properties of T
                var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var propertyNames = properties.Select(p => p.Name).ToHashSet(StringComparer.InvariantCultureIgnoreCase);

                // Check if each header exists as a property
                if (headers != null && !headers.All(h => propertyNames.Contains(h)))
                {
                    return (null, "CSV headers do not match the properties of the target type.");
                }

                // Read records
                var records = new List<T>();
                while (await csv.ReadAsync())
                {
                    var record = new T();
                    foreach (var property in properties)
                    {
                        if (csv.TryGetField(property.PropertyType, property.Name, out var value))
                        {
                            property.SetValue(record, value);
                        }
                    }
                    records.Add(record);
                }

                return (records, "CSV file has been successfully imported.");
            }
        }
        catch (Exception ex)
        {
            return (null, $"Error importing CSV file: {ex.Message}");
        }
    }
}

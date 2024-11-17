
using Bluewater.UseCases.Employees;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Expressions;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace Bluewater.Server.Helpers;

public static class CsvUtility
{    
    /*
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
    */

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
                // Flatten the properties of the type and nested classes
                var properties = FlattenProperties(typeof(T))
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
                    if(record == null) continue;
                    
                    foreach (var property in properties)
                    {
                        var value = GetPropertyValue(record, property);
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

    public static List<EmployeeImportDTO> ImportEmployeesFromCsv(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        csv.Context.RegisterClassMap<EmployeeImportDTOMap>();
        return csv.GetRecords<EmployeeImportDTO>().ToList();
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

                if (headers == null)
                {
                    return (null, "CSV file does not contain headers.");
                }

                // Map CSV headers to properties using attributes or property names
                var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var headerToPropertyMap = properties
                    .Select(p => new
                    {
                        Property = p,
                        Header = p.GetCustomAttribute<DisplayAttribute>()?.Name ?? p.Name
                    })
                    .ToDictionary(x => x.Header, x => x.Property, StringComparer.InvariantCultureIgnoreCase);

                // Validate that all headers exist in the mapping
                if (!headers.All(h => headerToPropertyMap.ContainsKey(h)))
                {
                    var missingHeaders = headers.Where(h => !headerToPropertyMap.ContainsKey(h));
                    return (null, $"CSV headers do not match the properties of the target type. Missing headers: {string.Join(", ", missingHeaders)}");
                }

                // Read records
                var records = new List<T>();
                while (await csv.ReadAsync())
                {
                    var record = new T();
                    foreach (var header in headers)
                    {
                        if (headerToPropertyMap.TryGetValue(header, out var property))
                        {
                            if (csv.TryGetField(property.PropertyType, header, out var value))
                            {
                                property.SetValue(record, value);
                            }
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

    // public static async Task<(List<T>?, string)> ImportFromCSV<T>(string filePath) where T : new()
    // {
    //     try
    //     {
    //         var config = new CsvConfiguration(CultureInfo.InvariantCulture)
    //         {
    //             HeaderValidated = null, // Ignore header validation errors
    //             MissingFieldFound = null // Ignore missing fields errors
    //         };

    //         using (var reader = new StreamReader(filePath))
    //         using (var csv = new CsvReader(reader, config))
    //         {
    //             // Read and validate headers
    //             csv.Read();
    //             csv.ReadHeader();
    //             var headers = csv.HeaderRecord;

    //             // Verify headers match properties of T
    //             var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
    //             var propertyNames = properties.Select(p => p.Name).ToHashSet(StringComparer.InvariantCultureIgnoreCase);

    //             // Check if each header exists as a property
    //             if (headers != null && !headers.All(h => propertyNames.Contains(h)))
    //             {
    //                 return (null, "CSV headers do not match the properties of the target type.");
    //             }

    //             // Read records
    //             var records = new List<T>();
    //             while (await csv.ReadAsync())
    //             {
    //                 var record = new T();
    //                 foreach (var property in properties)
    //                 {
    //                     if (csv.TryGetField(property.PropertyType, property.Name, out var value))
    //                     {
    //                         property.SetValue(record, value);
    //                     }
    //                 }
    //                 records.Add(record);
    //             }

    //             return (records, "CSV file has been successfully imported.");
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         return (null, $"Error importing CSV file: {ex.Message}");
    //     }
    // }


    private static IEnumerable<PropertyInfo> FlattenProperties(Type type, string parentName = "")
    {
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            // Check if the property is a complex type (not a system type)
            if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
            {
                // Recursively flatten nested properties
                foreach (var subProperty in FlattenProperties(property.PropertyType, $"{parentName}{property.Name}."))
                {
                    yield return subProperty;
                }
            }
            else
            {
                // Add regular properties
                yield return new PropertyInfoWrapper(parentName + property.Name, property);
            }
        }
    }

    private static object? GetPropertyValue(object obj, PropertyInfo property)
    {
        try
        {
            return property.GetValue(obj);
        }
        catch
        {
            return null;
        }
    }

    private class PropertyInfoWrapper : PropertyInfo
    {
        private readonly string _name;
        private readonly PropertyInfo _originalProperty;

        public PropertyInfoWrapper(string name, PropertyInfo originalProperty)
        {
            _name = name;
            _originalProperty = originalProperty;
        }

        public override string Name => _name;

        public override Type PropertyType => _originalProperty.PropertyType;

        public override bool CanRead => _originalProperty.CanRead;

        public override bool CanWrite => _originalProperty.CanWrite;

        public override PropertyAttributes Attributes => _originalProperty.Attributes;

        public override Type? DeclaringType => _originalProperty.DeclaringType;

        public override Type? ReflectedType => _originalProperty.ReflectedType;


        public override bool IsDefined(Type attributeType, bool inherit) => _originalProperty.IsDefined(attributeType, inherit);

        public override object? GetValue(object? obj, BindingFlags invokeAttr, Binder? binder, object?[]? index, CultureInfo? culture)
        {
        return _originalProperty.GetValue(obj, invokeAttr, binder, index, culture);
        }

        public override void SetValue(object? obj, object? value, BindingFlags invokeAttr, Binder? binder, object?[]? index, CultureInfo? culture)
        {
        _originalProperty.SetValue(obj, value, invokeAttr, binder, index, culture);
        }

        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
        return _originalProperty.GetAccessors(nonPublic);
        }

        public override MethodInfo? GetGetMethod(bool nonPublic)
        {
        return _originalProperty.GetGetMethod(nonPublic);
        }

        public override MethodInfo? GetSetMethod(bool nonPublic)
        {
        return _originalProperty.GetSetMethod(nonPublic);
        }

        public override ParameterInfo[] GetIndexParameters()
        {
        return _originalProperty.GetIndexParameters();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
        return _originalProperty.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
        return _originalProperty.GetCustomAttributes(attributeType, inherit);
        }

    }

}
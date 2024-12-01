using Bluewater.UseCases.Payrolls;
using CsvHelper;
using CsvHelper.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace Bluewater.Server.Helpers;

public static class CsvUtility
{    
    public static void ExportPayrollToCSV(string fileName, IEnumerable<PayrollDTO> payrolls)
    {
        var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        // Ensure the Downloads folder exists
        if (!Directory.Exists(downloadsPath))
            Directory.CreateDirectory(downloadsPath);

        // Combine the file name with the Downloads path
        var filePath = Path.Combine(downloadsPath, fileName);

        // Group payrolls by Division, then Department, then Section
        var groupedPayrolls = payrolls
            .GroupBy(p => p.Division)
            .Select(divisionGroup => new
            {
                Division = divisionGroup.Key,
                Departments = divisionGroup.GroupBy(d => d.Department)
            });

        // Open a CSV file for writing
        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        #region headers
        // Write the updated header
        csv.WriteField("Division");
        csv.WriteField("Department");
        csv.WriteField("Section");
        csv.WriteField("Employee Name");
        csv.WriteField("Labor Hours");
        csv.WriteField("Labor Hours Income");
        csv.WriteField("Regular Holiday Hours");
        csv.WriteField("Regular Holiday Amount");
        csv.WriteField("Special Holiday Hours");
        csv.WriteField("Special Holiday Amount");
        csv.WriteField("Night Differential Hours");
        csv.WriteField("Night Differential Amount");
        csv.WriteField("Night Diff Regular Holiday Hours");
        csv.WriteField("Night Diff Regular Holiday Amount");
        csv.WriteField("Night Diff Special Holiday Hours");
        csv.WriteField("Night Diff Special Holiday Amount");
        csv.WriteField("Pagibig Amount");
        csv.WriteField("Philhealth Amount");
        csv.WriteField("Union Dues");
        csv.WriteField("Lates");
        csv.WriteField("Lates Amount");
        csv.WriteField("Undertime");
        csv.WriteField("Undertime Amount");
        csv.WriteField("Overbreak");
        csv.WriteField("Overbreak Amount");
        csv.WriteField("Service Charge");
        csv.WriteField("Monthly Allowance Amount");
        csv.WriteField("Tax Deductions");
        csv.WriteField("Total Constant Deductions");
        csv.WriteField("Total Loan Deductions");
        csv.WriteField("Gross Pay Amount");
        csv.WriteField("Total Deductions");
        csv.WriteField("Net Amount");
        csv.NextRecord();
        #endregion

        string? lastDivision = null;
        string? lastDepartment = null;
        string? lastSection = null;

        #region Grand total variables
        // Initialize grand totals
        decimal grandTotalLaborHrs = 0;
        decimal grandTotalLaborHoursIncome = 0;
        decimal grandTotalRegularHolidayHrs = 0;
        decimal grandTotalRegularHolidayAmount = 0;
        decimal grandTotalSpecialHolidayHrs = 0;
        decimal grandTotalSpecialHolidayAmount = 0;
        decimal grandTotalNightDiffHrs = 0;
        decimal grandTotalNightDiffAmount = 0;
        decimal grandTotalNightDiffRegularHolidayHrs = 0;
        decimal grandTotalNightDiffRegularHolidayAmount = 0;
        decimal grandTotalNightDiffSpecialHolidayHrs = 0;
        decimal grandTotalNightDiffSpecialHolidayAmount = 0;
        decimal grandTotalPagibigAmount = 0;
        decimal grandTotalPhilhealthAmount = 0;
        decimal grandTotalUnionDues = 0;
        decimal grandTotalLates = 0;
        decimal grandTotalLatesAmount = 0;
        decimal grandTotalUndertime = 0;
        decimal grandTotalUndertimeAmount = 0;
        decimal grandTotalOverbreak = 0;
        decimal grandTotalOverbreakAmount = 0;
        decimal grandTotalSvcCharge = 0;
        decimal grandTotalMonthlyAllowanceAmount = 0;
        decimal grandTotalTaxDeductions = 0;
        decimal grandTotalTotalConstantDeductions = 0;
        decimal grandTotalTotalLoanDeductions = 0;
        decimal grandTotalGrossPay = 0;
        decimal grandTotalTotalDeductions = 0;
        decimal grandTotalNetPay = 0;
        #endregion

        // Write data and division totals
        foreach (var divisionGroup in groupedPayrolls)
        {
            # region local variable initialized
            // Initialize totals for the division
            decimal divisionTotalGrossPay = 0;
            decimal divisionTotalNetPay = 0;
            decimal divisionTotalLaborHrs = 0;
            decimal divisionTotalLaborHoursIncome = 0;
            decimal divisionTotalRegularHolidayHrs = 0;
            decimal divisionTotalRegularHolidayAmount = 0;
            decimal divisionTotalSpecialHolidayHrs = 0;
            decimal divisionTotalSpecialHolidayAmount = 0;
            decimal divisionTotalNightDiffHrs = 0;
            decimal divisionTotalNightDiffAmount = 0;
            decimal divisionTotalNightDiffRegularHolidayHrs = 0;
            decimal divisionTotalNightDiffRegularHolidayAmount = 0;
            decimal divisionTotalNightDiffSpecialHolidayHrs = 0;
            decimal divisionTotalNightDiffSpecialHolidayAmount = 0;
            decimal divisionTotalPagibigAmount = 0;
            decimal divisionTotalPhilhealthAmount = 0;
            decimal divisionTotalUnionDues = 0;
            decimal divisionTotalLates = 0;
            decimal divisionTotalLatesAmount = 0;
            decimal divisionTotalUndertime = 0;
            decimal divisionTotalUndertimeAmount = 0;
            decimal divisionTotalOverbreak = 0;
            decimal divisionTotalOverbreakAmount = 0;
            decimal divisionTotalSvcCharge = 0;
            decimal divisionTotalMonthlyAllowanceAmount = 0;
            decimal divisionTotalTaxDeductions = 0;
            decimal divisionTotalTotalConstantDeductions = 0;
            decimal divisionTotalTotalLoanDeductions = 0;
            decimal divisionTotalTotalDeductions = 0;
            #endregion

            foreach (var departmentGroup in divisionGroup.Departments)
            {
                foreach (var sectionGroup in departmentGroup.GroupBy(s => s.Section))
                {
                    foreach (var payroll in sectionGroup)
                    {
                        if (lastDivision != divisionGroup.Division)
                        {
                            csv.WriteField(divisionGroup.Division);
                            lastDivision = divisionGroup.Division;
                        }
                        else
                            csv.WriteField("");

                        if (lastDepartment != departmentGroup.Key)
                        {
                            csv.WriteField(departmentGroup.Key);
                            lastDepartment = departmentGroup.Key;
                        }
                        else
                            csv.WriteField("");

                        if (lastSection != sectionGroup.Key)
                        {
                            csv.WriteField(sectionGroup.Key);
                            lastSection = sectionGroup.Key;
                        }
                        else
                            csv.WriteField("");

                        csv.WriteField(payroll.Name);
                        csv.WriteField(payroll.LaborHrs);
                        csv.WriteField(payroll.LaborHoursIncome);
                        csv.WriteField(payroll.RegularHolidayHrs);
                        csv.WriteField(payroll.RegularHolidayAmount);
                        csv.WriteField(payroll.SpecialHolidayHrs);
                        csv.WriteField(payroll.SpecialHolidayAmount);
                        csv.WriteField(payroll.NightDiffHrs);
                        csv.WriteField(payroll.NightDiffAmount);
                        csv.WriteField(payroll.NightDiffRegularHolidayHrs);
                        csv.WriteField(payroll.NightDiffRegularHolidayAmount);
                        csv.WriteField(payroll.NightDiffSpecialHolidayHrs);
                        csv.WriteField(payroll.NightDiffSpecialHolidayAmount);
                        csv.WriteField(payroll.PagibigAmount);
                        csv.WriteField(payroll.PhilhealthAmount);
                        csv.WriteField(payroll.UnionDues);
                        csv.WriteField(payroll.Lates);
                        csv.WriteField(payroll.LatesAmount);
                        csv.WriteField(payroll.Undertime);
                        csv.WriteField(payroll.UndertimeAmount);
                        csv.WriteField(payroll.Overbreak);
                        csv.WriteField(payroll.OverbreakAmount);
                        csv.WriteField(payroll.SvcCharge);
                        csv.WriteField(payroll.MonthlyAllowanceAmount);
                        
                        // todo: individual loan deductions here

                        csv.WriteField(payroll.TaxDeductions);
                        csv.WriteField(payroll.TotalConstantDeductions);
                        csv.WriteField(payroll.TotalLoanDeductions);
                        csv.WriteField(payroll.GrossPayAmount);
                        csv.WriteField(payroll.TotalDeductions);
                        csv.WriteField(payroll.NetAmount);                                                                                                
                        csv.NextRecord();

                        // Add all totals for each column
                        divisionTotalLaborHrs += payroll.LaborHrs;
                        divisionTotalLaborHoursIncome += payroll.LaborHoursIncome;
                        divisionTotalRegularHolidayHrs += payroll.RegularHolidayHrs;
                        divisionTotalRegularHolidayAmount += payroll.RegularHolidayAmount;
                        divisionTotalSpecialHolidayHrs += payroll.SpecialHolidayHrs;
                        divisionTotalSpecialHolidayAmount += payroll.SpecialHolidayAmount;
                        divisionTotalNightDiffHrs += payroll.NightDiffHrs;
                        divisionTotalNightDiffAmount += payroll.NightDiffAmount;
                        divisionTotalNightDiffRegularHolidayHrs += payroll.NightDiffRegularHolidayHrs;
                        divisionTotalNightDiffRegularHolidayAmount += payroll.NightDiffRegularHolidayAmount;
                        divisionTotalNightDiffSpecialHolidayHrs += payroll.NightDiffSpecialHolidayHrs;
                        divisionTotalNightDiffSpecialHolidayAmount += payroll.NightDiffSpecialHolidayAmount;
                        divisionTotalPagibigAmount += payroll.PagibigAmount;
                        divisionTotalPhilhealthAmount += payroll.PhilhealthAmount;
                        divisionTotalUnionDues += payroll.UnionDues;
                        divisionTotalLates += payroll.Lates;
                        divisionTotalLatesAmount += payroll.LatesAmount;
                        divisionTotalUndertime += payroll.Undertime;
                        divisionTotalUndertimeAmount += payroll.UndertimeAmount;
                        divisionTotalOverbreak += payroll.Overbreak;
                        divisionTotalOverbreakAmount += payroll.OverbreakAmount;
                        divisionTotalSvcCharge += payroll.SvcCharge;
                        divisionTotalMonthlyAllowanceAmount += payroll.MonthlyAllowanceAmount;
                        divisionTotalTaxDeductions += payroll.TaxDeductions;
                        divisionTotalTotalConstantDeductions += payroll.TotalConstantDeductions;
                        divisionTotalTotalLoanDeductions += payroll.TotalLoanDeductions;
                        divisionTotalGrossPay += payroll.GrossPayAmount;
                        divisionTotalTotalDeductions += payroll.TotalDeductions;
                        divisionTotalNetPay += payroll.NetAmount;

                        #region grand total
                        grandTotalLaborHrs += payroll.LaborHrs;
                        grandTotalLaborHoursIncome += payroll.LaborHoursIncome;
                        grandTotalRegularHolidayHrs += payroll.RegularHolidayHrs;
                        grandTotalRegularHolidayAmount += payroll.RegularHolidayAmount;
                        grandTotalSpecialHolidayHrs += payroll.SpecialHolidayHrs;
                        grandTotalSpecialHolidayAmount += payroll.SpecialHolidayAmount;
                        grandTotalNightDiffHrs += payroll.NightDiffHrs;
                        grandTotalNightDiffAmount += payroll.NightDiffAmount;
                        grandTotalNightDiffRegularHolidayHrs += payroll.NightDiffRegularHolidayHrs;
                        grandTotalNightDiffRegularHolidayAmount += payroll.NightDiffRegularHolidayAmount;
                        grandTotalNightDiffSpecialHolidayHrs += payroll.NightDiffSpecialHolidayHrs;
                        grandTotalNightDiffSpecialHolidayAmount += payroll.NightDiffSpecialHolidayAmount;
                        grandTotalPagibigAmount += payroll.PagibigAmount;
                        grandTotalPhilhealthAmount += payroll.PhilhealthAmount;
                        grandTotalUnionDues += payroll.UnionDues;
                        grandTotalLates += payroll.Lates;
                        grandTotalLatesAmount += payroll.LatesAmount;
                        grandTotalUndertime += payroll.Undertime;
                        grandTotalUndertimeAmount += payroll.UndertimeAmount;
                        grandTotalOverbreak += payroll.Overbreak;
                        grandTotalOverbreakAmount += payroll.OverbreakAmount;
                        grandTotalSvcCharge += payroll.SvcCharge;
                        grandTotalMonthlyAllowanceAmount += payroll.MonthlyAllowanceAmount;
                        grandTotalTaxDeductions += payroll.TaxDeductions;
                        grandTotalTotalConstantDeductions += payroll.TotalConstantDeductions;
                        grandTotalTotalLoanDeductions += payroll.TotalLoanDeductions;
                        grandTotalGrossPay += payroll.GrossPayAmount;
                        grandTotalTotalDeductions += payroll.TotalDeductions;
                        grandTotalNetPay += payroll.NetAmount;
                        #endregion

                    }
                }
            }

            #region totals
            // Write the division totals row
            csv.WriteField($"{divisionGroup.Division}"); 
            csv.WriteField("Total"); // Indicate that this is a total row
            csv.WriteField(""); // Empty for Section
            csv.WriteField(""); // Empty for Employee Name
            csv.WriteField(divisionTotalLaborHrs);
            csv.WriteField(divisionTotalLaborHoursIncome);
            csv.WriteField(divisionTotalRegularHolidayHrs);
            csv.WriteField(divisionTotalRegularHolidayAmount);
            csv.WriteField(divisionTotalSpecialHolidayHrs);
            csv.WriteField(divisionTotalSpecialHolidayAmount);
            csv.WriteField(divisionTotalNightDiffHrs);
            csv.WriteField(divisionTotalNightDiffAmount);
            csv.WriteField(divisionTotalNightDiffRegularHolidayHrs);
            csv.WriteField(divisionTotalNightDiffRegularHolidayAmount);
            csv.WriteField(divisionTotalNightDiffSpecialHolidayHrs);
            csv.WriteField(divisionTotalNightDiffSpecialHolidayAmount);
            csv.WriteField(divisionTotalPagibigAmount);
            csv.WriteField(divisionTotalPhilhealthAmount);
            csv.WriteField(divisionTotalUnionDues);
            csv.WriteField(divisionTotalLates);
            csv.WriteField(divisionTotalLatesAmount);
            csv.WriteField(divisionTotalUndertime);
            csv.WriteField(divisionTotalUndertimeAmount);
            csv.WriteField(divisionTotalOverbreak);
            csv.WriteField(divisionTotalOverbreakAmount);
            csv.WriteField(divisionTotalSvcCharge);
            csv.WriteField(divisionTotalMonthlyAllowanceAmount);
            csv.WriteField(divisionTotalTaxDeductions);
            csv.WriteField(divisionTotalTotalConstantDeductions);
            csv.WriteField(divisionTotalTotalLoanDeductions);
            csv.WriteField(divisionTotalGrossPay);
            csv.WriteField(divisionTotalTotalDeductions);
            csv.WriteField(divisionTotalNetPay);
            csv.NextRecord();
            #endregion
        }

        // Add an empty row before the grand total
        csv.NextRecord(); // Write an empty line

        // Write the grand totals row (styled as red for Excel visualization)
        csv.WriteField("Grand Total"); // Style marker for Excel
        csv.WriteField(""); // Empty for Department
        csv.WriteField(""); // Empty for Section
        csv.WriteField(""); // Empty for Employee Name
        csv.WriteField(grandTotalLaborHrs);
        csv.WriteField(grandTotalLaborHoursIncome);
        csv.WriteField(grandTotalRegularHolidayHrs);
        csv.WriteField(grandTotalRegularHolidayAmount);
        csv.WriteField(grandTotalSpecialHolidayHrs);
        csv.WriteField(grandTotalSpecialHolidayAmount);
        csv.WriteField(grandTotalNightDiffHrs);
        csv.WriteField(grandTotalNightDiffAmount);
        csv.WriteField(grandTotalNightDiffRegularHolidayHrs);
        csv.WriteField(grandTotalNightDiffRegularHolidayAmount);
        csv.WriteField(grandTotalNightDiffSpecialHolidayHrs);
        csv.WriteField(grandTotalNightDiffSpecialHolidayAmount);
        csv.WriteField(grandTotalPagibigAmount);
        csv.WriteField(grandTotalPhilhealthAmount);
        csv.WriteField(grandTotalUnionDues);
        csv.WriteField(grandTotalLates);
        csv.WriteField(grandTotalLatesAmount);
        csv.WriteField(grandTotalUndertime);
        csv.WriteField(grandTotalUndertimeAmount);
        csv.WriteField(grandTotalOverbreak);
        csv.WriteField(grandTotalOverbreakAmount);
        csv.WriteField(grandTotalSvcCharge);
        csv.WriteField(grandTotalMonthlyAllowanceAmount);
        csv.WriteField(grandTotalTaxDeductions);
        csv.WriteField(grandTotalTotalConstantDeductions);
        csv.WriteField(grandTotalTotalLoanDeductions);
        csv.WriteField(grandTotalGrossPay);
        csv.WriteField(grandTotalTotalDeductions);
        csv.WriteField(grandTotalNetPay);
        csv.NextRecord();
    }    
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
                                // Trim strings before setting the property
                                if (property.PropertyType == typeof(string) && value is string stringValue)
                                {
                                    value = stringValue.Trim();
                                }
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
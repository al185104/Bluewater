using Bluewater.UseCases.Payrolls;
using CsvHelper;
using CsvHelper.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using ClosedXML.Excel;

namespace Bluewater.Server.Helpers;

public static class CsvUtility
{
    public static void ExportPayrollToExcel(string fileName, IEnumerable<PayrollDTO> payrolls)
    {
        var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        // Ensure the Downloads folder exists
        if (!Directory.Exists(downloadsPath))
            Directory.CreateDirectory(downloadsPath);

        // Combine the file name with the Downloads path
        var filePath = Path.Combine(downloadsPath, fileName);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Payroll");

        // Define headers
        var headers = new[]
        {
            "Division", "Department", "Section", "Barcode", "Employee Name", "Bank Account",
            "Labor Hours", "Labor Hours Income", "Regular Holiday Hours", "Regular Holiday Amount",
            "Special Holiday Hours", "Special Holiday Amount", "Night Differential Hours", "Night Differential Amount",
            "Night Diff Regular Holiday Hours", "Night Diff Regular Holiday Amount",
            "Night Diff Special Holiday Hours", "Night Diff Special Holiday Amount",
            "Pagibig Amount", "Philhealth Amount", "Union Dues", "Lates", "Lates Amount",
            "Undertime", "Undertime Amount", "Overbreak", "Overbreak Amount",
            "Service Charge", "Monthly Allowance Amount", "Tax Deductions",
            "Total Constant Deductions", "Total Loan Deductions",
            "Gross Pay Amount", "Total Deductions", "Net Amount"
        };

        // Write headers
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        int currentRow = 2;
        var sortedPayrolls = payrolls
        .OrderBy(p => p.Division)
        .ThenBy(p => p.Department)
        .ThenBy(p => p.Section)
        .ToList();

        // Initialize grand totals
        decimal grandTotalLaborHrs = 0, grandTotalLaborHoursIncome = 0, grandTotalRegularHolidayHrs = 0,
                grandTotalRegularHolidayAmount = 0, grandTotalSpecialHolidayHrs = 0, grandTotalSpecialHolidayAmount = 0,
                grandTotalNightDiffHrs = 0, grandTotalNightDiffAmount = 0, grandTotalNightDiffRegularHolidayHrs = 0,
                grandTotalNightDiffRegularHolidayAmount = 0, grandTotalNightDiffSpecialHolidayHrs = 0,
                grandTotalNightDiffSpecialHolidayAmount = 0, grandTotalPagibigAmount = 0, grandTotalPhilhealthAmount = 0,
                grandTotalUnionDues = 0, grandTotalLates = 0, grandTotalLatesAmount = 0, grandTotalUndertime = 0,
                grandTotalUndertimeAmount = 0, grandTotalOverbreak = 0, grandTotalOverbreakAmount = 0, grandTotalSvcCharge = 0,
                grandTotalMonthlyAllowanceAmount = 0, grandTotalTaxDeductions = 0, grandTotalTotalConstantDeductions = 0,
                grandTotalTotalLoanDeductions = 0, grandTotalGrossPay = 0, grandTotalTotalDeductions = 0, grandTotalNetPay = 0;

        string? lastDivision = string.Empty;
        string? lastDepartment = string.Empty;
        string? lastSection = string.Empty;

        foreach (var divisionGroup in sortedPayrolls.GroupBy(p => p.Division))
        {
            // Initialize division totals
            decimal divisionTotalLaborHrs = 0, divisionTotalLaborHoursIncome = 0, divisionTotalRegularHolidayHrs = 0,
                    divisionTotalRegularHolidayAmount = 0, divisionTotalSpecialHolidayHrs = 0, divisionTotalSpecialHolidayAmount = 0,
                    divisionTotalNightDiffHrs = 0, divisionTotalNightDiffAmount = 0, divisionTotalNightDiffRegularHolidayHrs = 0,
                    divisionTotalNightDiffRegularHolidayAmount = 0, divisionTotalNightDiffSpecialHolidayHrs = 0,
                    divisionTotalNightDiffSpecialHolidayAmount = 0, divisionTotalPagibigAmount = 0, divisionTotalPhilhealthAmount = 0,
                    divisionTotalUnionDues = 0, divisionTotalLates = 0, divisionTotalLatesAmount = 0, divisionTotalUndertime = 0,
                    divisionTotalUndertimeAmount = 0, divisionTotalOverbreak = 0, divisionTotalOverbreakAmount = 0, divisionTotalSvcCharge = 0,
                    divisionTotalMonthlyAllowanceAmount = 0, divisionTotalTaxDeductions = 0, divisionTotalTotalConstantDeductions = 0,
                    divisionTotalTotalLoanDeductions = 0, divisionTotalGrossPay = 0, divisionTotalTotalDeductions = 0, divisionTotalNetPay = 0;

            foreach (var payroll in divisionGroup)
            {
                // Write payroll data
                worksheet.Cell(currentRow, 1).Value = lastDivision != divisionGroup.Key ? divisionGroup.Key : "";
                worksheet.Cell(currentRow, 2).Value = lastDepartment != payroll.Department ? payroll.Department : "";
                worksheet.Cell(currentRow, 3).Value = lastSection != payroll.Section ? payroll.Section : "";
                if(!string.IsNullOrEmpty(divisionGroup.Key) && !lastDivision!.Equals(divisionGroup.Key, StringComparison.InvariantCultureIgnoreCase))
                    lastDivision = divisionGroup.Key;
                if(!string.IsNullOrEmpty(payroll.Department) && !lastDepartment!.Equals(payroll.Department, StringComparison.InvariantCultureIgnoreCase))
                    lastDepartment = payroll.Department;
                if(!string.IsNullOrEmpty(payroll.Section) && !lastSection!.Equals(payroll.Section, StringComparison.InvariantCultureIgnoreCase))
                    lastSection = payroll.Section;

                worksheet.Cell(currentRow, 4).Value = payroll.Barcode;
                worksheet.Cell(currentRow, 5).Value = payroll.Name;
                worksheet.Cell(currentRow, 6).Value = payroll.BankAccount;
                worksheet.Cell(currentRow, 7).Value = payroll.LaborHrs;
                worksheet.Cell(currentRow, 8).Value = payroll.LaborHoursIncome.ToString("N2");
                worksheet.Cell(currentRow, 9).Value = payroll.RegularHolidayHrs;
                worksheet.Cell(currentRow, 10).Value = payroll.RegularHolidayAmount.ToString("N2");
                worksheet.Cell(currentRow, 11).Value = payroll.SpecialHolidayHrs;
                worksheet.Cell(currentRow, 12).Value = payroll.SpecialHolidayAmount.ToString("N2");
                worksheet.Cell(currentRow, 13).Value = payroll.NightDiffHrs;
                worksheet.Cell(currentRow, 14).Value = payroll.NightDiffAmount.ToString("N2");
                worksheet.Cell(currentRow, 15).Value = payroll.NightDiffRegularHolidayHrs;
                worksheet.Cell(currentRow, 16).Value = payroll.NightDiffRegularHolidayAmount.ToString("N2");
                worksheet.Cell(currentRow, 17).Value = payroll.NightDiffSpecialHolidayHrs;
                worksheet.Cell(currentRow, 18).Value = payroll.NightDiffSpecialHolidayAmount.ToString("N2");
                worksheet.Cell(currentRow, 19).Value = payroll.PagibigAmount.ToString("N2");
                worksheet.Cell(currentRow, 20).Value = payroll.PhilhealthAmount.ToString("N2");
                worksheet.Cell(currentRow, 21).Value = payroll.UnionDues.ToString("N2");
                worksheet.Cell(currentRow, 22).Value = payroll.Lates;
                worksheet.Cell(currentRow, 23).Value = payroll.LatesAmount.ToString("N2");
                worksheet.Cell(currentRow, 24).Value = payroll.Undertime;
                worksheet.Cell(currentRow, 25).Value = payroll.UndertimeAmount.ToString("N2");
                worksheet.Cell(currentRow, 26).Value = payroll.Overbreak;
                worksheet.Cell(currentRow, 27).Value = payroll.OverbreakAmount.ToString("N2");
                worksheet.Cell(currentRow, 28).Value = payroll.SvcCharge;
                worksheet.Cell(currentRow, 29).Value = payroll.MonthlyAllowanceAmount.ToString("N2");
                worksheet.Cell(currentRow, 30).Value = payroll.TaxDeductions.ToString("N2");
                worksheet.Cell(currentRow, 31).Value = payroll.TotalConstantDeductions.ToString("N2");
                worksheet.Cell(currentRow, 32).Value = payroll.TotalLoanDeductions.ToString("N2");
                worksheet.Cell(currentRow, 33).Value = payroll.GrossPayAmount.ToString("N2");
                worksheet.Cell(currentRow, 34).Value = payroll.TotalDeductions.ToString("N2");
                worksheet.Cell(currentRow, 35).Value = payroll.NetAmount.ToString("N2");

                // Update totals
                #region division groups
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
                #endregion

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

                currentRow++;
            }

            // Division total
            worksheet.Cell(currentRow, 1).Value = $"{divisionGroup.Key} Total";
            worksheet.Row(currentRow).Style.Font.Bold = true;
            worksheet.Row(currentRow).Style.Fill.BackgroundColor = XLColor.LightGreen;

            // Write division totals
            worksheet.Cell(currentRow, 7).Value = divisionTotalLaborHrs;
            worksheet.Cell(currentRow, 8).Value = divisionTotalLaborHoursIncome.ToString("N2");
            worksheet.Cell(currentRow, 9).Value = divisionTotalRegularHolidayHrs;
            worksheet.Cell(currentRow, 10).Value = divisionTotalRegularHolidayAmount.ToString("N2");
            worksheet.Cell(currentRow, 11).Value = divisionTotalSpecialHolidayHrs;
            worksheet.Cell(currentRow, 12).Value = divisionTotalSpecialHolidayAmount.ToString("N2");
            worksheet.Cell(currentRow, 13).Value = divisionTotalNightDiffHrs;
            worksheet.Cell(currentRow, 14).Value = divisionTotalNightDiffAmount.ToString("N2");
            worksheet.Cell(currentRow, 15).Value = divisionTotalNightDiffRegularHolidayHrs;
            worksheet.Cell(currentRow, 16).Value = divisionTotalNightDiffRegularHolidayAmount.ToString("N2");
            worksheet.Cell(currentRow, 17).Value = divisionTotalNightDiffSpecialHolidayHrs;
            worksheet.Cell(currentRow, 18).Value = divisionTotalNightDiffSpecialHolidayAmount.ToString("N2");
            worksheet.Cell(currentRow, 19).Value = divisionTotalPagibigAmount.ToString("N2");
            worksheet.Cell(currentRow, 20).Value = divisionTotalPhilhealthAmount.ToString("N2");
            worksheet.Cell(currentRow, 21).Value = divisionTotalUnionDues.ToString("N2");
            worksheet.Cell(currentRow, 22).Value = divisionTotalLates;
            worksheet.Cell(currentRow, 23).Value = divisionTotalLatesAmount.ToString("N2");
            worksheet.Cell(currentRow, 24).Value = divisionTotalUndertime;
            worksheet.Cell(currentRow, 25).Value = divisionTotalUndertimeAmount.ToString("N2");
            worksheet.Cell(currentRow, 26).Value = divisionTotalOverbreak;
            worksheet.Cell(currentRow, 27).Value = divisionTotalOverbreakAmount.ToString("N2");
            worksheet.Cell(currentRow, 28).Value = divisionTotalSvcCharge.ToString("N2");
            worksheet.Cell(currentRow, 29).Value = divisionTotalMonthlyAllowanceAmount.ToString("N2");
            worksheet.Cell(currentRow, 30).Value = divisionTotalTaxDeductions.ToString("N2");
            worksheet.Cell(currentRow, 31).Value = divisionTotalTotalConstantDeductions.ToString("N2");
            worksheet.Cell(currentRow, 32).Value = divisionTotalTotalLoanDeductions.ToString("N2");
            worksheet.Cell(currentRow, 33).Value = divisionTotalGrossPay.ToString("N2");
            worksheet.Cell(currentRow, 34).Value = divisionTotalTotalDeductions.ToString("N2");
            worksheet.Cell(currentRow, 35).Value = divisionTotalNetPay.ToString("N2");

            currentRow++;
        }

        currentRow++; // Empty row before grand total

        // Grand total
        worksheet.Cell(currentRow, 1).Value = "Grand Total";
        worksheet.Row(currentRow).Style.Font.Bold = true;
        worksheet.Row(currentRow).Style.Fill.BackgroundColor = XLColor.LightPink;

        // Define a mapping of column indices to grand total values
        var grandTotals = new (int Column, decimal Value)[]
        {
            (7, grandTotalLaborHrs),
            (8, grandTotalLaborHoursIncome),
            (9, grandTotalRegularHolidayHrs),
            (10, grandTotalRegularHolidayAmount),
            (11, grandTotalSpecialHolidayHrs),
            (12, grandTotalSpecialHolidayAmount),
            (13, grandTotalNightDiffHrs),
            (14, grandTotalNightDiffAmount),
            (15, grandTotalNightDiffRegularHolidayHrs),
            (16, grandTotalNightDiffRegularHolidayAmount),
            (17, grandTotalNightDiffSpecialHolidayHrs),
            (18, grandTotalNightDiffSpecialHolidayAmount),
            (19, grandTotalPagibigAmount),
            (20, grandTotalPhilhealthAmount),
            (21, grandTotalUnionDues),
            (22, grandTotalLates),
            (23, grandTotalLatesAmount),
            (24, grandTotalUndertime),
            (25, grandTotalUndertimeAmount),
            (26, grandTotalOverbreak),
            (27, grandTotalOverbreakAmount),
            (28, grandTotalSvcCharge),
            (29, grandTotalMonthlyAllowanceAmount),
            (30, grandTotalTaxDeductions),
            (31, grandTotalTotalConstantDeductions),
            (32, grandTotalTotalLoanDeductions),
            (33, grandTotalGrossPay),
            (34, grandTotalTotalDeductions),
            (35, grandTotalNetPay)
        };        

        // Loop through the mapping and set values or hide columns
        foreach (var (Column, Value) in grandTotals)
        {
            if (Value != 0)
                worksheet.Cell(currentRow, Column).Value = Value.ToString("N2");
            else
                worksheet.Column(Column).Hide();
        }
        // Save to file
        workbook.SaveAs(filePath);
    }

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
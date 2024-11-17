using Bluewater.UseCases.Employees;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

public class EmployeeImportDTOMap : ClassMap<EmployeeImportDTO>
{
    public EmployeeImportDTOMap()
    {
        AutoMap(CultureInfo.InvariantCulture);

        // Map headers to properties explicitly
        Map(m => m.EmployeeID).Name("Employee ID");
        Map(m => m.LastName).Name("Last Name");
        Map(m => m.FirstName).Name("First Name");
        Map(m => m.MiddleName).Name("Middle Name");
        Map(m => m.BirthDate).Name("Birth Date");
        Map(m => m.DepartmentCode).Name("Department Code");
        Map(m => m.DepartmentDescription).Name("Department");
        Map(m => m.ChargingCode).Name("Charging Code");
        Map(m => m.ChargingDescription).Name("Charging");
        Map(m => m.DivisionCode).Name("Division Code");
        Map(m => m.DivisionDescription).Name("Division");
        Map(m => m.SectionCode).Name("Section Code");
        Map(m => m.SectionDescription).Name("Section");
        Map(m => m.PayCode).Name("Pay Code");
        Map(m => m.Address).Name("Address");
        Map(m => m.DateHired).Name("Date Hired");
        Map(m => m.Status).Name("Status");
        Map(m => m.EmployeeType).Name("Employee Type");
        Map(m => m.Rank).Name("Rank");
        Map(m => m.Gender).Name("Gender");
        Map(m => m.CivilStatus).Name("Civil Status");
        Map(m => m.Position).Name("Position");
        Map(m => m.SSSNo).Name("SSS No.");
        Map(m => m.TINNo).Name("TIN No.");
        Map(m => m.PagIbigID).Name("Pag-ibig ID");
        Map(m => m.BankAccount).Name("Bank Acct.");
        Map(m => m.PhilHealthID).Name("PhilHealth ID");
        Map(m => m.CellNo).Name("Cell No.");
        Map(m => m.MonthlyRate).Name("Monthly Rate");
    }
}

namespace Bluewater.UseCases.Employees;
using System.ComponentModel.DataAnnotations;

public class EmployeeImportDTO
{
    [Display(Name = "Employee ID")]
    public string EmployeeID { get; set; } = string.Empty;

    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Display(Name = "Middle Name")]
    public string MiddleName { get; set; } = string.Empty;

    [Display(Name = "Birth Date")]
    public DateTime? BirthDate { get; set; }

    [Display(Name = "Department")]
    public string Department { get; set; } = string.Empty;

    [Display(Name = "Department Name")]
    public string DepartmentName { get; set; } = string.Empty;

    [Display(Name = "Division Name")]
    public string DivisionName { get; set; } = string.Empty;

    [Display(Name = "Section Name")]
    public string SectionName { get; set; } = string.Empty;

    [Display(Name = "Pay Code")]
    public string PayCode { get; set; } = string.Empty;

    [Display(Name = "Address")]
    public string Address { get; set; } = string.Empty;

    [Display(Name = "Date Hired")]
    public DateTime? DateHired { get; set; }

    [Display(Name = "Status")]
    public string Status { get; set; } = string.Empty;

    [Display(Name = "Employee Type")]
    public string EmployeeType { get; set; } = string.Empty;

    [Display(Name = "Rank")]
    public string Rank { get; set; } = string.Empty;

    [Display(Name = "Gender")]
    public string Gender { get; set; } = string.Empty;

    [Display(Name = "Civil Status")]
    public string CivilStatus { get; set; } = string.Empty;

    [Display(Name = "Position")]
    public string Position { get; set; } = string.Empty;

    [Display(Name = "SSS No.")]
    public string SSSNo { get; set; } = string.Empty;

    [Display(Name = "TIN No.")]
    public string TINNo { get; set; } = string.Empty;

    [Display(Name = "Pag-ibig ID")]
    public string PagIbigID { get; set; } = string.Empty;

    [Display(Name = "Bank Acct.")]
    public string BankAccount { get; set; } = string.Empty;

    [Display(Name = "PhilHealth ID")]
    public string PhilHealthID { get; set; } = string.Empty;

    [Display(Name = "Cell No.")]
    public string CellNo { get; set; } = string.Empty;

    [Display(Name = "Monthly Rate")]
    public decimal? MonthlyRate { get; set; }
}

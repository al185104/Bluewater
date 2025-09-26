using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Payrolls;

public class CreatePayrollRequest
{
  public const string Route = "/Payrolls";

  [Required]
  public Guid EmployeeId { get; set; }

  [Required]
  public DateOnly Date { get; set; }

  [Range(0, double.MaxValue)]
  public decimal GrossPayAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal NetAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal BasicPayAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal SssAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal SssERAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal PagibigAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal PagibigERAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal PhilhealthAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal PhilhealthERAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal RestDayAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal RestDayHrs { get; set; }
  [Range(0, double.MaxValue)]
  public decimal RegularHolidayAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal RegularHolidayHrs { get; set; }
  [Range(0, double.MaxValue)]
  public decimal SpecialHolidayAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal SpecialHolidayHrs { get; set; }
  [Range(0, double.MaxValue)]
  public decimal OvertimeAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal OvertimeHrs { get; set; }
  [Range(0, double.MaxValue)]
  public decimal NightDiffAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal NightDiffHrs { get; set; }
  [Range(0, double.MaxValue)]
  public decimal NightDiffOvertimeAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal NightDiffOvertimeHrs { get; set; }
  [Range(0, double.MaxValue)]
  public decimal NightDiffRegularHolidayAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal NightDiffRegularHolidayHrs { get; set; }
  [Range(0, double.MaxValue)]
  public decimal NightDiffSpecialHolidayAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal NightDiffSpecialHolidayHrs { get; set; }
  [Range(0, double.MaxValue)]
  public decimal OvertimeRestDayAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal OvertimeRestDayHrs { get; set; }
  [Range(0, double.MaxValue)]
  public decimal OvertimeRegularHolidayAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal OvertimeRegularHolidayHrs { get; set; }
  [Range(0, double.MaxValue)]
  public decimal OvertimeSpecialHolidayAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal OvertimeSpecialHolidayHrs { get; set; }
  [Range(0, double.MaxValue)]
  public decimal UnionDues { get; set; }
  [Range(0, int.MaxValue)]
  public int Absences { get; set; }
  [Range(0, double.MaxValue)]
  public decimal AbsencesAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal Leaves { get; set; }
  [Range(0, double.MaxValue)]
  public decimal LeavesAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal Lates { get; set; }
  [Range(0, double.MaxValue)]
  public decimal LatesAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal Undertime { get; set; }
  [Range(0, double.MaxValue)]
  public decimal UndertimeAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal Overbreak { get; set; }
  [Range(0, double.MaxValue)]
  public decimal OverbreakAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal SvcCharge { get; set; }
  [Range(0, double.MaxValue)]
  public decimal CostOfLivingAllowanceAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal MonthlyAllowanceAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal SalaryUnderpaymentAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal RefundAbsencesAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal RefundUndertimeAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal RefundOvertimeAmount { get; set; }
  [Range(0, double.MaxValue)]
  public decimal LaborHoursIncome { get; set; }
  [Range(0, double.MaxValue)]
  public decimal LaborHrs { get; set; }
  [Range(0, double.MaxValue)]
  public decimal TaxDeductions { get; set; }
  [Range(0, double.MaxValue)]
  public decimal TotalConstantDeductions { get; set; }
  [Range(0, double.MaxValue)]
  public decimal TotalLoanDeductions { get; set; }
  [Range(0, double.MaxValue)]
  public decimal TotalDeductions { get; set; }
}

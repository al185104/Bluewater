using System;
using System.Collections.Generic;

namespace Bluewater.App.Models;

public class PayrollListResponseDto
{
  public List<PayrollDto?> Payrolls { get; set; } = new();
  public int TotalCount { get; set; }
}

public class PayrollGroupedListResponseDto
{
  public List<PayrollSummaryDto?> Payrolls { get; set; } = new();
}

public class PayrollDto
{
  public Guid Id { get; set; }
  public Guid? EmployeeId { get; set; }
  public string? Name { get; set; }
  public string? Barcode { get; set; }
  public string? BankAccount { get; set; }
  public DateOnly Date { get; set; }
  public string? Division { get; set; }
  public string? Department { get; set; }
  public string? Section { get; set; }
  public string? Position { get; set; }
  public string? Charging { get; set; }
  public decimal GrossPayAmount { get; set; }
  public decimal NetAmount { get; set; }
  public decimal BasicPayAmount { get; set; }
  public decimal SssAmount { get; set; }
  public decimal SssERAmount { get; set; }
  public decimal PagibigAmount { get; set; }
  public decimal PagibigERAmount { get; set; }
  public decimal PhilhealthAmount { get; set; }
  public decimal PhilhealthERAmount { get; set; }
  public decimal RestDayAmount { get; set; }
  public decimal RestDayHrs { get; set; }
  public decimal RegularHolidayAmount { get; set; }
  public decimal RegularHolidayHrs { get; set; }
  public decimal SpecialHolidayAmount { get; set; }
  public decimal SpecialHolidayHrs { get; set; }
  public decimal OvertimeAmount { get; set; }
  public decimal OvertimeHrs { get; set; }
  public decimal NightDiffAmount { get; set; }
  public decimal NightDiffHrs { get; set; }
  public decimal NightDiffOvertimeAmount { get; set; }
  public decimal NightDiffOvertimeHrs { get; set; }
  public decimal NightDiffRegularHolidayAmount { get; set; }
  public decimal NightDiffRegularHolidayHrs { get; set; }
  public decimal NightDiffSpecialHolidayAmount { get; set; }
  public decimal NightDiffSpecialHolidayHrs { get; set; }
  public decimal OvertimeRestDayAmount { get; set; }
  public decimal OvertimeRestDayHrs { get; set; }
  public decimal OvertimeRegularHolidayAmount { get; set; }
  public decimal OvertimeRegularHolidayHrs { get; set; }
  public decimal OvertimeSpecialHolidayAmount { get; set; }
  public decimal OvertimeSpecialHolidayHrs { get; set; }
  public decimal UnionDues { get; set; }
  public int Absences { get; set; }
  public decimal AbsencesAmount { get; set; }
  public decimal Leaves { get; set; }
  public decimal LeavesAmount { get; set; }
  public decimal Lates { get; set; }
  public decimal LatesAmount { get; set; }
  public decimal Undertime { get; set; }
  public decimal UndertimeAmount { get; set; }
  public decimal Overbreak { get; set; }
  public decimal OverbreakAmount { get; set; }
  public decimal SvcCharge { get; set; }
  public decimal CostOfLivingAllowanceAmount { get; set; }
  public decimal MonthlyAllowanceAmount { get; set; }
  public decimal SalaryUnderpaymentAmount { get; set; }
  public decimal RefundAbsencesAmount { get; set; }
  public decimal RefundUndertimeAmount { get; set; }
  public decimal RefundOvertimeAmount { get; set; }
  public decimal LaborHoursIncome { get; set; }
  public decimal LaborHrs { get; set; }
  public decimal TaxDeductions { get; set; }
  public decimal TotalConstantDeductions { get; set; }
  public decimal TotalLoanDeductions { get; set; }
  public decimal TotalDeductions { get; set; }
}

public class PayrollSummaryDto
{
  public DateOnly Date { get; set; }
  public int Count { get; set; }
  public decimal TotalServiceCharge { get; set; }
  public int TotalAbsences { get; set; }
  public decimal TotalLeaves { get; set; }
  public decimal TotalLates { get; set; }
  public decimal TotalUndertimes { get; set; }
  public decimal TotalOverbreak { get; set; }
  public decimal TotalTaxDeductions { get; set; }
  public decimal TotalNetAmount { get; set; }
}

public class CreatePayrollRequestDto
{
  public const string Route = "Payrolls";

  public Guid EmployeeId { get; set; }
  public DateOnly Date { get; set; }
  public decimal GrossPayAmount { get; set; }
  public decimal NetAmount { get; set; }
  public decimal BasicPayAmount { get; set; }
  public decimal SssAmount { get; set; }
  public decimal SssERAmount { get; set; }
  public decimal PagibigAmount { get; set; }
  public decimal PagibigERAmount { get; set; }
  public decimal PhilhealthAmount { get; set; }
  public decimal PhilhealthERAmount { get; set; }
  public decimal RestDayAmount { get; set; }
  public decimal RestDayHrs { get; set; }
  public decimal RegularHolidayAmount { get; set; }
  public decimal RegularHolidayHrs { get; set; }
  public decimal SpecialHolidayAmount { get; set; }
  public decimal SpecialHolidayHrs { get; set; }
  public decimal OvertimeAmount { get; set; }
  public decimal OvertimeHrs { get; set; }
  public decimal NightDiffAmount { get; set; }
  public decimal NightDiffHrs { get; set; }
  public decimal NightDiffOvertimeAmount { get; set; }
  public decimal NightDiffOvertimeHrs { get; set; }
  public decimal NightDiffRegularHolidayAmount { get; set; }
  public decimal NightDiffRegularHolidayHrs { get; set; }
  public decimal NightDiffSpecialHolidayAmount { get; set; }
  public decimal NightDiffSpecialHolidayHrs { get; set; }
  public decimal OvertimeRestDayAmount { get; set; }
  public decimal OvertimeRestDayHrs { get; set; }
  public decimal OvertimeRegularHolidayAmount { get; set; }
  public decimal OvertimeRegularHolidayHrs { get; set; }
  public decimal OvertimeSpecialHolidayAmount { get; set; }
  public decimal OvertimeSpecialHolidayHrs { get; set; }
  public decimal UnionDues { get; set; }
  public int Absences { get; set; }
  public decimal AbsencesAmount { get; set; }
  public decimal Leaves { get; set; }
  public decimal LeavesAmount { get; set; }
  public decimal Lates { get; set; }
  public decimal LatesAmount { get; set; }
  public decimal Undertime { get; set; }
  public decimal UndertimeAmount { get; set; }
  public decimal Overbreak { get; set; }
  public decimal OverbreakAmount { get; set; }
  public decimal SvcCharge { get; set; }
  public decimal CostOfLivingAllowanceAmount { get; set; }
  public decimal MonthlyAllowanceAmount { get; set; }
  public decimal SalaryUnderpaymentAmount { get; set; }
  public decimal RefundAbsencesAmount { get; set; }
  public decimal RefundUndertimeAmount { get; set; }
  public decimal RefundOvertimeAmount { get; set; }
  public decimal LaborHoursIncome { get; set; }
  public decimal LaborHrs { get; set; }
  public decimal TaxDeductions { get; set; }
  public decimal TotalConstantDeductions { get; set; }
  public decimal TotalLoanDeductions { get; set; }
  public decimal TotalDeductions { get; set; }
}

public class CreatePayrollResponseDto
{
  public Guid PayrollId { get; set; }
}

public class UpdatePayrollRequestDto
{
  public Guid PayrollId { get; set; }
  public Guid Id { get; set; }
  public Guid EmployeeId { get; set; }
  public DateOnly Date { get; set; }
  public decimal GrossPayAmount { get; set; }
  public decimal NetAmount { get; set; }
  public decimal BasicPayAmount { get; set; }
  public decimal SssAmount { get; set; }
  public decimal SssERAmount { get; set; }
  public decimal PagibigAmount { get; set; }
  public decimal PagibigERAmount { get; set; }
  public decimal PhilhealthAmount { get; set; }
  public decimal PhilhealthERAmount { get; set; }
  public decimal RestDayAmount { get; set; }
  public decimal RestDayHrs { get; set; }
  public decimal RegularHolidayAmount { get; set; }
  public decimal RegularHolidayHrs { get; set; }
  public decimal SpecialHolidayAmount { get; set; }
  public decimal SpecialHolidayHrs { get; set; }
  public decimal OvertimeAmount { get; set; }
  public decimal OvertimeHrs { get; set; }
  public decimal NightDiffAmount { get; set; }
  public decimal NightDiffHrs { get; set; }
  public decimal NightDiffOvertimeAmount { get; set; }
  public decimal NightDiffOvertimeHrs { get; set; }
  public decimal NightDiffRegularHolidayAmount { get; set; }
  public decimal NightDiffRegularHolidayHrs { get; set; }
  public decimal NightDiffSpecialHolidayAmount { get; set; }
  public decimal NightDiffSpecialHolidayHrs { get; set; }
  public decimal OvertimeRestDayAmount { get; set; }
  public decimal OvertimeRestDayHrs { get; set; }
  public decimal OvertimeRegularHolidayAmount { get; set; }
  public decimal OvertimeRegularHolidayHrs { get; set; }
  public decimal OvertimeSpecialHolidayAmount { get; set; }
  public decimal OvertimeSpecialHolidayHrs { get; set; }
  public decimal UnionDues { get; set; }
  public int Absences { get; set; }
  public decimal AbsencesAmount { get; set; }
  public decimal Leaves { get; set; }
  public decimal LeavesAmount { get; set; }
  public decimal Lates { get; set; }
  public decimal LatesAmount { get; set; }
  public decimal Undertime { get; set; }
  public decimal UndertimeAmount { get; set; }
  public decimal Overbreak { get; set; }
  public decimal OverbreakAmount { get; set; }
  public decimal SvcCharge { get; set; }
  public decimal CostOfLivingAllowanceAmount { get; set; }
  public decimal MonthlyAllowanceAmount { get; set; }
  public decimal SalaryUnderpaymentAmount { get; set; }
  public decimal RefundAbsencesAmount { get; set; }
  public decimal RefundUndertimeAmount { get; set; }
  public decimal RefundOvertimeAmount { get; set; }
  public decimal LaborHoursIncome { get; set; }
  public decimal LaborHrs { get; set; }
  public decimal TaxDeductions { get; set; }
  public decimal TotalConstantDeductions { get; set; }
  public decimal TotalLoanDeductions { get; set; }
  public decimal TotalDeductions { get; set; }

  public static string BuildRoute(Guid payrollId) => $"Payrolls/{payrollId}";
}

public class UpdatePayrollResponseDto
{
  public PayrollDto? Payroll { get; set; }
}

public class PayrollSummary : IRowIndexed
{
  public Guid Id { get; set; }
  public Guid? EmployeeId { get; set; }
  public string? Name { get; set; }
  public string? Barcode { get; set; }
  public string? BankAccount { get; set; }
  public DateOnly Date { get; set; }
  public string? Division { get; set; }
  public string? Department { get; set; }
  public string? Section { get; set; }
  public string? Position { get; set; }
  public string? Charging { get; set; }
  public decimal GrossPayAmount { get; set; }
  public decimal NetAmount { get; set; }
  public decimal BasicPayAmount { get; set; }
  public decimal SssAmount { get; set; }
  public decimal SssERAmount { get; set; }
  public decimal PagibigAmount { get; set; }
  public decimal PagibigERAmount { get; set; }
  public decimal PhilhealthAmount { get; set; }
  public decimal PhilhealthERAmount { get; set; }
  public decimal RestDayAmount { get; set; }
  public decimal RestDayHrs { get; set; }
  public decimal RegularHolidayAmount { get; set; }
  public decimal RegularHolidayHrs { get; set; }
  public decimal SpecialHolidayAmount { get; set; }
  public decimal SpecialHolidayHrs { get; set; }
  public decimal OvertimeAmount { get; set; }
  public decimal OvertimeHrs { get; set; }
  public decimal NightDiffAmount { get; set; }
  public decimal NightDiffHrs { get; set; }
  public decimal NightDiffOvertimeAmount { get; set; }
  public decimal NightDiffOvertimeHrs { get; set; }
  public decimal NightDiffRegularHolidayAmount { get; set; }
  public decimal NightDiffRegularHolidayHrs { get; set; }
  public decimal NightDiffSpecialHolidayAmount { get; set; }
  public decimal NightDiffSpecialHolidayHrs { get; set; }
  public decimal OvertimeRestDayAmount { get; set; }
  public decimal OvertimeRestDayHrs { get; set; }
  public decimal OvertimeRegularHolidayAmount { get; set; }
  public decimal OvertimeRegularHolidayHrs { get; set; }
  public decimal OvertimeSpecialHolidayAmount { get; set; }
  public decimal OvertimeSpecialHolidayHrs { get; set; }
  public decimal UnionDues { get; set; }
  public int Absences { get; set; }
  public decimal AbsencesAmount { get; set; }
  public decimal Leaves { get; set; }
  public decimal LeavesAmount { get; set; }
  public decimal Lates { get; set; }
  public decimal LatesAmount { get; set; }
  public decimal Undertime { get; set; }
  public decimal UndertimeAmount { get; set; }
  public decimal Overbreak { get; set; }
  public decimal OverbreakAmount { get; set; }
  public decimal SvcCharge { get; set; }
  public decimal CostOfLivingAllowanceAmount { get; set; }
  public decimal MonthlyAllowanceAmount { get; set; }
  public decimal SalaryUnderpaymentAmount { get; set; }
  public decimal RefundAbsencesAmount { get; set; }
  public decimal RefundUndertimeAmount { get; set; }
  public decimal RefundOvertimeAmount { get; set; }
  public decimal LaborHoursIncome { get; set; }
  public decimal LaborHrs { get; set; }
  public decimal TaxDeductions { get; set; }
  public decimal TotalConstantDeductions { get; set; }
  public decimal TotalLoanDeductions { get; set; }
  public decimal TotalDeductions { get; set; }
  public int RowIndex { get; set; }
}

public class PayrollGroupedSummary
{
  public DateOnly Date { get; set; }
  public int Count { get; set; }
  public decimal TotalServiceCharge { get; set; }
  public int TotalAbsences { get; set; }
  public decimal TotalLeaves { get; set; }
  public decimal TotalLates { get; set; }
  public decimal TotalUndertimes { get; set; }
  public decimal TotalOverbreak { get; set; }
  public decimal TotalTaxDeductions { get; set; }
  public decimal TotalNetAmount { get; set; }
}

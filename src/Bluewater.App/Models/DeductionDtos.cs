using System.ComponentModel;

namespace Bluewater.App.Models;

public class DeductionListResponseDto
{
  public List<DeductionDto?> Deductions { get; set; } = new();
}

public class DeductionDto
{
  public Guid Id { get; set; }
  public Guid? EmpId { get; set; }
  public string Name { get; set; } = string.Empty;
  public DeductionTypeDto? Type { get; set; } = DeductionTypeDto.NotSet;
  public decimal? TotalAmount { get; set; }
  public decimal? MonthlyAmortization { get; set; }
  public decimal? RemainingBalance { get; set; }
  public int? NoOfMonths { get; set; }
  public DateTime? StartDate { get; set; }
  public DateTime? EndDate { get; set; }
  public string? Remarks { get; set; }
  public ApplicationStatusDto? Status { get; set; } = ApplicationStatusDto.NotSet;
}

public class CreateDeductionRequestDto
{
  public const string Route = "Deductions";

  public Guid EmpId { get; set; }
  public DeductionTypeDto? Type { get; set; }
  public decimal? TotalAmount { get; set; }
  public decimal? MonthlyAmortization { get; set; }
  public decimal? RemainingBalance { get; set; }
  public int? NoOfMonths { get; set; }
  public DateTime? StartDate { get; set; }
  public DateTime? EndDate { get; set; }
  public string? Remarks { get; set; }
}

public class CreateDeductionResponseDto
{
  public DeductionDto? Deduction { get; set; }
}

public class DeductionSummary : IRowIndexed
{
  public Guid Id { get; set; }
  public Guid? EmpId { get; set; }
  public string Name { get; set; } = string.Empty;
  public DeductionTypeDto? Type { get; set; } = DeductionTypeDto.NotSet;
  public decimal? TotalAmount { get; set; }
  public decimal? MonthlyAmortization { get; set; }
  public decimal? RemainingBalance { get; set; }
  public int? NoOfMonths { get; set; }
  public DateTime? StartDate { get; set; }
  public DateTime? EndDate { get; set; }
  public string? Remarks { get; set; }
  public ApplicationStatusDto? Status { get; set; } = ApplicationStatusDto.NotSet;
  public int RowIndex { get; set; }
}

public class DeductionTypeOption
{
  public DeductionTypeDto Value { get; init; }
  public string Description { get; init; } = string.Empty;
}

public enum DeductionTypeDto
{
  [Description("Not Set")]
  NotSet = 0,
  [Description("SSS Salary Loan")]
  SssSalaryLoan,
  [Description("SSS Calamity Loan")]
  SssCalamityLoan,
  [Description("HDMF Multi-Purpose Loan")]
  HdmfMultiPurposeLoan,
  [Description("HDMF Calamity Loan")]
  HdmfCalamityLoan,
  [Description("RCBC Loan")]
  Rcbc,
  [Description("Hospitalization")]
  Hospitalization,
  [Description("HDMF MP2 Contribution")]
  Mp2Contribution,
  [Description("Cash Advance")]
  CashAdvance,
  [Description("Union Dues")]
  UnionDues,
  [Description("Salary Overpayment")]
  SalaryOverpayment,
  [Description("Eyeglasses")]
  Eyeglasses,
  [Description("Overtime Overpayment")]
  OvertimeOverpayment,
  [Description("Others")]
  Others
}

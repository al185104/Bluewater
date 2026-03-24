using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.Web.Deductions;

public record DeductionRecord(
  Guid Id,
  Guid? EmpId,
  string Name,
  DeductionsTypeDTO? Type,
  decimal? TotalAmount,
  decimal? MonthlyAmortization,
  decimal? RemainingBalance,
  int? NoOfMonths,
  DateTime? StartDate,
  DateTime? EndDate,
  string? Remarks,
  ApplicationStatusDTO? Status);

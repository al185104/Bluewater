using Bluewater.UseCases.Forms.Deductions;

namespace Bluewater.Web.Deductions;

internal static class DeductionMapper
{
  public static DeductionRecord ToRecord(DeductionDTO dto)
  {
    return new DeductionRecord(
      dto.Id,
      dto.EmpId,
      dto.Name ?? string.Empty,
      dto.Type,
      dto.TotalAmount,
      dto.MonthlyAmortization,
      dto.RemainingBalance,
      dto.NoOfMonths,
      dto.StartDate?.ToDateTime(TimeOnly.MinValue),
      dto.EndDate?.ToDateTime(TimeOnly.MinValue),
      dto.Remarks,
      dto.Status);
  }
}

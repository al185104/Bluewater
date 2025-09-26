namespace Bluewater.Web.Pays;

public record PayRecord(
  Guid Id,
  decimal? BasicPay,
  decimal? DailyRate,
  decimal? HourlyRate,
  decimal? HdmfEmployeeContribution,
  decimal? HdmfEmployerContribution,
  decimal? Cola);

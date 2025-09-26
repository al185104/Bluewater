namespace Bluewater.Web.ServiceCharges;

public record ServiceChargeRecord(
  Guid Id,
  string Username,
  decimal Amount,
  DateOnly Date);

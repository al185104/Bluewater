namespace Bluewater.App.Models;

public sealed class ServiceChargeSummary
{
  public Guid Id { get; set; }
  public string Username { get; set; } = string.Empty;
  public decimal Amount { get; set; }
  public DateOnly Date { get; set; }
}

public sealed class ServiceChargeRecordDto
{
  public Guid Id { get; set; }
  public string Username { get; set; } = string.Empty;
  public decimal Amount { get; set; }
  public DateOnly Date { get; set; }
}

public sealed class ServiceChargeListResponseDto
{
  public List<ServiceChargeRecordDto> ServiceCharges { get; set; } = [];
}

public sealed class CreateServiceChargeRequestDto
{
  public const string Route = "ServiceCharges";

  public string Username { get; set; } = string.Empty;
  public decimal Amount { get; set; }
  public DateOnly Date { get; set; }
}

public sealed class CreateServiceChargeResponseDto
{
  public ServiceChargeRecordDto? ServiceCharge { get; set; }
}

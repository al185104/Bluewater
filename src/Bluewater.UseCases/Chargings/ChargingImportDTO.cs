namespace Bluewater.UseCases.Chargings;
public record ChargingImportDTO() : ChargingDTO
{
  public string Department { get; set; } = null!;
}

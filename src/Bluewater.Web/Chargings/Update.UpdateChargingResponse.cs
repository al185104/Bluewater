namespace Bluewater.Web.Chargings;

public class UpdateChargingResponse(ChargingRecord Charging)
{
  public ChargingRecord Charging { get; set; } = Charging;
}

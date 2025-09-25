namespace Bluewater.Web.Chargings;

public class GetChargingByIdRequest
{
  public const string Route = "/Chargings/{ChargingId:guid}";

  public Guid ChargingId { get; set; }
}

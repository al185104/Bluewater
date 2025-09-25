namespace Bluewater.Web.Chargings;

public class DeleteChargingRequest
{
  public const string Route = "/Chargings/{ChargingId:guid}";

  public Guid ChargingId { get; set; }
}

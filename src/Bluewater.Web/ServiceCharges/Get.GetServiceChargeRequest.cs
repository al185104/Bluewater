namespace Bluewater.Web.ServiceCharges;

public class GetServiceChargeRequest
{
  public const string Route = "/ServiceCharges/{ServiceChargeId:guid}";
  public static string BuildRoute(Guid serviceChargeId) => Route.Replace("{ServiceChargeId:guid}", serviceChargeId.ToString());

  public Guid ServiceChargeId { get; set; }
}

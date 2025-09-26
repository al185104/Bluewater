namespace Bluewater.Web.ServiceCharges;

public class DeleteServiceChargeRequest
{
  public const string Route = "/ServiceCharges/{ServiceChargeId:guid}";
  public static string BuildRoute(Guid serviceChargeId) => Route.Replace("{ServiceChargeId:guid}", serviceChargeId.ToString());

  public Guid ServiceChargeId { get; set; }
}

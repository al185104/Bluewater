namespace Bluewater.Web.Pays;

public class GetPayRequest
{
  public const string Route = "/Pays/{PayId:guid}";
  public static string BuildRoute(Guid payId) => Route.Replace("{PayId:guid}", payId.ToString());

  public Guid PayId { get; set; }
}

namespace Bluewater.Web.Divisions;

public class DeleteDivisionRequest
{
  public const string Route = "/Divisions/{DivisionId:guid}";
  public static string BuildRoute(Guid divisionId) => Route.Replace("{DivisionId:guid}", divisionId.ToString());

  public Guid DivisionId { get; set; }
}

namespace Bluewater.Web.Positions;

public class DeletePositionRequest
{
  public const string Route = "/Positions/{PositionId:guid}";
  public static string BuildRoute(Guid positionId) => Route.Replace("{PositionId:guid}", positionId.ToString());

  public Guid PositionId { get; set; }
}

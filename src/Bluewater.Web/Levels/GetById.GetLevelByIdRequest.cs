namespace Bluewater.Web.Levels;

public class GetLevelByIdRequest
{
  public const string Route = "/Levels/{LevelId:guid}";
  public static string BuildRoute(Guid levelId) => Route.Replace("{LevelId:guid}", levelId.ToString());

  public Guid LevelId { get; set; }
}

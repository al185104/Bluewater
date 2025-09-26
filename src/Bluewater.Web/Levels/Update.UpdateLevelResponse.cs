namespace Bluewater.Web.Levels;

public class UpdateLevelResponse(LevelRecord Level)
{
  public LevelRecord Level { get; set; } = Level;
}

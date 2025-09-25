namespace Bluewater.Web.Positions;

public class UpdatePositionResponse(PositionRecord position)
{
  public PositionRecord Position { get; set; } = position;
}

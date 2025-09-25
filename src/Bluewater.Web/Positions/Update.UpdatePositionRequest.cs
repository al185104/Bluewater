using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Positions;

public class UpdatePositionRequest
{
  public const string Route = "/Positions/{PositionId:guid}";
  public static string BuildRoute(Guid positionId) => Route.Replace("{PositionId:guid}", positionId.ToString());

  public Guid PositionId { get; set; }

  [Required]
  public Guid Id { get; set; }

  [Required]
  public string? Name { get; set; }
  public string? Description { get; set; }

  [Required]
  public Guid SectionId { get; set; }
}

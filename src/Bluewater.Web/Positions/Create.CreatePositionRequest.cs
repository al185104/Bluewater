using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Positions;

public class CreatePositionRequest
{
  public const string Route = "/Positions";

  [Required]
  public string? Name { get; set; }
  public string? Description { get; set; }

  [Required]
  public Guid SectionId { get; set; }
}

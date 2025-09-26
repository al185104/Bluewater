using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Levels;

public class CreateLevelRequest
{
  public const string Route = "/Levels";

  [Required]
  public string? Name { get; set; }

  [Required]
  public string? Value { get; set; }

  public bool IsActive { get; set; } = true;
}

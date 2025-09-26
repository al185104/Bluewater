using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Levels;

public class UpdateLevelRequest
{
  public const string Route = "/Levels/{LevelId:guid}";
  public static string BuildRoute(Guid levelId) => Route.Replace("{LevelId:guid}", levelId.ToString());

  public Guid LevelId { get; set; }

  [Required]
  public Guid Id { get; set; }

  [Required]
  public string? Name { get; set; }

  [Required]
  public string? Value { get; set; }

  public bool IsActive { get; set; }
}

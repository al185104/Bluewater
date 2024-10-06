using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Divisions;

public class UpdateDivisionRequest
{
  public const string Route = "/Divisions/{DivisionId:guid}";
  public static string BuildRoute(Guid divisionId) => Route.Replace("{DivisionId:guid}", divisionId.ToString());

  public Guid DivisionId { get; set; }

  [Required]
  public Guid Id { get; set; }
  [Required]
  public string? Name { get; set; }
  public string? Description { get; set; }
}

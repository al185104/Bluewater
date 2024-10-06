using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Divisions;
public class CreateDivisionRequest
{
  public const string Route = "/Divisions";

  [Required]
  public string? Name { get; set; }
  public string? Description { get; set; }
}

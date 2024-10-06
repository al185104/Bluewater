using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Departments;
public class CreateDepartmentRequest
{
  public const string Route = "/Departments";

  [Required]
  public string? Name { get; set; }
  public string? Description { get; set; }
  [Required]
  public Guid DivisionId { get; set; }
}

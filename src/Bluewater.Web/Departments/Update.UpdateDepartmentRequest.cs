using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Departments;

public class UpdateDepartmentRequest
{
  public const string Route = "/Departments/{DepartmentId:guid}";
  public static string BuildRoute(Guid DepartmentId) => Route.Replace("{DepartmentId:guid}", DepartmentId.ToString());

  public Guid DepartmentId { get; set; }

  [Required]
  public Guid Id { get; set; }
  [Required]
  public string? Name { get; set; }
  public string? Description { get; set; }
  [Required]
  public Guid DivisionId { get; set; }
}

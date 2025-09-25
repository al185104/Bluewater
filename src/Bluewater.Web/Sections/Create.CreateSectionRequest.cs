using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Sections;

public class CreateSectionRequest
{
  public const string Route = "/Sections";

  [Required]
  public string? Name { get; set; }
  public string? Description { get; set; }
  public string? Approved1Id { get; set; }
  public string? Approved2Id { get; set; }
  public string? Approved3Id { get; set; }

  [Required]
  public Guid DepartmentId { get; set; }
}

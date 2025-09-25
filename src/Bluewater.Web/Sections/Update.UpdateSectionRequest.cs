using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Sections;

public class UpdateSectionRequest
{
  public const string Route = "/Sections/{SectionId:guid}";
  public static string BuildRoute(Guid sectionId) => Route.Replace("{SectionId:guid}", sectionId.ToString());

  public Guid SectionId { get; set; }

  [Required]
  public Guid Id { get; set; }

  [Required]
  public string? Name { get; set; }
  public string? Description { get; set; }
  public string? Approved1Id { get; set; }
  public string? Approved2Id { get; set; }
  public string? Approved3Id { get; set; }

  [Required]
  public Guid DepartmentId { get; set; }
}

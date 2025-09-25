namespace Bluewater.Web.Sections;

public class DeleteSectionRequest
{
  public const string Route = "/Sections/{SectionId:guid}";
  public static string BuildRoute(Guid sectionId) => Route.Replace("{SectionId:guid}", sectionId.ToString());

  public Guid SectionId { get; set; }
}

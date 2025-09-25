namespace Bluewater.Web.Sections;

public class UpdateSectionResponse(SectionRecord section)
{
  public SectionRecord Section { get; set; } = section;
}

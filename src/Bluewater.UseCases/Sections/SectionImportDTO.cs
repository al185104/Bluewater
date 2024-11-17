namespace Bluewater.UseCases.Sections;
public record SectionImportDTO() : SectionDTO
{
  public string Department { get; set; } = null!;
}

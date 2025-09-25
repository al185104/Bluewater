namespace Bluewater.Web.Positions;

public class CreatePositionResponse(Guid Id, string Name, string? Description, Guid SectionId)
{
  public Guid Id { get; set; } = Id;
  public string Name { get; set; } = Name;
  public string? Description { get; set; } = Description;
  public Guid SectionId { get; set; } = SectionId;
}

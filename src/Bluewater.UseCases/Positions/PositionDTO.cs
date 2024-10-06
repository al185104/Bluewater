namespace Bluewater.UseCases.Positions;
public record PositionDTO()
{
  public Guid Id { get; init; }
  public string Name { get; set; } = null!;
  public string? Description { get; set; }
  public Guid SectionId { get; set; }

  public PositionDTO(Guid id, string name, string description, Guid sectionId) : this()
  {
    Id = id;
    Name = name;
    Description = description;
    SectionId = sectionId;
  }
}

namespace Bluewater.UseCases.Sections;
public record SectionDTO()
{
  public Guid Id { get; init; }
  public string Name { get; set; } = null!;
  public string? Description { get; set; }
  public string? Approved1Id { get; set; }
  public string? Approved2Id { get; set; }
  public string? Approved3Id { get; set; }
  public Guid DepartmentId { get; set; }

  public SectionDTO (Guid id, string name, string description, string? approved1id, string? approved2id, string? approved3id, Guid departmentId) : this()
  {
    Id = id;
    Name = name;
    Description = description;
    Approved1Id = approved1id;
    Approved2Id = approved2id;
    Approved3Id = approved3id;
    DepartmentId = departmentId;
  }
}

namespace Bluewater.UseCases.Departments;
public record DepartmentDTO()
{
  public Guid Id { get; init; }
  public string Name { get; set; } = null!;
  public string? Description { get; set; }
  public Guid DivisionId { get; init; }

  public DepartmentDTO(Guid id, string name, string description, Guid divisionId) : this()
  {
    Id = id;
    Name = name;
    Description = description;
    DivisionId = divisionId;
  }
}

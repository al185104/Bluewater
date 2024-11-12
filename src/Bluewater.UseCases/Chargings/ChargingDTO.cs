namespace Bluewater.UseCases.Chargings;
public record ChargingDTO()
{
  public Guid Id { get; init; }
  public string Name { get; set; } = null!;
  public string? Description { get; set; }
  public Guid? DepartmentId { get; set; }

  public ChargingDTO(Guid id, string name, string description, Guid? deptId) : this()
  {
    Id = id;
    Name = name;
    Description = description;
    DepartmentId = deptId;
  }
}

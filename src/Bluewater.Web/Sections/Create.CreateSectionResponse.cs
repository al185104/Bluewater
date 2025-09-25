namespace Bluewater.Web.Sections;

public class CreateSectionResponse(
  Guid Id,
  string Name,
  string? Description,
  string? Approved1Id,
  string? Approved2Id,
  string? Approved3Id,
  Guid DepartmentId)
{
  public Guid Id { get; set; } = Id;
  public string Name { get; set; } = Name;
  public string? Description { get; set; } = Description;
  public string? Approved1Id { get; set; } = Approved1Id;
  public string? Approved2Id { get; set; } = Approved2Id;
  public string? Approved3Id { get; set; } = Approved3Id;
  public Guid DepartmentId { get; set; } = DepartmentId;
}

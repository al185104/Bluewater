namespace Bluewater.Web.Departments;

public class CreateDepartmentResponse(Guid Id, string Name, string? Description)
{
  public Guid Id { get; set; } = Id;
  public string Name { get; set; } = Name;
  public string? Description { get; set; } = Description;
} 

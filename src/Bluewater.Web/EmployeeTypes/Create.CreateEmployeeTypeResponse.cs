namespace Bluewater.Web.EmployeeTypes;

public class CreateEmployeeTypeResponse(Guid Id, string Name, string Value, bool IsActive)
{
  public Guid Id { get; set; } = Id;
  public string Name { get; set; } = Name;
  public string Value { get; set; } = Value;
  public bool IsActive { get; set; } = IsActive;
}

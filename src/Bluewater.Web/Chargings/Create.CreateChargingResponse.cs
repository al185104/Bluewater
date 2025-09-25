namespace Bluewater.Web.Chargings;

public class CreateChargingResponse(Guid Id, string Name, string? Description, Guid? DepartmentId)
{
  public Guid Id { get; set; } = Id;
  public string Name { get; set; } = Name;
  public string? Description { get; set; } = Description;
  public Guid? DepartmentId { get; set; } = DepartmentId;
}

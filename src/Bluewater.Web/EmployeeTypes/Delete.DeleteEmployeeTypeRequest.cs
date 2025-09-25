namespace Bluewater.Web.EmployeeTypes;

public class DeleteEmployeeTypeRequest
{
  public const string Route = "/EmployeeTypes/{EmployeeTypeId:guid}";

  public Guid EmployeeTypeId { get; set; }
}

namespace Bluewater.Web.EmployeeTypes;

public class GetEmployeeTypeByIdRequest
{
  public const string Route = "/EmployeeTypes/{EmployeeTypeId:guid}";

  public Guid EmployeeTypeId { get; set; }
}

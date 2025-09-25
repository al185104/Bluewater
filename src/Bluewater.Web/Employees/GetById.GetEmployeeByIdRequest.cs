namespace Bluewater.Web.Employees;

public class GetEmployeeByIdRequest
{
  public const string Route = "/Employees/{EmployeeId:guid}";

  public Guid EmployeeId { get; set; }
}

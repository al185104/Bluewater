namespace Bluewater.Web.Employees;

public class DeleteEmployeeRequest
{
  public const string Route = "/Employees/{EmployeeId:guid}";

  public Guid EmployeeId { get; set; }
}

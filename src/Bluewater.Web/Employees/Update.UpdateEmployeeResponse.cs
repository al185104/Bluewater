namespace Bluewater.Web.Employees;

public class UpdateEmployeeResponse(EmployeeRecord Employee)
{
  public EmployeeRecord Employee { get; set; } = Employee;
}

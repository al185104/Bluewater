namespace Bluewater.Web.EmployeeTypes;

public class UpdateEmployeeTypeResponse(EmployeeTypeRecord EmployeeType)
{
  public EmployeeTypeRecord EmployeeType { get; set; } = EmployeeType;
}

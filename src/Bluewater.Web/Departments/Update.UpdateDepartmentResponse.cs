namespace Bluewater.Web.Departments;

public class UpdateDepartmentResponse(DepartmentRecord Department)
{
  public DepartmentRecord Department { get; set; } = Department;
}

namespace Bluewater.Web.Employees;

public class EmployeeListResponse
{
  public List<EmployeeRecord> Employees { get; set; } = new();
  public int TotalCount { get; set; }
}

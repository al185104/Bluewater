namespace Bluewater.Web.Schedules;

public class ScheduleListResponse
{
  public List<EmployeeScheduleRecord> Employees { get; set; } = new();
  public int TotalCount { get; set; }
}

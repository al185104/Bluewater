namespace Bluewater.Web.Timesheets;

public class TimesheetListAllResponse
{
  public List<AllEmployeeTimesheetRecord> Employees { get; set; } = new();
}

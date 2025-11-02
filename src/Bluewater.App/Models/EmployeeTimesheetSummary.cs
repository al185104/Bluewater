using System;
using System.Collections.ObjectModel;

namespace Bluewater.App.Models;

public class EmployeeTimesheetSummary
{
  public Guid EmployeeId { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Department { get; set; } = string.Empty;
  public string Section { get; set; } = string.Empty;
  public string Charging { get; set; } = string.Empty;
  public decimal TotalWorkHours { get; set; }
  public decimal TotalBreak { get; set; }
  public decimal TotalLates { get; set; }
  public int TotalAbsents { get; set; }
  public ObservableCollection<AttendanceTimesheetSummary> Timesheets { get; } = new();
}

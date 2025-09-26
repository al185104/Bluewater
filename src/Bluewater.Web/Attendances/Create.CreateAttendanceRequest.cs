using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Attendances;

public class CreateAttendanceRequest
{
  public const string Route = "/Attendances";

  [Required]
  public Guid EmployeeId { get; set; }
  public Guid? ShiftId { get; set; }
  public Guid? TimesheetId { get; set; }
  public Guid? LeaveId { get; set; }
  public DateOnly? EntryDate { get; set; }
  public decimal? WorkHrs { get; set; }
  public decimal? LateHrs { get; set; }
  public decimal? UnderHrs { get; set; }
  public decimal? OverbreakHrs { get; set; }
  public decimal? NightShiftHrs { get; set; }
  public bool IsLocked { get; set; }
}

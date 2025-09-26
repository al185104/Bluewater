using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Attendances;

public class UpdateAttendanceRequest
{
  public const string Route = "/Attendances";

  [Required]
  public Guid EmployeeId { get; set; }

  [Required]
  public DateOnly? EntryDate { get; set; }

  public Guid? ShiftId { get; set; }
  public Guid? TimesheetId { get; set; }
  public Guid? LeaveId { get; set; }
  public bool IsLocked { get; set; }
}

namespace Bluewater.Web.Attendances;

public class UpdateAttendanceResponse(AttendanceRecord Attendance)
{
  public AttendanceRecord Attendance { get; set; } = Attendance;
}

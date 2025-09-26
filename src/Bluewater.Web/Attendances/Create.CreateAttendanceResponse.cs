namespace Bluewater.Web.Attendances;

public class CreateAttendanceResponse(AttendanceRecord Attendance)
{
  public AttendanceRecord Attendance { get; set; } = Attendance;
}

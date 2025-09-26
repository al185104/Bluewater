namespace Bluewater.Web.Attendances;

public class DeleteAttendanceRequest
{
  public const string Route = "/Attendances/{AttendanceId:guid}";
  public static string BuildRoute(Guid attendanceId) => Route.Replace("{AttendanceId:guid}", attendanceId.ToString());

  public Guid AttendanceId { get; set; }
}

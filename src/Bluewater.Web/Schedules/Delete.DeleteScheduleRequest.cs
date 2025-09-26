namespace Bluewater.Web.Schedules;

public class DeleteScheduleRequest
{
  public const string Route = "/Schedules/{ScheduleId:guid}";
  public static string BuildRoute(Guid scheduleId) => Route.Replace("{ScheduleId:guid}", scheduleId.ToString());

  public Guid ScheduleId { get; set; }
}

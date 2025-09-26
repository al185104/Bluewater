namespace Bluewater.Web.Schedules;

public class UpdateScheduleResponse(ScheduleRecord Schedule)
{
  public ScheduleRecord Schedule { get; set; } = Schedule;
}

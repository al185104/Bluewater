namespace Bluewater.Web.Schedules;

public class CreateScheduleResponse(Guid ScheduleId)
{
  public Guid ScheduleId { get; set; } = ScheduleId;
}

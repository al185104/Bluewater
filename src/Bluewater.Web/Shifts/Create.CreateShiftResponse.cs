namespace Bluewater.Web.Shifts;

public class CreateShiftResponse(
  Guid Id,
  string Name,
  TimeOnly? ShiftStartTime,
  TimeOnly? ShiftBreakTime,
  TimeOnly? ShiftBreakEndTime,
  TimeOnly? ShiftEndTime,
  decimal BreakHours)
{
  public Guid Id { get; set; } = Id;
  public string Name { get; set; } = Name;
  public TimeOnly? ShiftStartTime { get; set; } = ShiftStartTime;
  public TimeOnly? ShiftBreakTime { get; set; } = ShiftBreakTime;
  public TimeOnly? ShiftBreakEndTime { get; set; } = ShiftBreakEndTime;
  public TimeOnly? ShiftEndTime { get; set; } = ShiftEndTime;
  public decimal BreakHours { get; set; } = BreakHours;
}

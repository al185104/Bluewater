namespace Bluewater.Web.Shifts;

public record ShiftRecord(
  Guid Id,
  string Name,
  TimeOnly? ShiftStartTime,
  TimeOnly? ShiftBreakTime,
  TimeOnly? ShiftBreakEndTime,
  TimeOnly? ShiftEndTime,
  decimal BreakHours
);

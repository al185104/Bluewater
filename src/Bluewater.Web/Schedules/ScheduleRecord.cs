namespace Bluewater.Web.Schedules;

public record ScheduleRecord(
  Guid Id,
  Guid EmployeeId,
  string Name,
  Guid ShiftId,
  DateOnly ScheduleDate,
  bool IsDefault,
  ShiftDetailsRecord? Shift);

public record ShiftDetailsRecord(
  Guid Id,
  string Name,
  TimeOnly? ShiftStartTime,
  TimeOnly? ShiftBreakTime,
  TimeOnly? ShiftBreakEndTime,
  TimeOnly? ShiftEndTime,
  decimal BreakHours);

public record EmployeeScheduleRecord(
  Guid EmployeeId,
  string Barcode,
  string Name,
  string Section,
  string Charging,
  List<ShiftInfoRecord> Shifts);

public record ShiftInfoRecord(
  Guid ScheduleId,
  ShiftDetailsRecord? Shift,
  DateOnly ScheduleDate,
  bool IsDefault,
  bool IsUpdated);

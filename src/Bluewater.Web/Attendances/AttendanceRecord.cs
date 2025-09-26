using System;

namespace Bluewater.Web.Attendances;

public record AttendanceRecord(
  Guid Id,
  Guid EmployeeId,
  Guid? ShiftId,
  Guid? TimesheetId,
  Guid? LeaveId,
  DateOnly? EntryDate,
  decimal? WorkHrs,
  decimal? LateHrs,
  decimal? UnderHrs,
  decimal? OverbreakHrs,
  decimal? NightShiftHours,
  bool IsLocked,
  ShiftRecord? Shift,
  TimesheetRecord? Timesheet);

public record ShiftRecord(
  Guid Id,
  string Name,
  TimeOnly? ShiftStartTime,
  TimeOnly? ShiftBreakTime,
  TimeOnly? ShiftBreakEndTime,
  TimeOnly? ShiftEndTime,
  decimal BreakHours);

public record TimesheetRecord(
  Guid Id,
  Guid EmployeeId,
  DateTime? TimeIn1,
  DateTime? TimeOut1,
  DateTime? TimeIn2,
  DateTime? TimeOut2,
  DateOnly? EntryDate,
  bool IsEdited);

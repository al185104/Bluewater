using Bluewater.UseCases.Attendances;
using Bluewater.UseCases.Shifts;
using Bluewater.UseCases.Timesheets;

namespace Bluewater.Web.Attendances;

public static class AttendanceMapper
{
  public static AttendanceRecord ToRecord(AttendanceDTO dto)
  {
    return new AttendanceRecord(
      dto.Id,
      dto.EmployeeId,
      dto.ShiftId,
      dto.TimesheetId,
      dto.LeaveId,
      dto.EntryDate,
      dto.WorkHrs,
      dto.LateHrs,
      dto.UnderHrs,
      dto.OverbreakHrs,
      dto.NightShiftHours,
      dto.IsLocked,
      ToRecord(dto.Shift),
      ToRecord(dto.Timesheet));
  }

  public static AllAttendanceRecord ToRecord(AllAttendancesDTO dto)
  {
    return new AllAttendanceRecord(
      dto.EmployeeId,
      dto.Barcode,
      dto.Name,
      dto.Department,
      dto.Section,
      dto.Charging,
      dto.Attendances.Select(ToRecord).ToList(),
      dto.TotalWorkHrs,
      dto.TotalAbsences,
      dto.TotalLateHrs,
      dto.TotalUnderHrs,
      dto.TotalOverbreakHrs,
      dto.TotalNightShiftHrs,
      dto.TotalLeaves);
  }

  private static ShiftRecord? ToRecord(ShiftDTO? shift)
  {
    if (shift is null) return null;

    return new ShiftRecord(
      shift.Id,
      shift.Name,
      shift.ShiftStartTime,
      shift.ShiftBreakTime,
      shift.ShiftBreakEndTime,
      shift.ShiftEndTime,
      shift.BreakHours);
  }

  private static TimesheetRecord? ToRecord(TimesheetDTO? timesheet)
  {
    if (timesheet is null) return null;

    return new TimesheetRecord(
      timesheet.Id,
      timesheet.EmployeeId,
      timesheet.TimeIn1,
      timesheet.TimeOut1,
      timesheet.TimeIn2,
      timesheet.TimeOut2,
      timesheet.EntryDate,
      timesheet.IsEdited);
  }
}

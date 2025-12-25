using System;
using System.Collections.Generic;

namespace Bluewater.App.Models;

public class AttendanceListResponseDto
{
  public List<AttendanceDto?> Attendances { get; set; } = new();
}

public class AttendanceListAllResponseDto
{
  public List<EmployeeAttendanceDto?> Employees { get; set; } = new();
  public int TotalCount { get; set; }
}

public class AttendanceDto
{
  public Guid Id { get; set; }
  public Guid EmployeeId { get; set; }
  public Guid? ShiftId { get; set; }
  public Guid? TimesheetId { get; set; }
  public Guid? LeaveId { get; set; }
  public DateOnly? EntryDate { get; set; }
  public decimal? WorkHrs { get; set; }
  public decimal? LateHrs { get; set; }
  public decimal? UnderHrs { get; set; }
  public decimal? OverbreakHrs { get; set; }
  public decimal? NightShiftHours { get; set; }
  public bool IsLocked { get; set; }
  public AttendanceShiftDto? Shift { get; set; }
  public AttendanceTimesheetDto? Timesheet { get; set; }
}

public class AttendanceShiftDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public TimeOnly? ShiftStartTime { get; set; }
  public TimeOnly? ShiftBreakTime { get; set; }
  public TimeOnly? ShiftBreakEndTime { get; set; }
  public TimeOnly? ShiftEndTime { get; set; }
  public decimal BreakHours { get; set; }
}

public class AttendanceTimesheetDto
{
  public Guid Id { get; set; }
  public Guid EmployeeId { get; set; }
  public DateTime? TimeIn1 { get; set; }
  public DateTime? TimeOut1 { get; set; }
  public DateTime? TimeIn2 { get; set; }
  public DateTime? TimeOut2 { get; set; }
  public DateOnly? EntryDate { get; set; }
  public bool IsEdited { get; set; }
}

public class EmployeeAttendanceDto
{
  public Guid EmployeeId { get; set; }
  public string Barcode { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string? Department { get; set; }
  public string? Section { get; set; }
  public string? Charging { get; set; }
  public List<AttendanceDto?> Attendances { get; set; } = new();
  public decimal TotalWorkHrs { get; set; }
  public int TotalAbsences { get; set; }
  public decimal TotalLateHrs { get; set; }
  public decimal TotalUnderHrs { get; set; }
  public decimal TotalOverbreakHrs { get; set; }
  public decimal TotalNightShiftHrs { get; set; }
  public decimal TotalLeaves { get; set; }
}

public class AttendanceCreateRequestDto
{
  public const string Route = "Attendances";

  public Guid EmployeeId { get; set; }
  public Guid? ShiftId { get; set; }
  public Guid? TimesheetId { get; set; }
  public Guid? LeaveId { get; set; }
  public DateOnly? EntryDate { get; set; }
  public decimal? WorkHrs { get; set; }
  public decimal? LateHrs { get; set; }
  public decimal? UnderHrs { get; set; }
  public decimal? OverbreakHrs { get; set; }
  public decimal? NightShiftHrs { get; set; }
  public bool IsLocked { get; set; }
}

public class AttendanceCreateResponseDto
{
  public AttendanceDto? Attendance { get; set; }
}

public class AttendanceUpdateRequestDto
{
  public const string Route = "Attendances";

  public Guid EmployeeId { get; set; }
  public DateOnly? EntryDate { get; set; }
  public Guid? ShiftId { get; set; }
  public Guid? TimesheetId { get; set; }
  public Guid? LeaveId { get; set; }
  public bool IsLocked { get; set; }
}

public class AttendanceUpdateResponseDto
{
  public AttendanceDto? Attendance { get; set; }
}

public static class AttendanceRequestRoutes
{
  public static string BuildGetRoute(Guid attendanceId) => $"Attendances/{attendanceId}";
  public static string BuildDeleteRoute(Guid attendanceId) => $"Attendances/{attendanceId}";
}

public class AttendanceSummary : IRowIndexed
{
  public Guid Id { get; set; }
  public Guid EmployeeId { get; set; }
  public Guid? ShiftId { get; set; }
  public Guid? TimesheetId { get; set; }
  public Guid? LeaveId { get; set; }
  public DateOnly? EntryDate { get; set; }
  public decimal? WorkHours { get; set; }
  public decimal? LateHours { get; set; }
  public decimal? UnderHours { get; set; }
  public decimal? OverbreakHours { get; set; }
  public decimal? NightShiftHours { get; set; }
  public bool IsLocked { get; set; }
  public AttendanceShiftSummary? Shift { get; set; }
  public AttendanceTimesheetSummary? Timesheet { get; set; }
  public int RowIndex { get; set; }
}

public class AttendanceShiftSummary
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public TimeOnly? ShiftStartTime { get; set; }
  public TimeOnly? ShiftBreakTime { get; set; }
  public TimeOnly? ShiftBreakEndTime { get; set; }
  public TimeOnly? ShiftEndTime { get; set; }
  public decimal BreakHours { get; set; }
}

public partial class AttendanceTimesheetSummary : IRowIndexed
{
  public Guid Id { get; set; }
  public Guid EmployeeId { get; set; }
  public DateTime? TimeIn1 { get; set; }
  public DateTime? TimeOut1 { get; set; }
  public DateTime? TimeIn2 { get; set; }
  public DateTime? TimeOut2 { get; set; }
  public DateOnly? EntryDate { get; set; }
  public bool IsEdited { get; set; }
  public int RowIndex { get; set; }
}

public class EmployeeAttendanceSummary
{
  public Guid EmployeeId { get; set; }
  public string Barcode { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string? Department { get; set; }
  public string? Section { get; set; }
  public string? Charging { get; set; }
  public List<AttendanceSummary> Attendances { get; set; } = new();
  public decimal TotalWorkHours { get; set; }
  public int TotalAbsences { get; set; }
  public decimal TotalLateHours { get; set; }
  public decimal TotalUnderHours { get; set; }
  public decimal TotalOverbreakHours { get; set; }
  public decimal TotalNightShiftHours { get; set; }
  public decimal TotalLeaves { get; set; }
}

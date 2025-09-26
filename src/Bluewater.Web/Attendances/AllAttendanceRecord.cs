namespace Bluewater.Web.Attendances;

public record AllAttendanceRecord(
  Guid EmployeeId,
  string Barcode,
  string Name,
  string? Department,
  string? Section,
  string? Charging,
  List<AttendanceRecord> Attendances,
  decimal TotalWorkHrs,
  int TotalAbsences,
  decimal TotalLateHrs,
  decimal TotalUnderHrs,
  decimal TotalOverbreakHrs,
  decimal TotalNightShiftHrs,
  decimal TotalLeaves);

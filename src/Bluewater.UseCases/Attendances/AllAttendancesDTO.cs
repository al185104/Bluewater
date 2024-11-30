namespace Bluewater.UseCases.Attendances;
public record AllAttendancesDTO(Guid EmployeeId, string Barcode, string Name, string? Department, string? Section, string? Charging, List<AttendanceDTO> Attendances, decimal TotalWorkHrs, int TotalAbsences, decimal TotalLateHrs, decimal TotalUnderHrs, decimal TotalOverbreakHrs, decimal TotalNightShiftHrs, decimal TotalLeaves);

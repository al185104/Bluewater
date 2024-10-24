namespace Bluewater.UseCases.Attendances;
public record AllAttendancesDTO(Guid EmployeeId, string Name, string? Department, string? Section, string? Charging, List<AttendanceDTO> Attendances, decimal TotalWorkHrs, decimal TotalLateHrs, decimal TotalUnderHrs, decimal TotalLeaves);

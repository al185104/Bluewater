namespace Bluewater.UseCases.Attendances;
public record AttendanceDTO(Guid Id, Guid EmployeeId, Guid? ShiftId, Guid? TimesheetId, Guid? LeaveId, DateOnly? EntryDate, decimal? WorkHrs, decimal? LateHrs, decimal? UnderHrs, bool IsLocked = false);

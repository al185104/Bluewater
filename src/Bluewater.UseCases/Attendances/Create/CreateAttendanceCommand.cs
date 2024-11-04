using Ardalis.Result;

namespace Bluewater.UseCases.Attendances.Create;
public record CreateAttendanceCommand(Guid EmployeeId, Guid? ShiftId, Guid? TimesheetId, Guid? LeaveId, DateOnly? EntryDate, decimal? WorkHrs, decimal? LateHrs, decimal? UnderHrs, decimal? OverbreakHrs, decimal? NightShiftHrs, bool IsLocked = false) : Ardalis.SharedKernel.ICommand<Result<Guid>>;

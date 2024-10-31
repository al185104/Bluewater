using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Attendances.Update;
public record UpdateAttendanceCommand(Guid EmployeeId, Guid? ShiftId, Guid? TimesheetId, Guid? LeaveId, DateOnly? EntryDate, bool IsLocked = false) : ICommand<Result<AttendanceDTO>>;

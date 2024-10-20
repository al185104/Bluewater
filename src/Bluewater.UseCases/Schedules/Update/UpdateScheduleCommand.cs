using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Schedules.Update;
public record UpdateScheduleCommand(Guid ScheduleId, Guid EmployeeId, Guid ShiftId, DateOnly ScheduleDate, bool IsDefault) : ICommand<Result<ScheduleDTO>>;

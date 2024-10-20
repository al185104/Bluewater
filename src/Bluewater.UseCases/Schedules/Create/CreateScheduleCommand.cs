using Ardalis.Result;

namespace Bluewater.UseCases.Schedules.Create;
public record CreateScheduleCommand(Guid EmployeeId, Guid ShiftId, DateOnly ScheduleDate, bool IsDefault) : Ardalis.SharedKernel.ICommand<Result<Guid>>;

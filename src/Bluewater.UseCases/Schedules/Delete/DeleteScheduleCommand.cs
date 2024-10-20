using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Schedules.Delete;
public record DeleteScheduleCommand(Guid ScheduleId) : ICommand<Result>;

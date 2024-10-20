using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Schedules.Get;
public record GetScheduleQuery(Guid ScheduleId) : IQuery<Result<ScheduleDTO>>;

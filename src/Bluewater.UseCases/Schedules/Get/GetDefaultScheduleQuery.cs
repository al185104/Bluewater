using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Schedules.Get;
public record GetDefaultScheduleQuery(Guid empId) : IQuery<Result<IEnumerable<ScheduleDTO>>>;

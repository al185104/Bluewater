using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Schedules.Get;
public record GetDefaultScheduleByEmpIdAndDayQuery(Guid empId, DayOfWeek dayOfWeek) : IQuery<Result<ScheduleDTO>>;

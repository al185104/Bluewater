using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.UseCases.Shifts;

namespace Bluewater.UseCases.Schedules.Get;
public record GetDefaultScheduleQuery(Guid empId) : IQuery<Result<IEnumerable<ShiftDTO>>>;

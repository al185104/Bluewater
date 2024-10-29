using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Schedules.Get;
public record GetScheduleByEmpIdAndDateQuery(Guid EmpId, DateOnly EntryDate) : IQuery<Result<ScheduleDTO>>;

using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Schedules.List;
public record ListScheduleQuery(int? skip, int? take, string chargingName, DateOnly startDate, DateOnly endDate) : IQuery<Result<IEnumerable<EmployeeScheduleDTO>>>;

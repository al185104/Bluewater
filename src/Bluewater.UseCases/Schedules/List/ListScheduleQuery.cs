using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.UseCases.Schedules.List;
public record ListScheduleQuery(int? skip, int? take, string chargingName, DateOnly startDate, DateOnly endDate, Tenant tenant) : IQuery<Result<PagedResult<EmployeeScheduleDTO>>>;

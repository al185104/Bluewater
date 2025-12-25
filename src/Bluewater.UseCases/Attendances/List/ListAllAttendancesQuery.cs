using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate.Enum;
using Bluewater.UseCases.Common;

namespace Bluewater.UseCases.Attendances.List;
public record ListAllAttendancesQuery(int? skip, int? take, string charging, DateOnly startDate, DateOnly endDate, Tenant tenant) : IQuery<Result<PagedResult<AllAttendancesDTO>>>;

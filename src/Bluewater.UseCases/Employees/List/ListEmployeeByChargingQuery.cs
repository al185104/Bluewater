using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.UseCases.Employees.List;
public record ListEmployeeByChargingQuery(int? skip, int? take, string chargingName, Tenant tenant) : IQuery<Result<PagedResult<EmployeeDTO>>>;

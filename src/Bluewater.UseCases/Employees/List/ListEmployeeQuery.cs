using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate.Enum;
using Bluewater.UseCases.Common;

namespace Bluewater.UseCases.Employees.List;
public record ListEmployeeQuery(int? skip, int? take, Tenant tenant = Tenant.Maribago) : IQuery<Result<PagedResult<EmployeeDTO>>>;

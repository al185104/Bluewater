using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.UseCases.Employees.List;
public record ListEmployeeQuery(int? skip, int? take, Tenant tenant = Tenant.Maribago) : IQuery<Result<IEnumerable<EmployeeDTO>>>;

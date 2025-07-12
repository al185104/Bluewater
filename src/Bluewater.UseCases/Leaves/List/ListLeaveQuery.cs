using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.UseCases.Leaves.List;
public record ListLeaveQuery(int? skip, int? take, Tenant tenant) : IQuery<Result<IEnumerable<LeaveDTO>>>;

using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.UseCases.Forms.FailureInOuts.List;
public record ListFailureInOutQuery(int? skip, int? take, Tenant tenant) : IQuery<Result<IEnumerable<FailureInOutDTO>>>;

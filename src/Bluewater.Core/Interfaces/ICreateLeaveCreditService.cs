using Ardalis.Result;
using Bluewater.Core.LeaveCreditAggregate;

namespace Bluewater.Core.Interfaces;

public interface ICreateLeaveCreditService
{
  Task<Result<Guid>> CreateLeaveCredit(
    string code,
    string? description,
    decimal? credit,
    int? sortOrder,
    bool isLeaveWithPay,
    bool isCanCarryOver,
    CancellationToken cancellationToken = default);
}

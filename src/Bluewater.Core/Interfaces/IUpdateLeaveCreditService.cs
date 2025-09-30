using Ardalis.Result;
using Bluewater.Core.LeaveCreditAggregate;

namespace Bluewater.Core.Interfaces;

public interface IUpdateLeaveCreditService
{
  Task<Result<LeaveCredit>> UpdateLeaveCreditAsync(
    Guid leaveCreditId,
    string code,
    string? description,
    decimal? credit,
    int? sortOrder,
    bool isLeaveWithPay,
    bool isCanCarryOver,
    CancellationToken cancellationToken = default);
}

using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;
using Bluewater.Core.LeaveCreditAggregate;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.Services;

public class UpdateLeaveCreditService(IRepository<LeaveCredit> _repository, ILogger<UpdateLeaveCreditService> _logger) : IUpdateLeaveCreditService
{
  public async Task<Result<LeaveCredit>> UpdateLeaveCreditAsync(
    Guid leaveCreditId,
    string code,
    string? description,
    decimal? credit,
    int? sortOrder,
    bool isLeaveWithPay,
    bool isCanCarryOver,
    CancellationToken cancellationToken = default)
  {
    var existing = await _repository.GetByIdAsync(leaveCreditId, cancellationToken);
    if (existing == null)
    {
      return Result.NotFound();
    }

    if (string.IsNullOrWhiteSpace(code))
    {
      return Result<LeaveCredit>.CriticalError(code ?? string.Empty, "Leave code is required.");
    }

    if (string.IsNullOrWhiteSpace(description))
    {
      return Result<LeaveCredit>.CriticalError(description ?? string.Empty, "Leave description is required.");
    }

    if (credit <= 0)
    {
      return Result<LeaveCredit>.CriticalError((credit ?? 0).ToString(), "Leave credit must be greater than 0.");
    }

    if (sortOrder < 0)
    {
      return Result<LeaveCredit>.CriticalError((sortOrder ?? 0).ToString(), "Sort order must be greater than or equal to 0.");
    }

    if (sortOrder > 100)
    {
      return Result<LeaveCredit>.CriticalError((sortOrder ?? 0).ToString(), "Sort order must be less than or equal to 100.");
    }

    existing.UpdateLeaveCredit(code, description!, credit ?? existing.DefaultCredits, isLeaveWithPay, isCanCarryOver, sortOrder ?? existing.SortOrder);
    await _repository.UpdateAsync(existing, cancellationToken);

    _logger.LogInformation("Updated Leave Credit {LeaveCreditId}", leaveCreditId);

    return Result.Success(existing);
  }
}

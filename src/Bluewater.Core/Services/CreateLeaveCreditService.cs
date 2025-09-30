using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;
using Bluewater.Core.LeaveCreditAggregate;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.Services;

public class CreateLeaveCreditService(IRepository<LeaveCredit> _repository, ILogger<CreateLeaveCreditService> _logger) : ICreateLeaveCreditService
{
  public async Task<Result<Guid>> CreateLeaveCredit(
    string code,
    string? description,
    decimal? credit,
    int? sortOrder,
    bool isLeaveWithPay,
    bool isCanCarryOver,
    CancellationToken cancellationToken = default)
  {
    if (string.IsNullOrWhiteSpace(code))
    {
      return Result<Guid>.CriticalError(code ?? string.Empty, "Leave code is required.");
    }

    if (string.IsNullOrWhiteSpace(description))
    {
      return Result<Guid>.CriticalError(description ?? string.Empty, "Leave description is required.");
    }

    if (credit <= 0)
    {
      return Result<Guid>.CriticalError((credit ?? 0).ToString(), "Leave credit must be greater than 0.");
    }

    if (sortOrder < 0)
    {
      return Result<Guid>.CriticalError((sortOrder ?? 0).ToString(), "Sort order must be greater than or equal to 0.");
    }

    if (sortOrder > 100)
    {
      return Result<Guid>.CriticalError((sortOrder ?? 0).ToString(), "Sort order must be less than or equal to 100.");
    }

    var leaveCredit = new LeaveCredit(code, description!, credit ?? 0.00m, isLeaveWithPay, isCanCarryOver, sortOrder ?? 0);
    var created = await _repository.AddAsync(leaveCredit, cancellationToken);

    _logger.LogInformation("Created Leave Credit {LeaveCreditId}", created.Id);

    return created.Id;
  }
}

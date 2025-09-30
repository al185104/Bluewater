using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.UseCases.LeaveCredits;

namespace Bluewater.UseCases.LeaveCredits.Update;

public record UpdateLeaveCreditCommand(
  Guid LeaveCreditId,
  string Code,
  string? Description,
  decimal? Credit,
  int? SortOrder,
  bool IsLeaveWithPay,
  bool IsCanCarryOver) : ICommand<Result<LeaveCreditDTO>>;

using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;
using Bluewater.Core.LeaveCreditAggregate;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.Services;
public class DeleteLeaveCreditService(IRepository<LeaveCredit> _repository, ILogger<DeleteLeaveCreditService> _logger) : IDeleteLeaveCreditService
{
  public async Task<Result> DeleteLeaveCredit(Guid leaveCreditId)
  {
    _logger.LogInformation("Deleting Leave Credit {leaveCreditId}", leaveCreditId);
    LeaveCredit? aggregateToDelete = await _repository.GetByIdAsync(leaveCreditId);
    if (aggregateToDelete == null) return Result.NotFound();

    await _repository.DeleteAsync(aggregateToDelete);
    return Result.Success();
  }
}

using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.LeaveAggregate;
using Bluewater.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.Services;

public class DeleteLeaveService(IRepository<Leave> _repository, ILogger<DeleteLeaveService> _logger) : IDeleteLeaveService
{
  public async Task<Result> DeleteLeave(Guid leaveId)
  {
    _logger.LogInformation("Deleting Leave Credit {leaveId}", leaveId);
    Leave? aggregateToDelete = await _repository.GetByIdAsync(leaveId);
    if (aggregateToDelete == null) return Result.NotFound();

    await _repository.DeleteAsync(aggregateToDelete);
    return Result.Success();
  }
}

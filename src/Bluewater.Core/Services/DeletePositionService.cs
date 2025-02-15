using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.PositionAggregate;
using Bluewater.Core.PositionAggregate.Events;
using Bluewater.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.Services;
public class DeletePositionService(IRepository<Position> _repository, ILogger<DeletePositionService> _logger) : IDeletePositionService
{
  public async Task<Result> DeletePosition(Guid PositionId)
  {
    _logger.LogInformation("Deleting Position {contributorId}", PositionId);
    Position? aggregateToDelete = await _repository.GetByIdAsync(PositionId);
    if (aggregateToDelete == null) return Result.NotFound();

    await _repository.DeleteAsync(aggregateToDelete);
    return Result.Success();
  }
}

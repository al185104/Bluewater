using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.PositionAggregate;
using Bluewater.Core.PositionAggregate.Events;
using Bluewater.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.Services;
public class DeletePositionService(IRepository<Position> _repository, IMediator _mediator, ILogger<DeletePositionService> _logger) : IDeletePositionService
{
  public async Task<Result> DeletePosition(Guid PositionId)
  {
    _logger.LogInformation("Deleting Position {contributorId}", PositionId);
    Position? aggregateToDelete = await _repository.GetByIdAsync(PositionId);
    if (aggregateToDelete == null) return Result.NotFound();

    await _repository.DeleteAsync(aggregateToDelete);
    var domainEvent = new PositionDeletedEvent(PositionId);
    await _mediator.Publish(domainEvent);
    return Result.Success();
  }
}

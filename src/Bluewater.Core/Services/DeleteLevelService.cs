using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.LevelAggregate;
using Bluewater.Core.LevelAggregate.Events;
using Bluewater.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.Services;
public class DeleteLevelService(IRepository<Level> _repository, IMediator _mediator, ILogger<DeleteLevelService> _logger) : IDeleteLevelService
{
  public async Task<Result> DeleteLevel(Guid LevelId)
  {
    _logger.LogInformation("Deleting Level {contributorId}", LevelId);
    Level? aggregateToDelete = await _repository.GetByIdAsync(LevelId);
    if (aggregateToDelete == null) return Result.NotFound();

    await _repository.DeleteAsync(aggregateToDelete);
    var domainEvent = new LevelDeletedEvent(LevelId);
    await _mediator.Publish(domainEvent);
    return Result.Success();
  }
}

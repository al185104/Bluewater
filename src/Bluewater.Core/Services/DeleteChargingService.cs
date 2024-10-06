using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.ChargingAggregate;
using Bluewater.Core.ChargingAggregate.Events;
using Bluewater.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.Services;
public class DeleteChargingService(IRepository<Charging> _repository, IMediator _mediator, ILogger<DeleteChargingService> _logger) : IDeleteChargingService
{
  public async Task<Result> DeleteCharging(Guid ChargingId)
  {
    _logger.LogInformation("Deleting Charging {contributorId}", ChargingId);
    Charging? aggregateToDelete = await _repository.GetByIdAsync(ChargingId);
    if (aggregateToDelete == null) return Result.NotFound();

    await _repository.DeleteAsync(aggregateToDelete);
    var domainEvent = new ChargingDeletedEvent(ChargingId);
    await _mediator.Publish(domainEvent);
    return Result.Success();
  }
}

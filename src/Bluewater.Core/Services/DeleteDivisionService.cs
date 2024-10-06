using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.DivisionAggregate;
using Bluewater.Core.DivisionAggregate.Events;
using Bluewater.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.Services;
public class DeleteDivisionService(IRepository<Division> _repository, IMediator _mediator, ILogger<DeleteDivisionService> _logger) : IDeleteDivisionService
{
  public async Task<Result> DeleteDivision(Guid divisionId)
  {
    _logger.LogInformation("Deleting Division {contributorId}", divisionId);
    Division? aggregateToDelete = await _repository.GetByIdAsync(divisionId);
    if (aggregateToDelete == null) return Result.NotFound();

    await _repository.DeleteAsync(aggregateToDelete);
    var domainEvent = new DivisionDeletedEvent(divisionId);
    await _mediator.Publish(domainEvent);
    return Result.Success();
  }
}

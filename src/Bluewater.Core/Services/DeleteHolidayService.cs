using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.HolidayAggregate;
using Bluewater.Core.HolidayAggregate.Events;
using Bluewater.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.Services;
public class DeleteHolidayService(IRepository<Holiday> _repository, IMediator _mediator, ILogger<DeleteHolidayService> _logger) : IDeleteHolidayService
{
  public async Task<Result> DeleteHoliday(Guid HolidayId)
  {
    _logger.LogInformation("Deleting Holiday {contributorId}", HolidayId);
    Holiday? aggregateToDelete = await _repository.GetByIdAsync(HolidayId);
    if (aggregateToDelete == null) return Result.NotFound();

    await _repository.DeleteAsync(aggregateToDelete);
    var domainEvent = new HolidayDeletedEvent(HolidayId);
    await _mediator.Publish(domainEvent);
    return Result.Success();
  }
}

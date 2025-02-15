using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeTypeAggregate;
using Bluewater.Core.EmployeeTypeAggregate.Events;
using Bluewater.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.Services;
public class DeleteEmployeeTypeService(IRepository<EmployeeType> _repository, ILogger<DeleteEmployeeTypeService> _logger) : IDeleteEmployeeTypeService
{
  public async Task<Result> DeleteEmployeeType(Guid EmployeeTypeId)
  {
    _logger.LogInformation("Deleting EmployeeType {contributorId}", EmployeeTypeId);
    EmployeeType? aggregateToDelete = await _repository.GetByIdAsync(EmployeeTypeId);
    if (aggregateToDelete == null) return Result.NotFound();

    await _repository.DeleteAsync(aggregateToDelete);
    // var domainEvent = new EmployeeTypeDeletedEvent(EmployeeTypeId);
    // await _mediator.Publish(domainEvent);
    return Result.Success();
  }
}

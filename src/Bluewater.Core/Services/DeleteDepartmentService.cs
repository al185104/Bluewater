using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.DepartmentAggregate;
using Bluewater.Core.DepartmentAggregate.Events;
using Bluewater.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.Services;
public class DeleteDepartmentService(IRepository<Department> _repository, ILogger<DeleteDepartmentService> _logger) : IDeleteDepartmentService
{
  public async Task<Result> DeleteDepartment(Guid DepartmentId)
  {
    _logger.LogInformation("Deleting Department {contributorId}", DepartmentId);
    Department? aggregateToDelete = await _repository.GetByIdAsync(DepartmentId);
    if (aggregateToDelete == null) return Result.NotFound();

    await _repository.DeleteAsync(aggregateToDelete);
    // var domainEvent = new DepartmentDeletedEvent(DepartmentId);
    // await _mediator.Publish(domainEvent);
    return Result.Success();
  }
}

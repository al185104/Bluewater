using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.EmployeeAggregate.Events;
using Bluewater.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.Services;
public class DeleteEmployeeService(IRepository<Employee> _repository, ILogger<DeleteEmployeeService> _logger) : IDeleteEmployeeService
{
  public async Task<Result> DeleteEmployee(Guid EmployeeId)
  {
    _logger.LogInformation("Deleting Employee {contributorId}", EmployeeId);
    Employee? aggregateToDelete = await _repository.GetByIdAsync(EmployeeId);
    if (aggregateToDelete == null) return Result.NotFound();

    await _repository.DeleteAsync(aggregateToDelete);
    // var domainEvent = new EmployeeDeletedEvent(EmployeeId);
    // await _mediator.Publish(domainEvent);
    return Result.Success();
  }
}

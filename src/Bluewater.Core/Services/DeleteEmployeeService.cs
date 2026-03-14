using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.Services;
public class DeleteEmployeeService(IRepository<Employee> repository, ILogger<DeleteEmployeeService> logger) : IDeleteEmployeeService
{
  public async Task<Result> DeleteEmployee(Guid EmployeeId)
  {
    logger.LogInformation("Deleting Employee {contributorId}", EmployeeId);
    Employee? aggregateToDelete = await repository.GetByIdAsync(EmployeeId);
    if (aggregateToDelete == null) return Result.NotFound();

    aggregateToDelete.MarkAsDeleted();
    await repository.UpdateAsync(aggregateToDelete);

    return Result.Success();
  }
}

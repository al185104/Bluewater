using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeTypeAggregate;
using Bluewater.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.Services;

public class UpdateEmployeeTypeService(IRepository<EmployeeType> _repository, ILogger<UpdateEmployeeTypeService> _logger) : IUpdateEmployeeTypeService
{
  public async Task<Result<EmployeeType>> UpdateEmployeeTypeAsync(Guid employeeTypeId, string name, string value, bool isActive, CancellationToken cancellationToken = default)
  {
    var existing = await _repository.GetByIdAsync(employeeTypeId, cancellationToken);
    if (existing == null)
    {
      return Result.NotFound();
    }

    if (string.IsNullOrWhiteSpace(name))
    {
      return Result<EmployeeType>.Invalid(new[] { new ValidationError(nameof(name), "Employee type name is required.") });
    }

    if (string.IsNullOrWhiteSpace(value))
    {
      return Result<EmployeeType>.Invalid(new[] { new ValidationError(nameof(value), "Employee type value is required.") });
    }

    existing.UpdateEmployeeType(name, value, isActive);
    await _repository.UpdateAsync(existing, cancellationToken);

    _logger.LogInformation("Updated Employee Type {EmployeeTypeId}", employeeTypeId);

    return Result.Success(existing);
  }
}

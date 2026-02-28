using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeTypeAggregate;
using Bluewater.Core.EmployeeTypeAggregate.Specifications;
using Bluewater.Core.Helpers;
using Bluewater.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.Services;

public class CreateEmployeeTypeService(IRepository<EmployeeType> _repository, ILogger<CreateEmployeeTypeService> _logger) : ICreateEmployeeTypeService
{
  public async Task<Result<Guid>> CreateEmployeeTypeAsync(string name, string value, bool isActive, CancellationToken cancellationToken = default)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      return Result<Guid>.Invalid(new[] { ValidationErrorExtension.ToValidationError(nameof(name), "Employee type name is required.") });
    }

    if (string.IsNullOrWhiteSpace(value))
    {
      return Result<Guid>.Invalid(new[] { ValidationErrorExtension.ToValidationError(nameof(value), "Employee type value is required.") });
    }

    var existingEmployeeType = await _repository.FirstOrDefaultAsync(new EmployeeTypeByNameSpec(name), cancellationToken);
    if (existingEmployeeType != null)
    {
      return Result.Success(existingEmployeeType.Id);
    }

    var employeeType = new EmployeeType(name, value, isActive);
    var created = await _repository.AddAsync(employeeType, cancellationToken);

    _logger.LogInformation("Created Employee Type {EmployeeTypeId}", created.Id);

    return Result.Success(created.Id);
  }
}

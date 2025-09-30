using Ardalis.Result;
using Bluewater.Core.EmployeeTypeAggregate;

namespace Bluewater.Core.Interfaces;

public interface IUpdateEmployeeTypeService
{
  Task<Result<EmployeeType>> UpdateEmployeeTypeAsync(Guid employeeTypeId, string name, string value, bool isActive, CancellationToken cancellationToken = default);
}

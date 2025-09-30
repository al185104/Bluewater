using Bluewater.Core.EmployeeTypeAggregate;

namespace Bluewater.Core.Interfaces;

public interface IGetEmployeeTypeService
{
  Task<EmployeeType?> GetEmployeeTypeAsync(Guid employeeTypeId, CancellationToken cancellationToken = default);
}

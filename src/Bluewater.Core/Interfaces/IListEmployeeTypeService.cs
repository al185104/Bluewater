using Bluewater.Core.EmployeeTypeAggregate;

namespace Bluewater.Core.Interfaces;

public interface IListEmployeeTypeService
{
  Task<IEnumerable<EmployeeType>> ListEmployeeTypesAsync(CancellationToken cancellationToken = default);
}

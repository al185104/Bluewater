using System.Linq;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeTypeAggregate;
using Bluewater.Core.Interfaces;

namespace Bluewater.Core.Services;

public class ListEmployeeTypeService(IReadRepository<EmployeeType> _repository) : IListEmployeeTypeService
{
  public async Task<IEnumerable<EmployeeType>> ListEmployeeTypesAsync(CancellationToken cancellationToken = default)
  {
    var employeeTypes = await _repository.ListAsync(cancellationToken);
    return employeeTypes.AsEnumerable();
  }
}

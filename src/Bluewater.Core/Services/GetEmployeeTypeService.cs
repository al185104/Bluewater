using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeTypeAggregate;
using Bluewater.Core.EmployeeTypeAggregate.Specifications;
using Bluewater.Core.Interfaces;

namespace Bluewater.Core.Services;

public class GetEmployeeTypeService(IReadRepository<EmployeeType> _repository) : IGetEmployeeTypeService
{
  public async Task<EmployeeType?> GetEmployeeTypeAsync(Guid employeeTypeId, CancellationToken cancellationToken = default)
  {
    var spec = new EmployeeTypeByIdSpec(employeeTypeId);
    return await _repository.FirstOrDefaultAsync(spec, cancellationToken);
  }
}

using System.Linq;
using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;

namespace Bluewater.UseCases.EmployeeTypes.List;
// NOTE: CHANGED FROM ORIGINAL IMPLEMENTATION
internal class ListEmployeeTypeHandler(IListEmployeeTypeService _employeeTypeService) : IQueryHandler<ListEmployeeTypeQuery, Result<IEnumerable<EmployeeTypeDTO>>>
{
  public async Task<Result<IEnumerable<EmployeeTypeDTO>>> Handle(ListEmployeeTypeQuery request, CancellationToken cancellationToken)
  {
    var employeeTypes = await _employeeTypeService.ListEmployeeTypesAsync(cancellationToken);
    var result = employeeTypes.Select(s => new EmployeeTypeDTO(s.Id, s.Name, s.Value, s.IsActive));
    return Result.Success(result);
  }
}

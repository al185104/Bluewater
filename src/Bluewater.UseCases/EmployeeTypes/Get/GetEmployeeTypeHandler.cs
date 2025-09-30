using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;

namespace Bluewater.UseCases.EmployeeTypes.Get;
public class GetEmployeeTypeHandler(IGetEmployeeTypeService _employeeTypeService) : IQueryHandler<GetEmployeeTypeQuery, Result<EmployeeTypeDTO>>
{
  public async Task<Result<EmployeeTypeDTO>> Handle(GetEmployeeTypeQuery request, CancellationToken cancellationToken)
  {
    var entity = await _employeeTypeService.GetEmployeeTypeAsync(request.EmployeeTypeId ?? Guid.Empty, cancellationToken);
    if (entity == null) return Result.NotFound();

    return new EmployeeTypeDTO(entity.Id, entity.Name, entity.Value, entity.IsActive);
  }
}

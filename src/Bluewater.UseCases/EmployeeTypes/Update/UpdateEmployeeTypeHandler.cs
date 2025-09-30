using System.Linq;
using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;

namespace Bluewater.UseCases.EmployeeTypes.Update;
public class UpdateEmployeeTypeHandler(IUpdateEmployeeTypeService _updateEmployeeTypeService) : ICommandHandler<UpdateEmployeeTypeCommand, Result<EmployeeTypeDTO>>
{
  public async Task<Result<EmployeeTypeDTO>> Handle(UpdateEmployeeTypeCommand request, CancellationToken cancellationToken)
  {
    var updateResult = await _updateEmployeeTypeService.UpdateEmployeeTypeAsync(request.EmployeeTypeId, request.NewName, request.Value, request.IsActive, cancellationToken);

    if (!updateResult.IsSuccess)
    {
      return updateResult.Status switch
      {
        ResultStatus.NotFound => Result<EmployeeTypeDTO>.NotFound(),
        ResultStatus.Invalid => Result<EmployeeTypeDTO>.Invalid(updateResult.ValidationErrors),
        ResultStatus.Unauthorized => Result<EmployeeTypeDTO>.Unauthorized(),
        ResultStatus.Forbidden => Result<EmployeeTypeDTO>.Forbidden(),
        ResultStatus.CriticalError => Result<EmployeeTypeDTO>.CriticalError(updateResult.Errors.ToArray()),
        ResultStatus.Error => Result<EmployeeTypeDTO>.Error(updateResult.Errors.ToArray()),
        _ => Result<EmployeeTypeDTO>.Error(new[] { "Unable to update employee type." })
      };
    }

    var employeeType = updateResult.Value;
    return Result.Success(new EmployeeTypeDTO(employeeType.Id, employeeType.Name, employeeType.Value, employeeType.IsActive));
  }
}

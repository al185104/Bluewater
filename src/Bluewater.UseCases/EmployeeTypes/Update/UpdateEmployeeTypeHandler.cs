using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeTypeAggregate;

namespace Bluewater.UseCases.EmployeeTypes.Update;
public class UpdateEmployeeTypeHandler(IRepository<EmployeeType> _repository) : ICommandHandler<UpdateEmployeeTypeCommand, Result<EmployeeTypeDTO>>
{
  public async Task<Result<EmployeeTypeDTO>> Handle(UpdateEmployeeTypeCommand request, CancellationToken cancellationToken)
  {
    var existingEmployeeType = await _repository.GetByIdAsync(request.EmployeeTypeId, cancellationToken);
    if (existingEmployeeType == null)
    {
      return Result.NotFound();
    }

    existingEmployeeType.UpdateEmployeeType(request.NewName!, request.Value, request.IsActive);

    await _repository.UpdateAsync(existingEmployeeType, cancellationToken);

    return Result.Success(new EmployeeTypeDTO(existingEmployeeType.Id, existingEmployeeType.Name, existingEmployeeType.Value, existingEmployeeType.IsActive));
  }
}

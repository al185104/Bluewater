using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.DepartmentAggregate;
using Bluewater.Core.DepartmentAggregate.Specifications;

namespace Bluewater.UseCases.Departments.Update;
public class UpdateDepartmentHandler(IRepository<Department> _repository) : ICommandHandler<UpdateDepartmentCommand, Result<DepartmentDTO>>
{
  public async Task<Result<DepartmentDTO>> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
  {
    var existingDepartment = await _repository.GetByIdAsync(request.DepartmentId, cancellationToken);
    if (existingDepartment == null)
    {
      return Result.NotFound();
    }

    existingDepartment.UpdateDepartment(request.NewName!, request.Description, request.divisionId);

    await _repository.UpdateAsync(existingDepartment, cancellationToken);

    return Result.Success(new DepartmentDTO(existingDepartment.Id, existingDepartment.Name, existingDepartment.Description ?? string.Empty, existingDepartment.DivisionId));
  }
}

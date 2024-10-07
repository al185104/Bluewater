using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;

namespace Bluewater.UseCases.EmployeeTypes.Delete;
public class DeleteEmployeeTypeHandler(IDeleteEmployeeTypeService _deleteEmployeeTypeService) : ICommandHandler<DeleteEmployeeTypeCommand, Result>
{
  public async Task<Result> Handle(DeleteEmployeeTypeCommand request, CancellationToken cancellationToken)
  {
    return await _deleteEmployeeTypeService.DeleteEmployeeType(request.EmployeeTypeId);
  }
}

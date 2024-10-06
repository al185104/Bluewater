using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;

namespace Bluewater.UseCases.Departments.Delete;
public class DeleteDepartmentHandler(IDeleteDepartmentService _deleteDepartmentService) : ICommandHandler<DeleteDepartmentCommand, Result>
{
  public async Task<Result> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
  {
    return await _deleteDepartmentService.DeleteDepartment(request.DepartmentId);
  }
}

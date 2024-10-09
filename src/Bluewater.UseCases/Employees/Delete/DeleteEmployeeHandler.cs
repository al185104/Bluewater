using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;

namespace Bluewater.UseCases.Employees.Delete;
public class DeleteEmployeeHandler(IDeleteEmployeeService _deleteEmployeeService) : ICommandHandler<DeleteEmployeeCommand, Result>
{
  public async Task<Result> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
  {
    return await _deleteEmployeeService.DeleteEmployee(request.EmployeeId);
  }
}

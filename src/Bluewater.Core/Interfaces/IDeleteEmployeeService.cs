using Ardalis.Result;

namespace Bluewater.Core.Interfaces;
public interface IDeleteEmployeeService
{
  public Task<Result> DeleteEmployee(Guid EmployeeId);
}

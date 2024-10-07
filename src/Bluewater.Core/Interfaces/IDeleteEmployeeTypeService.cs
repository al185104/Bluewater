using Ardalis.Result;

namespace Bluewater.Core.Interfaces;
public interface IDeleteEmployeeTypeService
{
  public Task<Result> DeleteEmployeeType(Guid EmployeeTypeId);
}

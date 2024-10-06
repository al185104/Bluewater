using Ardalis.Result;

namespace Bluewater.Core.Interfaces;
public interface IDeleteDepartmentService
{
  public Task<Result> DeleteDepartment(Guid DepartmentId);
}

using Ardalis.Result;

namespace Bluewater.Core.Interfaces;

public interface IDeleteLeaveService
{
  public Task<Result> DeleteLeave(Guid leaveId);
}

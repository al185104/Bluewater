using Ardalis.Result;

namespace Bluewater.Core.Interfaces;
public interface IDeleteLeaveCreditService
{
  public Task<Result> DeleteLeaveCredit(Guid leaveCreditId);
}

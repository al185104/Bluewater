using Ardalis.Result;

namespace Bluewater.Core.Interfaces;
public interface IDeletePositionService
{
  public Task<Result> DeletePosition(Guid positionId);
}

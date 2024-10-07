using Ardalis.Result;

namespace Bluewater.Core.Interfaces;
public interface IDeleteLevelService
{
  public Task<Result> DeleteLevel(Guid LevelId);
}

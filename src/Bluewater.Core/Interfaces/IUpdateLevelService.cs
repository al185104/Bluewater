using Ardalis.Result;
using Bluewater.Core.LevelAggregate;

namespace Bluewater.Core.Interfaces;

public interface IUpdateLevelService
{
  Task<Result<Level>> UpdateLevelAsync(Guid levelId, string name, string value, bool isActive, CancellationToken cancellationToken = default);
}

using Bluewater.Core.LevelAggregate;

namespace Bluewater.Core.Interfaces;

public interface IGetLevelService
{
  Task<Level?> GetLevelAsync(Guid levelId, CancellationToken cancellationToken = default);
}

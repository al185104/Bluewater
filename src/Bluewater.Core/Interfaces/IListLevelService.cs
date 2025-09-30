using Bluewater.Core.LevelAggregate;

namespace Bluewater.Core.Interfaces;

public interface IListLevelService
{
  Task<IEnumerable<Level>> ListLevelsAsync(CancellationToken cancellationToken = default);
}

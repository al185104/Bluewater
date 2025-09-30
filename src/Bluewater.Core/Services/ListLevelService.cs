using System.Linq;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;
using Bluewater.Core.LevelAggregate;

namespace Bluewater.Core.Services;

public class ListLevelService(IReadRepository<Level> _repository) : IListLevelService
{
  public async Task<IEnumerable<Level>> ListLevelsAsync(CancellationToken cancellationToken = default)
  {
    var levels = await _repository.ListAsync(cancellationToken);
    return levels.AsEnumerable();
  }
}

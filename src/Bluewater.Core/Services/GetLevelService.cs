using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;
using Bluewater.Core.LevelAggregate;
using Bluewater.Core.LevelAggregate.Specifications;

namespace Bluewater.Core.Services;

public class GetLevelService(IReadRepository<Level> _repository) : IGetLevelService
{
  public async Task<Level?> GetLevelAsync(Guid levelId, CancellationToken cancellationToken = default)
  {
    var spec = new LevelByIdSpec(levelId);
    return await _repository.FirstOrDefaultAsync(spec, cancellationToken);
  }
}

using Ardalis.Result;
using System.Linq;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;

namespace Bluewater.UseCases.Levels.List;

internal class ListLevelHandler(IListLevelService _levelService) : IQueryHandler<ListLevelQuery, Result<IEnumerable<LevelDTO>>>
{
  public async Task<Result<IEnumerable<LevelDTO>>> Handle(ListLevelQuery request, CancellationToken cancellationToken)
  {
    var levels = await _levelService.ListLevelsAsync(cancellationToken);
    var result = levels.Select(s => new LevelDTO(s.Id, s.Name, s.Value, s.IsActive));
    return Result.Success(result);
  }
}

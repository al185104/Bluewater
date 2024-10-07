using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.LevelAggregate;

namespace Bluewater.UseCases.Levels.List;

internal class ListLevelHandler(IRepository<Level> _repository) : IQueryHandler<ListLevelQuery, Result<IEnumerable<LevelDTO>>>
{
  public async Task<Result<IEnumerable<LevelDTO>>> Handle(ListLevelQuery request, CancellationToken cancellationToken)
  {
    var result = (await _repository.ListAsync(cancellationToken)).Select(s => new LevelDTO(s.Id, s.Name, s.Value, s.IsActive));
    return Result.Success(result);
  }
}

using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.LevelAggregate;
using Bluewater.Core.LevelAggregate.Specifications;

namespace Bluewater.UseCases.Levels.Get;
public class GetLevelHandler(IRepository<Level> _repository) : IQueryHandler<GetLevelQuery, Result<LevelDTO>>
{
  public async Task<Result<LevelDTO>> Handle(GetLevelQuery request, CancellationToken cancellationToken)
  {
    var spec = new LevelByIdSpec(request.LevelId);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null) return Result.NotFound();

    return new LevelDTO(entity.Id, entity.Name, entity.Value, entity.IsActive);
  }
}

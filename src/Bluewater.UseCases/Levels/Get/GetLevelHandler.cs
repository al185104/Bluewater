using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;

namespace Bluewater.UseCases.Levels.Get;
public class GetLevelHandler(IGetLevelService _levelService) : IQueryHandler<GetLevelQuery, Result<LevelDTO>>
{
  public async Task<Result<LevelDTO>> Handle(GetLevelQuery request, CancellationToken cancellationToken)
  {
    var entity = await _levelService.GetLevelAsync(request.LevelId ?? Guid.Empty, cancellationToken);
    if (entity == null) return Result.NotFound();

    return new LevelDTO(entity.Id, entity.Name, entity.Value, entity.IsActive);
  }
}

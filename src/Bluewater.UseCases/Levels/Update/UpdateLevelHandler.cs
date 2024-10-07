using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.LevelAggregate;

namespace Bluewater.UseCases.Levels.Update;
public class UpdateLevelHandler(IRepository<Level> _repository) : ICommandHandler<UpdateLevelCommand, Result<LevelDTO>>
{
  public async Task<Result<LevelDTO>> Handle(UpdateLevelCommand request, CancellationToken cancellationToken)
  {
    var existingLevel = await _repository.GetByIdAsync(request.LevelId, cancellationToken);
    if (existingLevel == null)
    {
      return Result.NotFound();
    }

    existingLevel.UpdateLevel(request.NewName!, request.Value, request.IsActive);

    await _repository.UpdateAsync(existingLevel, cancellationToken);

    return Result.Success(new LevelDTO(existingLevel.Id, existingLevel.Name, existingLevel.Value, existingLevel.IsActive));
  }
}

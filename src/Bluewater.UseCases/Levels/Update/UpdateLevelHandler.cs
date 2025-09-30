using System.Linq;
using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;

namespace Bluewater.UseCases.Levels.Update;
public class UpdateLevelHandler(IUpdateLevelService _updateLevelService) : ICommandHandler<UpdateLevelCommand, Result<LevelDTO>>
{
  public async Task<Result<LevelDTO>> Handle(UpdateLevelCommand request, CancellationToken cancellationToken)
  {
    var updateResult = await _updateLevelService.UpdateLevelAsync(request.LevelId, request.NewName, request.Value, request.IsActive, cancellationToken);

    if (!updateResult.IsSuccess)
    {
      return updateResult.Status switch
      {
        ResultStatus.NotFound => Result<LevelDTO>.NotFound(),
        ResultStatus.Invalid => Result<LevelDTO>.Invalid(updateResult.ValidationErrors),
        ResultStatus.Unauthorized => Result<LevelDTO>.Unauthorized(),
        ResultStatus.Forbidden => Result<LevelDTO>.Forbidden(),
        ResultStatus.CriticalError => Result<LevelDTO>.CriticalError(updateResult.Errors.ToArray()),
        ResultStatus.Error => Result<LevelDTO>.Error(updateResult.Errors.First()),
        _ => Result<LevelDTO>.Error("Unable to update level.")
      };
    }

    var level = updateResult.Value;
    return Result.Success(new LevelDTO(level.Id, level.Name, level.Value, level.IsActive));
  }
}

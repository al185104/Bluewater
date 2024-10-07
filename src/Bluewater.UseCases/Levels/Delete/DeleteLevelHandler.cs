using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;

namespace Bluewater.UseCases.Levels.Delete;
public class DeleteLevelHandler(IDeleteLevelService _deleteLevelService) : ICommandHandler<DeleteLevelCommand, Result>
{
  public async Task<Result> Handle(DeleteLevelCommand request, CancellationToken cancellationToken)
  {
    return await _deleteLevelService.DeleteLevel(request.LevelId);
  }
}

using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;
using Bluewater.UseCases.Levels.Create;

namespace Bluewater.UseCases.Contributors.Create;
public class CreateLevelHandler(ICreateLevelService _createLevelService) : ICommandHandler<CreateLevelCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateLevelCommand request, CancellationToken cancellationToken)
  {
    return await _createLevelService.CreateLevelAsync(request.Name, request.Value, request.IsActive, cancellationToken);
  }
}

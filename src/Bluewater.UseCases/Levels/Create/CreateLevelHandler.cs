using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.LevelAggregate;
using Bluewater.UseCases.Levels.Create;

namespace Bluewater.UseCases.Contributors.Create;
public class CreateLevelHandler(IRepository<Level> _repository) : ICommandHandler<CreateLevelCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateLevelCommand request, CancellationToken cancellationToken)
  {
    var newLevel = new Level(request.Name, request.Value, request.IsActive);
    var createdItem = await _repository.AddAsync(newLevel, cancellationToken);
    return createdItem.Id;
  }
}

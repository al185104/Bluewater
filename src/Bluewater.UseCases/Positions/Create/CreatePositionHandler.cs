using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.PositionAggregate;
using Bluewater.Core.PositionAggregate.Specifications;
using Bluewater.UseCases.Positions.Create;

namespace Bluewater.UseCases.Contributors.Create;
public class CreatePositionHandler(IRepository<Position> _repository) : ICommandHandler<CreatePositionCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreatePositionCommand request, CancellationToken cancellationToken)
  {
    var existingPosition = await _repository.FirstOrDefaultAsync(new PositionByNameSpec(request.Name), cancellationToken);
    if (existingPosition != null)
    {
      return existingPosition.Id;
    }

    var newPosition = new Position(request.Name, request.Description, request.sectionId);
    var createdItem = await _repository.AddAsync(newPosition, cancellationToken);
    return createdItem.Id;
  }
}

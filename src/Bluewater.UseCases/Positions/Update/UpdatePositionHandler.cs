using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.PositionAggregate;

namespace Bluewater.UseCases.Positions.Update;
public class UpdatePositionHandler(IRepository<Position> _repository) : ICommandHandler<UpdatePositionCommand, Result<PositionDTO>>
{
  public async Task<Result<PositionDTO>> Handle(UpdatePositionCommand request, CancellationToken cancellationToken)
  {
    var existingPosition = await _repository.GetByIdAsync(request.PositionId, cancellationToken);
    if (existingPosition == null)
      return Result.NotFound();

    existingPosition.UpdatePosition(request.NewName!, request.Description, request.SectionId);

    await _repository.UpdateAsync(existingPosition, cancellationToken);

    return Result.Success(new PositionDTO(existingPosition.Id, existingPosition.Name, existingPosition.Description ?? string.Empty, existingPosition.SectionId));
  }
}

using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;

namespace Bluewater.UseCases.Positions.Delete;
public class DeletePositionHandler(IDeletePositionService _deletePositionService) : ICommandHandler<DeletePositionCommand, Result>
{
  public async Task<Result> Handle(DeletePositionCommand request, CancellationToken cancellationToken)
  {
    return await _deletePositionService.DeletePosition(request.PositionId);
  }
}

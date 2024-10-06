using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;

namespace Bluewater.UseCases.Chargings.Delete;
public class DeleteChargingHandler(IDeleteChargingService _deleteChargingService) : ICommandHandler<DeleteChargingCommand, Result>
{
  public async Task<Result> Handle(DeleteChargingCommand request, CancellationToken cancellationToken)
  {
    return await _deleteChargingService.DeleteCharging(request.ChargingId);
  }
}

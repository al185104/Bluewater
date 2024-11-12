using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.ChargingAggregate;

namespace Bluewater.UseCases.Chargings.Update;
public class UpdateChargingHandler(IRepository<Charging> _repository) : ICommandHandler<UpdateChargingCommand, Result<ChargingDTO>>
{
  public async Task<Result<ChargingDTO>> Handle(UpdateChargingCommand request, CancellationToken cancellationToken)
  {
    var existingCharging = await _repository.GetByIdAsync(request.ChargingId, cancellationToken);
    if (existingCharging == null)
    {
      return Result.NotFound();
    }

    existingCharging.UpdateCharging(request.NewName!, request.Description, request.deptId);

    await _repository.UpdateAsync(existingCharging, cancellationToken);

    return Result.Success(new ChargingDTO(existingCharging.Id, existingCharging.Name, existingCharging.Description ?? string.Empty, existingCharging.DepartmentId));
  }
}

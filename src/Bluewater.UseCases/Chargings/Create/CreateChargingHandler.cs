using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.ChargingAggregate;
using Bluewater.Core.ChargingAggregate.Specifications;
using Bluewater.UseCases.Chargings.Create;

namespace Bluewater.UseCases.Contributors.Create;
public class CreateChargingHandler(IRepository<Charging> _repository) : ICommandHandler<CreateChargingCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateChargingCommand request, CancellationToken cancellationToken)
  {
    var existingCharging = await _repository.FirstOrDefaultAsync(new ChargingByNameSpec(request.Name), cancellationToken);
    if (existingCharging != null)
    {
      return existingCharging.Id;
    }

    var newCharging = new Charging(request.Name, request.Description, request.DeptId);
    var createdItem = await _repository.AddAsync(newCharging, cancellationToken);
    return createdItem.Id;
  }
}

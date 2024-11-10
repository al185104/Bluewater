using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.ServiceChargeAggregate;

namespace Bluewater.UseCases.ServiceCharges.Create;

public class CreateServiceChargeHandler(IRepository<ServiceCharge> _repository) : ICommandHandler<CreateServiceChargeCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateServiceChargeCommand request, CancellationToken cancellationToken)
  {
    var newServiceCharge = new ServiceCharge(request.username, request.svcCharge, request.date);
    var createdItem = await _repository.AddAsync(newServiceCharge, cancellationToken);
    return createdItem.Id;
  }
}

using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.ServiceChargeAggregate;

namespace Bluewater.UseCases.ServiceCharges.Delete;
public class DeleteServiceChargeHandler(IRepository<ServiceCharge> _repository) : ICommandHandler<DeleteServiceChargeCommand, Result>
{
  public async Task<Result> Handle(DeleteServiceChargeCommand request, CancellationToken cancellationToken)
  {
    ServiceCharge? aggregateToDelete = await _repository.GetByIdAsync(request.ServiceChargeId);
    if (aggregateToDelete == null) return Result.NotFound();
    await _repository.DeleteAsync(aggregateToDelete);
    return Result.Success();
  }
}


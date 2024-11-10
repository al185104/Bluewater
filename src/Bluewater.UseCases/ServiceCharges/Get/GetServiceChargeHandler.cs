using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.ServiceChargeAggregate;

namespace Bluewater.UseCases.ServiceCharges.Get;
public class GetServiceChargeHandler(IRepository<ServiceCharge> _repository) : IQueryHandler<GetServiceChargeQuery, Result<ServiceChargeDTO>>
{
  public async Task<Result<ServiceChargeDTO>> Handle(GetServiceChargeQuery request, CancellationToken cancellationToken)
  {
    var result = await _repository.GetByIdAsync(request.ServiceChargeId, cancellationToken);
    if (result == null) return Result.NotFound();

    return new ServiceChargeDTO(result!.Id, result.Username, result.Amount, result.Date);
  }
}

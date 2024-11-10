using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.ServiceChargeAggregate;
using Bluewater.Core.ServiceChargeAggregate.Specifications;

namespace Bluewater.UseCases.ServiceCharges.List;

internal class ListServiceChargeHandler(IRepository<ServiceCharge> _repository) : IQueryHandler<ListServiceChargeQuery, Result<IEnumerable<ServiceChargeDTO>>>
{
  public async Task<Result<IEnumerable<ServiceChargeDTO>>> Handle(ListServiceChargeQuery request, CancellationToken cancellationToken)
  {
    var spec = new ServiceChargeByDateSpec(request.date);
    var serviceCharges = await _repository.ListAsync(spec, cancellationToken);
    if (serviceCharges == null) return Result<IEnumerable<ServiceChargeDTO>>.NotFound();

    var serviceChargeDTOs = serviceCharges.Select(sc => new ServiceChargeDTO(sc.Id, sc.Username, sc.Amount, sc.Date)).ToList();
    return Result<IEnumerable<ServiceChargeDTO>>.Success(serviceChargeDTOs);
  }
}

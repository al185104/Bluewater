using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.ChargingAggregate;
using Bluewater.Core.ChargingAggregate.Specifications;

namespace Bluewater.UseCases.Chargings.Get;
public class GetChargingHandler(IRepository<Charging> _repository) : IQueryHandler<GetChargingQuery, Result<ChargingDTO>>
{
  public async Task<Result<ChargingDTO>> Handle(GetChargingQuery request, CancellationToken cancellationToken)
  {
    var spec = new ChargingByIdSpec(request.ChargingId);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null) return Result.NotFound();

    return new ChargingDTO(entity.Id, entity.Name, entity.Description ?? string.Empty);
  }
}

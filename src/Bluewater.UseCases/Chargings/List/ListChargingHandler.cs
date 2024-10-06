using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Chargings.List;
internal class ListChargingHandler(IListChargingQueryService _queryService) : IQueryHandler<ListChargingQuery, Result<IEnumerable<ChargingDTO>>>
{
  public async Task<Result<IEnumerable<ChargingDTO>>> Handle(ListChargingQuery request, CancellationToken cancellationToken)
  {
    var result = await _queryService.ListAsync();
    return Result.Success(result);
  }
}

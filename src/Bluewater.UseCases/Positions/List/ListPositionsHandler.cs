using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Positions.List;
public class ListPositionsHandler(IListPositionsQueryService _query) : IQueryHandler<ListPositionsQuery, Result<IEnumerable<PositionDTO>>>
{
  public async Task<Result<IEnumerable<PositionDTO>>> Handle(ListPositionsQuery request, CancellationToken cancellationToken)
  {
    var result = await _query.ListAsync();
    return Result.Success(result);
  }
}

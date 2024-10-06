using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Divisions.List;
internal class ListDivisionsHandler(IListDivisionsQueryService _query) : IQueryHandler<ListDivisionsQuery, Result<IEnumerable<DivisionDTO>>>
{
  public async Task<Result<IEnumerable<DivisionDTO>>> Handle(ListDivisionsQuery request, CancellationToken cancellationToken)
  {
    var result = await _query.ListAsync();

    return Result.Success(result);
  }
}

using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Shifts.List;
internal class ListShiftHandler(IListShiftQueryService _queryService) : IQueryHandler<ListShiftQuery, Result<IEnumerable<ShiftDTO>>>
{
  public async Task<Result<IEnumerable<ShiftDTO>>> Handle(ListShiftQuery request, CancellationToken cancellationToken)
  {
    var result = await _queryService.ListAsync();
    return Result.Success(result);
  }
}

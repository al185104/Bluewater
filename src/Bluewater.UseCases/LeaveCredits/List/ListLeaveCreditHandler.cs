
using System.Linq;
using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;

namespace Bluewater.UseCases.LeaveCredits.List;

public class ListLeaveCreditHandler(IListLeaveCreditService _leaveCreditService) : IQueryHandler<ListLeaveCreditQuery, Result<IEnumerable<LeaveCreditDTO>>>
{
  public async Task<Result<IEnumerable<LeaveCreditDTO>>> Handle(ListLeaveCreditQuery request, CancellationToken cancellationToken)
  {
    var leaveCredits = await _leaveCreditService.ListLeaveCreditsAsync(cancellationToken);
    var result = leaveCredits.Select(s => new LeaveCreditDTO(s.Id, s.LeaveCode, s.LeaveDescription, s.DefaultCredits, s.SortOrder, s.IsLeaveWithPay, s.IsCanCarryOver));
    return Result.Success(result);
  }
}

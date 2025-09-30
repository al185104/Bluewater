using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;
using Bluewater.UseCases.LeaveCredits;

namespace Bluewater.UseCases.LeaveCredits.Get;

public class GetLeaveCreditHandler(IGetLeaveCreditService _leaveCreditService) : IQueryHandler<GetLeaveCreditQuery, Result<LeaveCreditDTO>>
{
  public async Task<Result<LeaveCreditDTO>> Handle(GetLeaveCreditQuery request, CancellationToken cancellationToken)
  {
    var entity = await _leaveCreditService.GetLeaveCreditAsync(request.LeaveCreditId ?? Guid.Empty, cancellationToken);
    if (entity == null) return Result.NotFound();

    return new LeaveCreditDTO(entity.Id, entity.LeaveCode, entity.LeaveDescription, entity.DefaultCredits, entity.SortOrder, entity.IsLeaveWithPay, entity.IsCanCarryOver);
  }
}

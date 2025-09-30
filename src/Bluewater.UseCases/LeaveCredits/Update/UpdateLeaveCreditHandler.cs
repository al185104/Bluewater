using System.Linq;
using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;
using Bluewater.UseCases.LeaveCredits;

namespace Bluewater.UseCases.LeaveCredits.Update;

public class UpdateLeaveCreditHandler(IUpdateLeaveCreditService _updateLeaveCreditService) : ICommandHandler<UpdateLeaveCreditCommand, Result<LeaveCreditDTO>>
{
  public async Task<Result<LeaveCreditDTO>> Handle(UpdateLeaveCreditCommand request, CancellationToken cancellationToken)
  {
    var updateResult = await _updateLeaveCreditService.UpdateLeaveCreditAsync(
      request.LeaveCreditId,
      request.Code,
      request.Description,
      request.Credit,
      request.SortOrder,
      request.IsLeaveWithPay,
      request.IsCanCarryOver,
      cancellationToken);

    if (!updateResult.IsSuccess)
    {
      return updateResult.Status switch
      {
        ResultStatus.NotFound => Result<LeaveCreditDTO>.NotFound(),
        ResultStatus.Invalid => Result<LeaveCreditDTO>.Invalid(updateResult.ValidationErrors),
        ResultStatus.Unauthorized => Result<LeaveCreditDTO>.Unauthorized(),
        ResultStatus.Forbidden => Result<LeaveCreditDTO>.Forbidden(),
        ResultStatus.CriticalError => Result<LeaveCreditDTO>.CriticalError(updateResult.Errors.ToArray()),
        ResultStatus.Error => Result<LeaveCreditDTO>.Error(updateResult.Errors.ToArray()),
        _ => Result<LeaveCreditDTO>.Error(new[] { "Unable to update leave credit." })
      };
    }

    var leaveCredit = updateResult.Value;
    return Result.Success(new LeaveCreditDTO(leaveCredit.Id, leaveCredit.LeaveCode, leaveCredit.LeaveDescription, leaveCredit.DefaultCredits, leaveCredit.SortOrder, leaveCredit.IsLeaveWithPay, leaveCredit.IsCanCarryOver));
  }
}

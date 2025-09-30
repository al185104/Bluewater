using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;

namespace Bluewater.UseCases.LeaveCredits.Create;

public class CreateLeaveCreditHandler(ICreateLeaveCreditService _createLeaveCreditService) : ICommandHandler<CreateLeaveCreditCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateLeaveCreditCommand request, CancellationToken cancellationToken)
  {
    return await _createLeaveCreditService.CreateLeaveCredit(
      request.Code,
      request.Description,
      request.Credit,
      request.SortOrder,
      request.IsLeaveWithPay,
      request.IsCanCarryOver,
      cancellationToken);
  }
}

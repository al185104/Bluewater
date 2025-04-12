using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;

namespace Bluewater.UseCases.LeaveCredits.Delete;

public class DeleteLeaveCreditHandler(IDeleteLeaveCreditService _deleteLeaveCreditService) : ICommandHandler<DeleteLeaveCreditCommand, Result>
{
  public async Task<Result> Handle(DeleteLeaveCreditCommand request, CancellationToken cancellationToken)
  {
    return await _deleteLeaveCreditService.DeleteLeaveCredit(request.LeaveCreditId);
  }
}

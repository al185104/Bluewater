using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;

namespace Bluewater.UseCases.Leaves.Delete;

public class DeleteLeaveHandler(IDeleteLeaveService _deleteLeaveService) : ICommandHandler<DeleteLeaveCommand, Result>
{
  public async Task<Result> Handle(DeleteLeaveCommand request, CancellationToken cancellationToken)
  {
    return await _deleteLeaveService.DeleteLeave(request.LeaveId);
  }
}

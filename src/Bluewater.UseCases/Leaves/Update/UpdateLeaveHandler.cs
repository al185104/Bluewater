using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.Enum;
using Bluewater.Core.Forms.LeaveAggregate;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Leaves.Update;
public class UpdateLeaveHandler(IRepository<Leave> _repository) : ICommandHandler<UpdateLeaveCommand, Result<LeaveDTO>>
{
  public async Task<Result<LeaveDTO>> Handle(UpdateLeaveCommand request, CancellationToken cancellationToken)
  {
    var existingLeave = await _repository.GetByIdAsync(request.LeaveId, cancellationToken);
    if (existingLeave == null)
    {
      return Result.NotFound();
    }

    existingLeave.UpdateLeave(request.employeeId, request.leaveCreditId, request.startDate, request.endDate, request.isHalfDay, (ApplicationStatus)request.status);
    await _repository.UpdateAsync(existingLeave, cancellationToken);

    return Result.Success(new LeaveDTO(existingLeave.Id, existingLeave.StartDate, existingLeave.EndDate, existingLeave.IsHalfDay, (ApplicationStatusDTO)existingLeave.Status, existingLeave.EmployeeId ?? Guid.Empty, existingLeave.LeaveCreditId));
  }
}

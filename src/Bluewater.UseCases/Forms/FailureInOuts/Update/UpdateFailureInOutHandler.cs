using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.FailureInOutAggregate;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.FailureInOuts.Update;
public class UpdateFailureInOutHandler(IRepository<FailureInOut> _repository) : ICommandHandler<UpdateFailureInOutCommand, Result<FailureInOutDTO>>
{
  public async Task<Result<FailureInOutDTO>> Handle(UpdateFailureInOutCommand request, CancellationToken cancellationToken)
  {
    var existingFailureInOut = await _repository.GetByIdAsync(request.id, cancellationToken);
    if (existingFailureInOut == null)
    {
      return Result.NotFound();
    }

    existingFailureInOut.UpdateFailureInOut(request.empId, request.date, request.remarks, request.reason, request.status);

    await _repository.UpdateAsync(existingFailureInOut, cancellationToken);

    return Result.Success(new FailureInOutDTO(existingFailureInOut.Id, existingFailureInOut.EmployeeId, existingFailureInOut.Date, existingFailureInOut.Remarks, (FailureInOutReasonDTO?)existingFailureInOut.Reason, (ApplicationStatusDTO?)existingFailureInOut.Status));
  }
}

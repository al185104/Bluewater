using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.LeaveAggregate;

namespace Bluewater.UseCases.Leaves.Create;

public class CreateLeaveHandler(IRepository<Leave> _repository) : ICommandHandler<CreateLeaveCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateLeaveCommand request, CancellationToken cancellationToken)
  {
    if(request.startDate == null || request.endDate == null)
    {
      return Result<Guid>.Invalid(new[] { new ValidationError("Start date and end date are required.") });
    }

    var newLeave = new Leave(request.employeeId, request.leaveCreditId, request.startDate ?? DateTime.MinValue, request.endDate ?? DateTime.MinValue, request.isHalfDay);
    var createdItem = await _repository.AddAsync(newLeave, cancellationToken);
    return createdItem.Id;
  }
}

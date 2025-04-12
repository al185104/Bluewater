using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.LeaveCreditAggregate;

namespace Bluewater.UseCases.LeaveCredits.Create;

public class CreateLeaveCreditHandler(IRepository<LeaveCredit> _repository) : ICommandHandler<CreateLeaveCreditCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateLeaveCreditCommand request, CancellationToken cancellationToken)
  {
    // assertions
    if(string.IsNullOrWhiteSpace(request.Code))
      return Result<Guid>.CriticalError(request.Code, "Leave code is required.");
    if (string.IsNullOrWhiteSpace(request.Description))
      return Result<Guid>.CriticalError(request.Description, "Leave description is required.");
    if (request.Credit <= 0)
      return Result<Guid>.CriticalError(request.Credit.ToString(), "Leave credit must be greater than 0.");
    if (request.SortOrder < 0)
      return Result<Guid>.CriticalError(request.SortOrder.ToString(), "Sort order must be greater than or equal to 0.");
    if (request.SortOrder > 100)
      return Result<Guid>.CriticalError(request.SortOrder.ToString(), "Sort order must be less than or equal to 100.");

    var newLeaveCredit = new LeaveCredit(request.Code, request.Description, request.Credit ?? 0.00m, request.IsLeaveWithPay, request.IsCanCarryOver, request.SortOrder ?? 0);
    var createdItem = await _repository.AddAsync(newLeaveCredit, cancellationToken);
    return createdItem.Id;
  }
}

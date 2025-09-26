using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Leaves;

public class DeleteLeaveValidator : Validator<DeleteLeaveRequest>
{
  public DeleteLeaveValidator()
  {
    RuleFor(x => x.LeaveId)
      .NotEmpty().WithMessage("Leave ID is required and cannot be an empty GUID.");
  }
}

using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.LeaveCredits;

public class DeleteLeaveCreditValidator : Validator<DeleteLeaveCreditRequest>
{
  public DeleteLeaveCreditValidator()
  {
    RuleFor(x => x.LeaveCreditId)
      .NotEmpty().WithMessage("Leave credit ID is required and cannot be an empty GUID.");
  }
}

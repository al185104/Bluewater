using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Shifts;

public class DeleteShiftValidator : Validator<DeleteShiftRequest>
{
  public DeleteShiftValidator()
  {
    RuleFor(x => x.ShiftId)
      .NotEmpty().WithMessage("Shift ID is required and cannot be an empty GUID.");
  }
}

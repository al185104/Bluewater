using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Shifts;

public class GetShiftValidator : Validator<GetShiftByIdRequest>
{
  public GetShiftValidator()
  {
    RuleFor(x => x.ShiftId)
      .NotEmpty().WithMessage("Shift ID is required and cannot be an empty GUID.");
  }
}

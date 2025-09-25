using Bluewater.Infrastructure.Data.Config;
using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Shifts;

public class UpdateShiftValidator : Validator<UpdateShiftRequest>
{
  public UpdateShiftValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty()
      .WithMessage("Name is required.")
      .MinimumLength(2)
      .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

    RuleFor(x => x.ShiftId)
      .Must((args, shiftId) => args.Id == shiftId)
      .WithMessage("Route and body Ids must match; cannot update Id of an existing resource.");

    RuleFor(x => x.BreakHours)
      .GreaterThanOrEqualTo(0).WithMessage("Break hours cannot be negative.");
  }
}

using Bluewater.Infrastructure.Data.Config;
using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Shifts;

public class CreateShiftValidator : Validator<CreateShiftRequest>
{
  public CreateShiftValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty()
      .WithMessage("Name is required.")
      .MinimumLength(1)
      .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

    RuleFor(x => x.BreakHours)
      .GreaterThanOrEqualTo(0).WithMessage("Break hours cannot be negative.");
  }
}

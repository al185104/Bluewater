using Bluewater.Infrastructure.Data.Config;
using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Holidays;

public class CreateHolidayValidator : Validator<CreateHolidayRequest>
{
  public CreateHolidayValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty()
      .WithMessage("Name is required.")
      .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

    RuleFor(x => x.Date)
      .NotEmpty()
      .Must(date => date != default)
      .WithMessage("Date is required.");
  }
}

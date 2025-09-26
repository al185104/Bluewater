using Bluewater.Infrastructure.Data.Config;
using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Holidays;

public class UpdateHolidayValidator : Validator<UpdateHolidayRequest>
{
  public UpdateHolidayValidator()
  {
    RuleFor(x => x.HolidayId)
      .NotEmpty().WithMessage("Holiday ID is required and cannot be an empty GUID.");

    RuleFor(x => x.Id)
      .NotEmpty().WithMessage("Holiday ID is required.");

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

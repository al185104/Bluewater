using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Holidays;

public class DeleteHolidayValidator : Validator<DeleteHolidayRequest>
{
  public DeleteHolidayValidator()
  {
    RuleFor(x => x.HolidayId)
      .NotEmpty().WithMessage("Holiday ID is required and cannot be an empty GUID.");
  }
}

using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Holidays;

public class GetHolidayValidator : Validator<GetHolidayByIdRequest>
{
  public GetHolidayValidator()
  {
    RuleFor(x => x.HolidayId)
      .NotEmpty().WithMessage("Holiday ID is required and cannot be an empty GUID.");
  }
}

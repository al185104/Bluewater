using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Schedules;

public class GetScheduleValidator : Validator<GetScheduleRequest>
{
  public GetScheduleValidator()
  {
    RuleFor(x => x.ScheduleId).NotEmpty().WithMessage("Schedule ID is required.");
  }
}

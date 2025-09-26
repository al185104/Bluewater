using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Schedules;

public class DeleteScheduleValidator : Validator<DeleteScheduleRequest>
{
  public DeleteScheduleValidator()
  {
    RuleFor(x => x.ScheduleId).NotEmpty().WithMessage("Schedule ID is required.");
  }
}

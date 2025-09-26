using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Schedules;

public class CreateScheduleValidator : Validator<CreateScheduleRequest>
{
  public CreateScheduleValidator()
  {
    RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("Employee ID is required.");
    RuleFor(x => x.ShiftId).NotEmpty().WithMessage("Shift ID is required.");
    RuleFor(x => x.ScheduleDate).NotEmpty().WithMessage("Schedule date is required.");
  }
}

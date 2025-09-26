using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Schedules;

public class UpdateScheduleValidator : Validator<UpdateScheduleRequest>
{
  public UpdateScheduleValidator()
  {
    RuleFor(x => x.ScheduleId).NotEmpty().WithMessage("Schedule ID is required.");
    RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("Employee ID is required.");
    RuleFor(x => x.ShiftId).NotEmpty().WithMessage("Shift ID is required.");
    RuleFor(x => x.ScheduleDate).NotEmpty().WithMessage("Schedule date is required.");
  }
}

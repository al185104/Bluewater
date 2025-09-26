using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Attendances;

public class CreateAttendanceValidator : Validator<CreateAttendanceRequest>
{
  public CreateAttendanceValidator()
  {
    RuleFor(x => x.EmployeeId)
      .NotEmpty().WithMessage("Employee ID is required.");

    RuleFor(x => x.WorkHrs)
      .GreaterThanOrEqualTo(0).When(x => x.WorkHrs.HasValue)
      .WithMessage("Work hours cannot be negative.");

    RuleFor(x => x.LateHrs)
      .GreaterThanOrEqualTo(0).When(x => x.LateHrs.HasValue)
      .WithMessage("Late hours cannot be negative.");

    RuleFor(x => x.UnderHrs)
      .GreaterThanOrEqualTo(0).When(x => x.UnderHrs.HasValue)
      .WithMessage("Undertime hours cannot be negative.");

    RuleFor(x => x.OverbreakHrs)
      .GreaterThanOrEqualTo(0).When(x => x.OverbreakHrs.HasValue)
      .WithMessage("Overbreak hours cannot be negative.");

    RuleFor(x => x.NightShiftHrs)
      .GreaterThanOrEqualTo(0).When(x => x.NightShiftHrs.HasValue)
      .WithMessage("Night shift hours cannot be negative.");
  }
}

using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Attendances;

public class UpdateAttendanceValidator : Validator<UpdateAttendanceRequest>
{
  public UpdateAttendanceValidator()
  {
    RuleFor(x => x.EmployeeId)
      .NotEmpty().WithMessage("Employee ID is required.");

    RuleFor(x => x.EntryDate)
      .NotNull().WithMessage("Entry date is required.");
  }
}

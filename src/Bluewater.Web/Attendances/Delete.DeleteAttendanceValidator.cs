using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Attendances;

public class DeleteAttendanceValidator : Validator<DeleteAttendanceRequest>
{
  public DeleteAttendanceValidator()
  {
    RuleFor(x => x.AttendanceId)
      .NotEmpty().WithMessage("Attendance ID is required.");
  }
}

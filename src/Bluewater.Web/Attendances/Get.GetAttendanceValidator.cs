using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Attendances;

public class GetAttendanceValidator : Validator<GetAttendanceByIdRequest>
{
  public GetAttendanceValidator()
  {
    RuleFor(x => x.AttendanceId)
      .NotEmpty().WithMessage("Attendance ID is required.");
  }
}

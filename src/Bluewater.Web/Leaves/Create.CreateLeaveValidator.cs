using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Leaves;

public class CreateLeaveValidator : Validator<CreateLeaveRequest>
{
  public CreateLeaveValidator()
  {
    RuleFor(x => x.StartDate)
      .NotNull().WithMessage("Start date is required.");

    RuleFor(x => x.EndDate)
      .NotNull().WithMessage("End date is required.");

    RuleFor(x => x)
      .Must(x => !x.StartDate.HasValue || !x.EndDate.HasValue || x.EndDate >= x.StartDate)
      .WithMessage("End date must be greater than or equal to start date.");

    RuleFor(x => x.EmployeeId)
      .NotEmpty().WithMessage("Employee ID is required and cannot be an empty GUID.");

    RuleFor(x => x.LeaveCreditId)
      .NotEmpty().WithMessage("Leave credit ID is required and cannot be an empty GUID.");
  }
}

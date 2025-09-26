using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Leaves;

public class UpdateLeaveValidator : Validator<UpdateLeaveRequest>
{
  public UpdateLeaveValidator()
  {
    RuleFor(x => x.LeaveId)
      .NotEmpty().WithMessage("Leave ID is required and cannot be an empty GUID.");

    RuleFor(x => x.Id)
      .NotEmpty().WithMessage("Leave ID is required.");

    RuleFor(x => x.StartDate)
      .NotEmpty().WithMessage("Start date is required.");

    RuleFor(x => x.EndDate)
      .NotEmpty().WithMessage("End date is required.");

    RuleFor(x => x)
      .Must(x => x.EndDate >= x.StartDate)
      .WithMessage("End date must be greater than or equal to start date.");

    RuleFor(x => x.EmployeeId)
      .NotEmpty().WithMessage("Employee ID is required and cannot be an empty GUID.");

    RuleFor(x => x.LeaveCreditId)
      .NotEmpty().WithMessage("Leave credit ID is required and cannot be an empty GUID.");
  }
}

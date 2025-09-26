using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Payrolls;

public class CreatePayrollValidator : Validator<CreatePayrollRequest>
{
  public CreatePayrollValidator()
  {
    RuleFor(x => x.EmployeeId)
      .NotEmpty().WithMessage("Employee ID is required.");

    RuleFor(x => x.Date)
      .NotEmpty().WithMessage("Date is required.");
  }
}

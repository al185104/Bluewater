using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.ServiceCharges;

public class CreateServiceChargeValidator : Validator<CreateServiceChargeRequest>
{
  public CreateServiceChargeValidator()
  {
    RuleFor(x => x.Username)
      .NotEmpty().WithMessage("Username is required.");

    RuleFor(x => x.Amount)
      .GreaterThanOrEqualTo(0).WithMessage("Amount must be greater than or equal to zero.");

    RuleFor(x => x.Date)
      .NotEmpty().WithMessage("Date is required.");
  }
}

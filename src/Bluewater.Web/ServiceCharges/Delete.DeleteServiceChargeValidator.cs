using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.ServiceCharges;

public class DeleteServiceChargeValidator : Validator<DeleteServiceChargeRequest>
{
  public DeleteServiceChargeValidator()
  {
    RuleFor(x => x.ServiceChargeId).NotEmpty().WithMessage("Service charge ID is required.");
  }
}

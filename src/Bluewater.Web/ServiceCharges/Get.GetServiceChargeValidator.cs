using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.ServiceCharges;

public class GetServiceChargeValidator : Validator<GetServiceChargeRequest>
{
  public GetServiceChargeValidator()
  {
    RuleFor(x => x.ServiceChargeId).NotEmpty().WithMessage("Service charge ID is required.");
  }
}

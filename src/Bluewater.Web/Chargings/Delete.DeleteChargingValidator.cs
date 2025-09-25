using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Chargings;

public class DeleteChargingValidator : Validator<DeleteChargingRequest>
{
  public DeleteChargingValidator()
  {
    RuleFor(x => x.ChargingId)
      .NotEmpty()
      .WithMessage("ChargingId is required.");
  }
}

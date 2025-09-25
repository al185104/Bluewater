using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Chargings;

public class GetChargingValidator : Validator<GetChargingByIdRequest>
{
  public GetChargingValidator()
  {
    RuleFor(x => x.ChargingId)
      .NotEmpty()
      .WithMessage("ChargingId is required.");
  }
}

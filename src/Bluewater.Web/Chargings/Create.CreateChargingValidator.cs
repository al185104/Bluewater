using Bluewater.Infrastructure.Data.Config;
using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Chargings;

public class CreateChargingValidator : Validator<CreateChargingRequest>
{
  public CreateChargingValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty()
      .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
  }
}

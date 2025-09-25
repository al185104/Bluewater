using Bluewater.Infrastructure.Data.Config;
using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Chargings;

public class UpdateChargingValidator : Validator<UpdateChargingRequest>
{
  public UpdateChargingValidator()
  {
    RuleFor(x => x.Id)
      .NotEmpty();

    RuleFor(x => x.Name)
      .NotEmpty()
      .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
  }
}

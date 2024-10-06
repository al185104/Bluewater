using Bluewater.Infrastructure.Data.Config;
using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Divisions;

public class UpdateDivisionValidator : Validator<UpdateDivisionRequest>
{
  public UpdateDivisionValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty()
      .WithMessage("Name is required.")
      .MinimumLength(2)
      .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    RuleFor(x => x.DivisionId)
      .Must((args, divisionId) => args.Id == divisionId)
      .WithMessage("Route and body Ids must match; cannot update Id of an existing resource.");
  }
}

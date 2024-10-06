using Bluewater.Infrastructure.Data.Config;
using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Divisions;

public class CreateDivisionValidator : Validator<CreateDivisionRequest>
{
  public CreateDivisionValidator()
  {
    RuleFor(x => x.Name)
    .NotEmpty()
    .WithMessage("Name is required.")
    .MinimumLength(2)
    .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
  }
}

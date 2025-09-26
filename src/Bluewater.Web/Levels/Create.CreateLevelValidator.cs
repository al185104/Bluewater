using Bluewater.Infrastructure.Data.Config;
using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Levels;

public class CreateLevelValidator : Validator<CreateLevelRequest>
{
  public CreateLevelValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty()
      .WithMessage("Name is required.")
      .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

    RuleFor(x => x.Value)
      .NotEmpty()
      .WithMessage("Value is required.")
      .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
  }
}

using Bluewater.Infrastructure.Data.Config;
using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Levels;

public class UpdateLevelValidator : Validator<UpdateLevelRequest>
{
  public UpdateLevelValidator()
  {
    RuleFor(x => x.LevelId)
      .NotEmpty().WithMessage("Level ID is required and cannot be an empty GUID.");

    RuleFor(x => x.Id)
      .NotEmpty().WithMessage("Level ID is required.");

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

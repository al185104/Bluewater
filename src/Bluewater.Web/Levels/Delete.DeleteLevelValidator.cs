using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Levels;

public class DeleteLevelValidator : Validator<DeleteLevelRequest>
{
  public DeleteLevelValidator()
  {
    RuleFor(x => x.LevelId)
      .NotEmpty().WithMessage("Level ID is required and cannot be an empty GUID.");
  }
}

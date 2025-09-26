using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Levels;

public class GetLevelValidator : Validator<GetLevelByIdRequest>
{
  public GetLevelValidator()
  {
    RuleFor(x => x.LevelId)
      .NotEmpty().WithMessage("Level ID is required and cannot be an empty GUID.");
  }
}

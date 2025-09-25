using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Positions;

public class DeletePositionValidator : Validator<DeletePositionRequest>
{
  public DeletePositionValidator()
  {
    RuleFor(x => x.PositionId)
      .NotEmpty().WithMessage("Position ID is required and cannot be an empty GUID.");
  }
}

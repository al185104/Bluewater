using Bluewater.Infrastructure.Data.Config;
using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Positions;

public class UpdatePositionValidator : Validator<UpdatePositionRequest>
{
  public UpdatePositionValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty()
      .WithMessage("Name is required.")
      .MinimumLength(2)
      .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

    RuleFor(x => x.PositionId)
      .Must((args, positionId) => args.Id == positionId)
      .WithMessage("Route and body Ids must match; cannot update Id of an existing resource.");

    RuleFor(x => x.SectionId)
      .NotEmpty().WithMessage("Section ID is required and cannot be an empty GUID.");
  }
}

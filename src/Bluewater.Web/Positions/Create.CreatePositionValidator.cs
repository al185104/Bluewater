using Bluewater.Infrastructure.Data.Config;
using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Positions;

public class CreatePositionValidator : Validator<CreatePositionRequest>
{
  public CreatePositionValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty()
      .WithMessage("Name is required.")
      .MinimumLength(2)
      .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

    RuleFor(x => x.SectionId)
      .NotEmpty().WithMessage("Section ID is required and cannot be an empty GUID.");
  }
}

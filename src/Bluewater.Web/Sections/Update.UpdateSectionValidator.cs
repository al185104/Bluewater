using Bluewater.Infrastructure.Data.Config;
using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Sections;

public class UpdateSectionValidator : Validator<UpdateSectionRequest>
{
  public UpdateSectionValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty()
      .WithMessage("Name is required.")
      .MinimumLength(2)
      .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

    RuleFor(x => x.SectionId)
      .Must((args, sectionId) => args.Id == sectionId)
      .WithMessage("Route and body Ids must match; cannot update Id of an existing resource.");

    RuleFor(x => x.DepartmentId)
      .NotEmpty().WithMessage("Department ID is required and cannot be an empty GUID.");
  }
}

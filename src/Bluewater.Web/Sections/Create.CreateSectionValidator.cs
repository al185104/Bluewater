using Bluewater.Infrastructure.Data.Config;
using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Sections;

public class CreateSectionValidator : Validator<CreateSectionRequest>
{
  public CreateSectionValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty()
      .WithMessage("Name is required.")
      .MinimumLength(2)
      .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

    RuleFor(x => x.DepartmentId)
      .NotEmpty().WithMessage("Department ID is required and cannot be an empty GUID.");
  }
}

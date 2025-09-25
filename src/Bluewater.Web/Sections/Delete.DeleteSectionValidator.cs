using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Sections;

public class DeleteSectionValidator : Validator<DeleteSectionRequest>
{
  public DeleteSectionValidator()
  {
    RuleFor(x => x.SectionId)
      .NotEmpty().WithMessage("Section ID is required and cannot be an empty GUID.");
  }
}

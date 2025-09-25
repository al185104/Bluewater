using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Sections;

public class GetSectionValidator : Validator<GetSectionByIdRequest>
{
  public GetSectionValidator()
  {
    RuleFor(x => x.SectionId)
      .NotEmpty().WithMessage("Section ID is required and cannot be an empty GUID.");
  }
}

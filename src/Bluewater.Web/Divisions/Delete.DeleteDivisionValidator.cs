using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Divisions;

public class DeleteDivisionValidator : Validator<DeleteDivisionRequest>
{
  public DeleteDivisionValidator()
  {
    RuleFor(x => x.DivisionId)
      .NotEmpty().WithMessage("Division ID is required and cannot be an empty GUID.");
  }
}

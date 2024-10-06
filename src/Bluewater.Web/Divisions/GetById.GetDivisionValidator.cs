using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Divisions;

public class GetDivisionValidator : Validator<GetDivisionByIdRequest>
{
  public GetDivisionValidator()
  {
    RuleFor(x => x.DivisionId)
      .NotEmpty().WithMessage("Division ID is required and cannot be an empty GUID.");
  }
}

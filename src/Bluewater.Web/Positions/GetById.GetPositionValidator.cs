using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Positions;

public class GetPositionValidator : Validator<GetPositionByIdRequest>
{
  public GetPositionValidator()
  {
    RuleFor(x => x.PositionId)
      .NotEmpty().WithMessage("Position ID is required and cannot be an empty GUID.");
  }
}

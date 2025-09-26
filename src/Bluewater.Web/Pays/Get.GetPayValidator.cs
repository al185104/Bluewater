using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Pays;

public class GetPayValidator : Validator<GetPayRequest>
{
  public GetPayValidator()
  {
    RuleFor(x => x.PayId).NotEmpty().WithMessage("Pay ID is required.");
  }
}

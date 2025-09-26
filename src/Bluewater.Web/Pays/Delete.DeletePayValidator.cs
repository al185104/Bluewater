using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Pays;

public class DeletePayValidator : Validator<DeletePayRequest>
{
  public DeletePayValidator()
  {
    RuleFor(x => x.PayId).NotEmpty().WithMessage("Pay ID is required.");
  }
}

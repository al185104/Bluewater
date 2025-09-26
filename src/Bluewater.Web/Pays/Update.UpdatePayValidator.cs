using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Pays;

public class UpdatePayValidator : Validator<UpdatePayRequest>
{
  public UpdatePayValidator()
  {
    RuleFor(x => x.PayId).NotEmpty().WithMessage("Pay ID is required.");
    RuleFor(x => x.BasicPay).GreaterThanOrEqualTo(0);
    RuleFor(x => x.DailyRate).GreaterThanOrEqualTo(0);
    RuleFor(x => x.HourlyRate).GreaterThanOrEqualTo(0);
    RuleFor(x => x.HdmfCon).GreaterThanOrEqualTo(0);
    RuleFor(x => x.HdmfEr).GreaterThanOrEqualTo(0);
    RuleFor(x => x.Cola).GreaterThanOrEqualTo(0);
  }
}

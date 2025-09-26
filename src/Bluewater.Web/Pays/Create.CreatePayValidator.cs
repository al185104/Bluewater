using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Pays;

public class CreatePayValidator : Validator<CreatePayRequest>
{
  public CreatePayValidator()
  {
    RuleFor(x => x.BasicPay).GreaterThanOrEqualTo(0);
    RuleFor(x => x.DailyRate).GreaterThanOrEqualTo(0);
    RuleFor(x => x.HourlyRate).GreaterThanOrEqualTo(0);
    RuleFor(x => x.HdmfCon).GreaterThanOrEqualTo(0);
    RuleFor(x => x.HdmfEr).GreaterThanOrEqualTo(0);
  }
}

using Bluewater.Infrastructure.Data.Config;
using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.LeaveCredits;

public class CreateLeaveCreditValidator : Validator<CreateLeaveCreditRequest>
{
  public CreateLeaveCreditValidator()
  {
    RuleFor(x => x.Code)
      .NotEmpty()
      .WithMessage("Code is required.")
      .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

    RuleFor(x => x.Description)
      .NotEmpty()
      .WithMessage("Description is required.")
      .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

    RuleFor(x => x.Credit)
      .GreaterThanOrEqualTo(0)
      .WithMessage("Credit cannot be negative.");

    RuleFor(x => x.SortOrder)
      .GreaterThanOrEqualTo(0)
      .When(x => x.SortOrder.HasValue)
      .WithMessage("Sort order cannot be negative.");
  }
}

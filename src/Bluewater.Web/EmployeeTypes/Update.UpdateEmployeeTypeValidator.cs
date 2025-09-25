using Bluewater.Infrastructure.Data.Config;
using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.EmployeeTypes;

public class UpdateEmployeeTypeValidator : Validator<UpdateEmployeeTypeRequest>
{
  public UpdateEmployeeTypeValidator()
  {
    RuleFor(x => x.Id)
      .NotEmpty();

    RuleFor(x => x.Name)
      .NotEmpty()
      .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

    RuleFor(x => x.Value)
      .NotEmpty()
      .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
  }
}

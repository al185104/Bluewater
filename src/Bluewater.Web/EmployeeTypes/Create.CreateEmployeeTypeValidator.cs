using Bluewater.Infrastructure.Data.Config;
using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.EmployeeTypes;

public class CreateEmployeeTypeValidator : Validator<CreateEmployeeTypeRequest>
{
  public CreateEmployeeTypeValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty()
      .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

    RuleFor(x => x.Value)
      .NotEmpty()
      .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
  }
}

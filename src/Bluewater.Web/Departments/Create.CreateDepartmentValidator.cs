using Bluewater.Infrastructure.Data.Config;
using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Departments;

public class CreateDepartmentValidator : Validator<CreateDepartmentRequest>
{
  public CreateDepartmentValidator()
  {
    RuleFor(x => x.Name)
    .NotEmpty()
    .WithMessage("Name is required.")
    .MinimumLength(2)
    .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
  }
}

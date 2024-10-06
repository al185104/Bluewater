using Bluewater.Infrastructure.Data.Config;
using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Departments;

public class UpdateDepartmentValidator : Validator<UpdateDepartmentRequest>
{
  public UpdateDepartmentValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty()
      .WithMessage("Name is required.")
      .MinimumLength(2)
      .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    RuleFor(x => x.DepartmentId)
      .Must((args, DepartmentId) => args.Id == DepartmentId)
      .WithMessage("Route and body Ids must match; cannot update Id of an existing resource.");
  }
}

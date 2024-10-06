using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Departments;

public class DeleteDepartmentValidator : Validator<DeleteDepartmentRequest>
{
  public DeleteDepartmentValidator()
  {
    RuleFor(x => x.DepartmentId)
      .NotEmpty().WithMessage("Department ID is required and cannot be an empty GUID.");
  }
}

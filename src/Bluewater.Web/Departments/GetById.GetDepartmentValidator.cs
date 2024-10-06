using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Departments;

public class GetDepartmentValidator : Validator<GetDepartmentByIdRequest>
{
  public GetDepartmentValidator()
  {
    RuleFor(x => x.DepartmentId)
      .NotEmpty().WithMessage("Department ID is required and cannot be an empty GUID.");
  }
}

using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.EmployeeTypes;

public class GetEmployeeTypeValidator : Validator<GetEmployeeTypeByIdRequest>
{
  public GetEmployeeTypeValidator()
  {
    RuleFor(x => x.EmployeeTypeId)
      .NotEmpty()
      .WithMessage("EmployeeTypeId is required.");
  }
}

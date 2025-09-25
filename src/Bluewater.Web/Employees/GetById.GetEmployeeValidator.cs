using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Employees;

public class GetEmployeeValidator : Validator<GetEmployeeByIdRequest>
{
  public GetEmployeeValidator()
  {
    RuleFor(x => x.EmployeeId)
      .NotEmpty()
      .WithMessage("EmployeeId is required.");
  }
}

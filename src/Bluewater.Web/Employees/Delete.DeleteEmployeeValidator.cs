using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Employees;

public class DeleteEmployeeValidator : Validator<DeleteEmployeeRequest>
{
  public DeleteEmployeeValidator()
  {
    RuleFor(x => x.EmployeeId)
      .NotEmpty()
      .WithMessage("EmployeeId is required.");
  }
}

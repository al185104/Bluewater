using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.EmployeeTypes;

public class DeleteEmployeeTypeValidator : Validator<DeleteEmployeeTypeRequest>
{
  public DeleteEmployeeTypeValidator()
  {
    RuleFor(x => x.EmployeeTypeId)
      .NotEmpty()
      .WithMessage("EmployeeTypeId is required.");
  }
}

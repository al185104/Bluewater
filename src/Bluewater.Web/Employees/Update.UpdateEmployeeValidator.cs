using Bluewater.Core.EmployeeAggregate.Enum;
using Bluewater.Infrastructure.Data.Config;
using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Employees;

public class UpdateEmployeeValidator : Validator<UpdateEmployeeRequest>
{
  public UpdateEmployeeValidator()
  {
    RuleFor(x => x.Id)
      .NotEmpty();

    RuleFor(x => x.FirstName)
      .NotEmpty()
      .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

    RuleFor(x => x.LastName)
      .NotEmpty()
      .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

    RuleFor(x => x.Gender)
      .IsInEnum()
      .Must(g => g != Gender.NotSet)
      .WithMessage("Gender is required.");

    RuleFor(x => x.CivilStatus)
      .IsInEnum()
      .Must(s => s != CivilStatus.NotSet)
      .WithMessage("Civil status is required.");

    RuleFor(x => x.BloodType)
      .IsInEnum()
      .Must(b => b != BloodType.NotSet)
      .WithMessage("Blood type is required.");

    RuleFor(x => x.Status)
      .IsInEnum()
      .Must(s => s != Status.NotSet)
      .WithMessage("Status is required.");

    RuleFor(x => x.UserId)
      .NotEmpty();

    RuleFor(x => x.PositionId)
      .NotEmpty();

    RuleFor(x => x.PayId)
      .NotEmpty();

    RuleFor(x => x.TypeId)
      .NotEmpty();

    RuleFor(x => x.LevelId)
      .NotEmpty();

    RuleFor(x => x.ChargingId)
      .NotEmpty();
  }
}

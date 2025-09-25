using Ardalis.Result;
using Bluewater.UseCases.Employees.Create;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Employees;

/// <summary>
/// Create a new employee record.
/// </summary>
/// <param name="_mediator"></param>
public class Create(IMediator _mediator) : Endpoint<CreateEmployeeRequest, CreateEmployeeResponse>
{
  public override void Configure()
  {
    Post(CreateEmployeeRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(CreateEmployeeRequest req, CancellationToken ct)
  {
    var command = new CreateEmployeeCommand(
      req.FirstName!,
      req.LastName!,
      req.MiddleName,
      req.DateOfBirth,
      req.Gender,
      req.CivilStatus,
      req.BloodType,
      req.Status,
      req.Height,
      req.Weight,
      req.Image,
      req.Remarks,
      req.ContactInfo is null
        ? null
        : new CreateEmployeeCommand.ContactInfo(
            req.ContactInfo.Email,
            req.ContactInfo.TelNumber,
            req.ContactInfo.MobileNumber,
            req.ContactInfo.Address,
            req.ContactInfo.ProvincialAddress,
            req.ContactInfo.MothersMaidenName,
            req.ContactInfo.FathersName,
            req.ContactInfo.EmergencyContact,
            req.ContactInfo.RelationshipContact,
            req.ContactInfo.AddressContact,
            req.ContactInfo.TelNoContact,
            req.ContactInfo.MobileNoContact),
      req.EducationInfo is null
        ? null
        : new CreateEmployeeCommand.EducationInfo(
            req.EducationInfo.EducationalAttainment,
            req.EducationInfo.CourseGraduated,
            req.EducationInfo.UniversityGraduated),
      req.EmploymentInfo is null
        ? null
        : new CreateEmployeeCommand.EmploymentInfo(
            req.EmploymentInfo.DateHired,
            req.EmploymentInfo.DateRegularized,
            req.EmploymentInfo.DateResigned,
            req.EmploymentInfo.DateTerminated,
            req.EmploymentInfo.TinNo,
            req.EmploymentInfo.SssNo,
            req.EmploymentInfo.HdmfNo,
            req.EmploymentInfo.PhicNo,
            req.EmploymentInfo.BankAccount,
            req.EmploymentInfo.HasServiceCharge),
      req.UserId,
      req.PositionId,
      req.PayId,
      req.TypeId,
      req.LevelId,
      req.ChargingId,
      req.MealCredits,
      req.Tenant);

    Result<Guid> result = await _mediator.Send(command, ct);

    if (result.IsSuccess)
    {
      Response = new CreateEmployeeResponse(result.Value, req.FirstName!, req.LastName!, req.MiddleName, req.Tenant);
    }
  }
}

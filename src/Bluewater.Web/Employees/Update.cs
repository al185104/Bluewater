using Ardalis.Result;
using Bluewater.UseCases.Employees;
using Bluewater.UseCases.Employees.Update;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Employees;

/// <summary>
/// Update an existing employee.
/// </summary>
/// <param name="_mediator"></param>
public class Update(IMediator _mediator) : Endpoint<UpdateEmployeeRequest, UpdateEmployeeResponse>
{
  public override void Configure()
  {
    Put(UpdateEmployeeRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateEmployeeRequest req, CancellationToken ct)
  {
    var command = new UpdateEmployeeCommand(
      req.Id,
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
        : new UpdateEmployeeCommand.ContactInfo(
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
        : new UpdateEmployeeCommand.EducationInfo(
            req.EducationInfo.EducationalAttainment,
            req.EducationInfo.CourseGraduated,
            req.EducationInfo.UniversityGraduated),
      req.EmploymentInfo is null
        ? null
        : new UpdateEmployeeCommand.EmploymentInfo(
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

    Result<EmployeeDTO> result = await _mediator.Send(command, ct);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    if (result.IsSuccess)
    {
      Response = new UpdateEmployeeResponse(EmployeeMapper.ToRecord(result.Value));
    }
  }
}

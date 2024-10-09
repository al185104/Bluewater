using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.UseCases.Employees.Create;

namespace Bluewater.UseCases.Contributors.Create;
public class CreateEmployeeHandler(IRepository<Employee> _repository) : ICommandHandler<CreateEmployeeCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
  {
    var newEmployee = new Employee(
        request.FirstName,
        request.LastName,
        request.MiddleName,
        request.DateOfBirth,
        request.Gender,
        request.CivilStatus,
        request.BloodType,
        request.Status,
        request.Height,
        request.Weight,
        request.ImageUrl,
        request.Remarks
    );

    if (request.ContactInfo != null)
    {
      newEmployee.SetContactInfo(new Core.EmployeeAggregate.ContactInfo(
          request.ContactInfo.Email,
          request.ContactInfo.TelNumber,
          request.ContactInfo.MobileNumber,
          request.ContactInfo.Address,
          request.ContactInfo.ProvincialAddress,
          request.ContactInfo.MothersMaidenName,
          request.ContactInfo.FathersName,
          request.ContactInfo.EmergencyContact,
          request.ContactInfo.RelationshipContact,
          request.ContactInfo.addressContact,
          request.ContactInfo.TelNoContact,
          request.ContactInfo.MobileNoContact
      ));
    }

    if (request.EducationInfo != null)
    {
      newEmployee.SetEducationInfo(new Core.EmployeeAggregate.EducationInfo(
          request.EducationInfo.PrimarySchool,
          request.EducationInfo.SecondarySchool,
          request.EducationInfo.TertiarySchool,
          request.EducationInfo.VocationalSchool,
          request.EducationInfo.PrimaryDegree,
          request.EducationInfo.SecondaryDegree,
          request.EducationInfo.TertiaryDegree,
          request.EducationInfo.VocationalDegree
      ));
    }

    if (request.EmploymentInfo != null)
    {
      newEmployee.SetEmploymentInfo(new Core.EmployeeAggregate.EmploymentInfo(
        request.EmploymentInfo.DateHired,
        request.EmploymentInfo.DateRegularized,
        request.EmploymentInfo.DateResigned,
        request.EmploymentInfo.DateTerminated,
        request.EmploymentInfo.TinNo,
        request.EmploymentInfo.SssNo,
        request.EmploymentInfo.HdmfNo,
        request.EmploymentInfo.PhicNo,
        request.EmploymentInfo.BankAccount,
        request.EmploymentInfo.HasServiceCharge
      ));
    }

    newEmployee.SetExternalKeys(request.PositionId, request.PayId, request.TypeId, request.LevelId, request.ChargingId);

    var createdItem = await _repository.AddAsync(newEmployee, cancellationToken);
    return createdItem.Id;
  }
}

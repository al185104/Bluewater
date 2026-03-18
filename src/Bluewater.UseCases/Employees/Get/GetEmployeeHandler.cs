using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.EmployeeAggregate.Specifications;
using Bluewater.UseCases.Pays;
using Bluewater.UseCases.Users;

namespace Bluewater.UseCases.Employees.Get;

public class GetEmployeeHandler(IRepository<Employee> _repository) : IQueryHandler<GetEmployeeQuery, Result<EmployeeDTO>>
{
  public async Task<Result<EmployeeDTO>> Handle(GetEmployeeQuery request, CancellationToken cancellationToken)
  {
    var spec = new EmployeeByIdSpec(request.EmployeeId);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null) return Result.NotFound();

    var contact = new ContactInfoDTO(
        entity.ContactInfo?.Email,
        entity.ContactInfo?.TelNumber,
        entity.ContactInfo?.MobileNumber,
        entity.ContactInfo?.Address,
        entity.ContactInfo?.ProvincialAddress,
        entity.ContactInfo?.MothersMaidenName,
        entity.ContactInfo?.FathersName,
        entity.ContactInfo?.EmergencyContact,
        entity.ContactInfo?.RelationshipContact,
        entity.ContactInfo?.AddressContact,
        entity.ContactInfo?.TelNoContact,
        entity.ContactInfo?.MobileNoContact
    );

    var educationInfo = new EducationInfoDTO(
        entity.EducationInfo?.EducationalAttainment ?? Core.EmployeeAggregate.Enum.EducationalAttainment.NotSet,
        entity.EducationInfo?.CourseGraduated,
        entity.EducationInfo?.UniversityGraduated
    );

    var employeeInfo = new EmploymentInfoDTO(
        entity.EmploymentInfo?.DateHired,
        entity.EmploymentInfo?.DateRegularized,
        entity.EmploymentInfo?.DateResigned,
        entity.EmploymentInfo?.DateTerminated,
        entity.EmploymentInfo?.TINNo,
        entity.EmploymentInfo?.SSSNo,
        entity.EmploymentInfo?.HDMFNo,
        entity.EmploymentInfo?.PHICNo,
        entity.EmploymentInfo?.BankAccount,
        entity.EmploymentInfo?.HasServiceCharge ?? false
    );

    UserDTO? user = entity.User is null
        ? null
        : new UserDTO(
            entity.User.Id,
            entity.User.Username,
            entity.User.PasswordHash,
            entity.User.Credential,
            entity.User.SupervisedGroup,
            entity.User.IsGlobalSupervisor);

    PayDTO? pay = entity.Pay is null
        ? null
        : new PayDTO(
            entity.Pay.Id,
            entity.Pay.BasicPay,
            entity.Pay.DailyRate,
            entity.Pay.HourlyRate,
            entity.Pay.HDMF_Con,
            entity.Pay.HDMF_Er,
            entity.Pay.Cola);

    return new EmployeeDTO(
        entity.Id,
        entity.FirstName,
        entity.LastName,
        entity.MiddleName,
        entity.DateOfBirth,
        entity.Gender,
        entity.CivilStatus,
        entity.BloodType,
        entity.Status,
        entity.Height,
        entity.Weight,
        entity.ImageUrl,
        entity.Remarks,
        contact,
        educationInfo,
        employeeInfo,
        user,
        entity.Position?.Name,
        entity.Position?.Section?.Name,
        entity.Position?.Section?.Department?.Name,
        entity.Position?.Section?.Department?.Division?.Name,
        entity.Charging?.Name,
        pay,
        entity.Type?.Name,
        entity.Level?.Name
    );
  }
}

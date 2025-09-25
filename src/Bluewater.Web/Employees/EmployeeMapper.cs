using Bluewater.Core.EmployeeAggregate.Enum;
using Bluewater.UseCases.Employees;

namespace Bluewater.Web.Employees;

internal static class EmployeeMapper
{
  public static EmployeeRecord ToRecord(EmployeeDTO dto)
  {
    return new EmployeeRecord(
      dto.Id,
      dto.FirstName,
      dto.LastName,
      dto.MiddleName,
      dto.DateOfBirth,
      dto.Gender,
      dto.CivilStatus,
      dto.BloodType,
      dto.Status,
      dto.Height,
      dto.Weight,
      dto.ImageUrl,
      dto.Remarks,
      dto.ContactInfo is null
        ? null
        : new ContactInfoRecord(
            dto.ContactInfo.Email,
            dto.ContactInfo.TelNumber,
            dto.ContactInfo.MobileNumber,
            dto.ContactInfo.Address,
            dto.ContactInfo.ProvincialAddress,
            dto.ContactInfo.MothersMaidenName,
            dto.ContactInfo.FathersName,
            dto.ContactInfo.EmergencyContact,
            dto.ContactInfo.RelationshipContact,
            dto.ContactInfo.AddressContact,
            dto.ContactInfo.TelNoContact,
            dto.ContactInfo.MobileNoContact),
      dto.EducationInfo is null
        ? null
        : new EducationInfoRecord(
            dto.EducationInfo.EducationalAttainment,
            dto.EducationInfo.CourseGraduated,
            dto.EducationInfo.UniversityGraduated),
      dto.EmploymentInfo is null
        ? null
        : new EmploymentInfoRecord(
            dto.EmploymentInfo.DateHired,
            dto.EmploymentInfo.DateRegularized,
            dto.EmploymentInfo.DateResigned,
            dto.EmploymentInfo.DateTerminated,
            dto.EmploymentInfo.TinNo,
            dto.EmploymentInfo.SssNo,
            dto.EmploymentInfo.HdmfNo,
            dto.EmploymentInfo.PhicNo,
            dto.EmploymentInfo.BankAccount,
            dto.EmploymentInfo.HasServiceCharge),
      dto.User is null
        ? null
        : new UserRecord(
            dto.User.Id,
            dto.User.Username,
            dto.User.PasswordHash,
            dto.User.Credential,
            dto.User.SupervisedGroup,
            dto.User.IsGlobalSupervisor),
      dto.Position,
      dto.Section,
      dto.Department,
      dto.Division,
      dto.Charging,
      dto.Pay is null
        ? null
        : new PayRecord(
            dto.Pay.Id,
            dto.Pay.BasicPay,
            dto.Pay.DailyRate,
            dto.Pay.HourlyRate,
            dto.Pay.HDMF_Con,
            dto.Pay.HDMF_Er,
            dto.Pay.Cola),
      dto.Type,
      dto.Level,
      dto.MealCredits,
      dto.Tenant);
  }
}

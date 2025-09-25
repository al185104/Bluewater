using Bluewater.Core.EmployeeAggregate.Enum;
using Bluewater.Core.UserAggregate.Enum;

namespace Bluewater.Web.Employees;

public record EmployeeRecord(
  Guid Id,
  string FirstName,
  string LastName,
  string? MiddleName,
  DateTime? DateOfBirth,
  Gender Gender,
  CivilStatus CivilStatus,
  BloodType BloodType,
  Status Status,
  decimal? Height,
  decimal? Weight,
  byte[]? Image,
  string? Remarks,
  ContactInfoRecord? ContactInfo,
  EducationInfoRecord? EducationInfo,
  EmploymentInfoRecord? EmploymentInfo,
  UserRecord? User,
  string? Position,
  string? Section,
  string? Department,
  string? Division,
  string? Charging,
  PayRecord? Pay,
  string? Type,
  string? Level,
  int MealCredits,
  Tenant Tenant
);

public record ContactInfoRecord(
  string? Email,
  string? TelNumber,
  string? MobileNumber,
  string? Address,
  string? ProvincialAddress,
  string? MothersMaidenName,
  string? FathersName,
  string? EmergencyContact,
  string? RelationshipContact,
  string? AddressContact,
  string? TelNoContact,
  string? MobileNoContact
);

public record EducationInfoRecord(
  EducationalAttainment EducationalAttainment,
  string? CourseGraduated,
  string? UniversityGraduated
);

public record EmploymentInfoRecord(
  DateTime? DateHired,
  DateTime? DateRegularized,
  DateTime? DateResigned,
  DateTime? DateTerminated,
  string? TinNo,
  string? SssNo,
  string? HdmfNo,
  string? PhicNo,
  string? BankAccount,
  bool HasServiceCharge
);

public record UserRecord(
  Guid Id,
  string Username,
  string PasswordHash,
  Credential Credential,
  Guid? SupervisedGroup,
  bool IsGlobalSupervisor
);

public record PayRecord(
  Guid Id,
  decimal? BasicPay,
  decimal? DailyRate,
  decimal? HourlyRate,
  decimal? HdmfEmployeeContribution,
  decimal? HdmfEmployerContribution,
  decimal? Cola
);

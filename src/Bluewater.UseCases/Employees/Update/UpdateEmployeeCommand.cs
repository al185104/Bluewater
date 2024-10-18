using Ardalis.Result;
using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.UseCases.Employees.Update;

public record UpdateEmployeeCommand(
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
      byte[]? ImageUrl,
      string? Remarks,
      ContactInfo? ContactInfo,
      EducationInfo? EducationInfo,
      EmploymentInfo? EmploymentInfo,
      Guid UserId,
      Guid PositionId,
      Guid PayId,
      Guid TypeId,
      Guid LevelId,
      Guid ChargingId
  ) : Ardalis.SharedKernel.ICommand<Result<EmployeeDTO>>;

  public record ContactInfo(
      string? Email,
      string? TelNumber,
      string? MobileNumber,
      string? Address,
      string? ProvincialAddress,
      string? MothersMaidenName,
      string? FathersName,
      string? EmergencyContact,
      string? RelationshipContact,
      string? addressContact,
      string? TelNoContact,
      string? MobileNoContact
  );

  public record EducationInfo(
    EducationalAttainment EducationalAttainment,
    string? CourseGraduated,
    string? UniversityGraduated
  );

  public record EmploymentInfo(
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
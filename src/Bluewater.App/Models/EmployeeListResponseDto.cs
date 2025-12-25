using System;
using System.Collections.Generic;
using Bluewater.Core.EmployeeAggregate.Enum;
using Bluewater.Core.UserAggregate.Enum;

namespace Bluewater.App.Models;

public class EmployeeListResponseDto
{
  public List<EmployeeDto> Employees { get; set; } = new();
  public int TotalCount { get; set; }
}

public class EmployeeDto
{
  public Guid Id { get; set; }
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string? MiddleName { get; set; }
  public DateTime? DateOfBirth { get; set; }
  public Gender Gender { get; set; }
  public CivilStatus CivilStatus { get; set; }
  public BloodType BloodType { get; set; }
  public Status Status { get; set; }
  public decimal? Height { get; set; }
  public decimal? Weight { get; set; }
  public bool IsDeleted { get; set; }
  public string? Remarks { get; set; }
  public int MealCredits { get; set; }
  public Tenant Tenant { get; set; }
  public ContactInfoDto? ContactInfo { get; set; }
  public string? Position { get; set; }
  public string? Section { get; set; }
  public string? Department { get; set; }
  public string? Type { get; set; }
  public string? Level { get; set; }
  public string? Image { get; set; }
  public EducationInfoDto? EducationInfo { get; set; }
  public EmploymentInfoDto? EmploymentInfo { get; set; }
  public Guid? UserId { get; set; }
  public Guid? PositionId { get; set; }
  public Guid? PayId { get; set; }
  public Guid? TypeId { get; set; }
  public Guid? LevelId { get; set; }
  public Guid? ChargingId { get; set; }
  public DateTime CreatedDate { get; set; }
  public Guid CreateBy { get; set; }
  public DateTime UpdatedDate { get; set; }
  public Guid UpdateBy { get; set; }
  public UserDto? User { get; set; }
  public PayDto? Pay { get; set; }
}

public class ContactInfoDto
{
  public string? Email { get; set; }
  public string? TelNumber { get; set; }
  public string? MobileNumber { get; set; }
  public string? Address { get; set; }
  public string? ProvincialAddress { get; set; }
  public string? MothersMaidenName { get; set; }
  public string? FathersName { get; set; }
  public string? EmergencyContact { get; set; }
  public string? RelationshipContact { get; set; }
  public string? AddressContact { get; set; }
  public string? TelNoContact { get; set; }
  public string? MobileNoContact { get; set; }
}

public class EmploymentInfoDto
{
  public DateTime? DateHired { get; set; }
  public DateTime? DateRegularized { get; set; }
  public DateTime? DateResigned { get; set; }
  public DateTime? DateTerminated { get; set; }
  public string? TinNo { get; set; }
  public string? SssNo { get; set; }
  public string? HdmfNo { get; set; }
  public string? PhicNo { get; set; }
  public string? BankAccount { get; set; }
  public bool HasServiceCharge { get; set; }
}

public class EducationInfoDto
{
  public EducationalAttainment EducationalAttainment { get; set; }
  public string? CourseGraduated { get; set; }
  public string? UniversityGraduated { get; set; }
}

public class UserDto
{
  public string Username { get; set; } = string.Empty;
  public string PasswordHash { get; set; } = string.Empty;
  public Credential Credential { get; set; } = Credential.None;
  public Guid? SupervisedGroup { get; set; }
  public bool IsGlobalSupervisor { get; set; }
}

public class PayDto
{
  public decimal? BasicPay { get; set; }
  public decimal? DailyRate { get; set; }
  public decimal? HourlyRate { get; set; }
  public decimal? HdmfCon { get; set; }
  public decimal? HdmfEr { get; set; }
}

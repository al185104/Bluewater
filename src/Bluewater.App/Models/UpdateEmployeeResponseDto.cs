using System;
using Bluewater.Core.EmployeeAggregate.Enum;
using Bluewater.Core.UserAggregate.Enum;

namespace Bluewater.App.Models;

public class UpdateEmployeeResponseDto
{
  public EmployeeRecordDto? Employee { get; set; }
}

public class EmployeeRecordDto
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
  public byte[]? Image { get; set; }
  public string? Remarks { get; set; }
  public ContactInfoRecordDto? ContactInfo { get; set; }
  public EducationInfoRecordDto? EducationInfo { get; set; }
  public EmploymentInfoRecordDto? EmploymentInfo { get; set; }
  public UserRecordDto? User { get; set; }
  public string? Position { get; set; }
  public string? Section { get; set; }
  public string? Department { get; set; }
  public string? Division { get; set; }
  public string? Charging { get; set; }
  public PayRecordDto? Pay { get; set; }
  public string? Type { get; set; }
  public string? Level { get; set; }
  public int MealCredits { get; set; }
  public Tenant Tenant { get; set; }
}

public class ContactInfoRecordDto
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

public class EducationInfoRecordDto
{
  public EducationalAttainment EducationalAttainment { get; set; }
  public string? CourseGraduated { get; set; }
  public string? UniversityGraduated { get; set; }
}

public class EmploymentInfoRecordDto
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

public class UserRecordDto
{
  public Guid Id { get; set; }
  public string Username { get; set; } = string.Empty;
  public string PasswordHash { get; set; } = string.Empty;
  public Credential Credential { get; set; }
  public Guid? SupervisedGroup { get; set; }
  public bool IsGlobalSupervisor { get; set; }
}

public class PayRecordDto
{
  public Guid Id { get; set; }
  public decimal? BasicPay { get; set; }
  public decimal? DailyRate { get; set; }
  public decimal? HourlyRate { get; set; }
  public decimal? HdmfEmployeeContribution { get; set; }
  public decimal? HdmfEmployerContribution { get; set; }
  public decimal? Cola { get; set; }
}

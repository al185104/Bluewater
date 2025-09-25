using System.ComponentModel.DataAnnotations;
using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.Web.Employees;

public class CreateEmployeeRequest
{
  public const string Route = "/Employees";

  [Required]
  public string? FirstName { get; set; }

  [Required]
  public string? LastName { get; set; }

  public string? MiddleName { get; set; }
  public DateTime? DateOfBirth { get; set; }

  [Required]
  public Gender Gender { get; set; }

  [Required]
  public CivilStatus CivilStatus { get; set; }

  [Required]
  public BloodType BloodType { get; set; }

  [Required]
  public Status Status { get; set; }

  public decimal? Height { get; set; }
  public decimal? Weight { get; set; }
  public byte[]? Image { get; set; }
  public string? Remarks { get; set; }

  public ContactInfoRequest? ContactInfo { get; set; }
  public EducationInfoRequest? EducationInfo { get; set; }
  public EmploymentInfoRequest? EmploymentInfo { get; set; }

  public Guid? UserId { get; set; }
  public Guid? PositionId { get; set; }
  public Guid? PayId { get; set; }
  public Guid? TypeId { get; set; }
  public Guid? LevelId { get; set; }
  public Guid? ChargingId { get; set; }
  public int MealCredits { get; set; }
  public Tenant Tenant { get; set; } = Tenant.Maribago;
}

public class ContactInfoRequest
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

public class EducationInfoRequest
{
  public EducationalAttainment EducationalAttainment { get; set; } = EducationalAttainment.NotSet;
  public string? CourseGraduated { get; set; }
  public string? UniversityGraduated { get; set; }
}

public class EmploymentInfoRequest
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

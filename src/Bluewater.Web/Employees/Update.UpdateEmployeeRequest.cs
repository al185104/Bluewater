using System.ComponentModel.DataAnnotations;
using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.Web.Employees;

public class UpdateEmployeeRequest
{
  public const string Route = "/Employees";

  [Required]
  public Guid Id { get; set; }

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

  [Required]
  public Guid UserId { get; set; }

  [Required]
  public Guid PositionId { get; set; }

  [Required]
  public Guid PayId { get; set; }

  [Required]
  public Guid TypeId { get; set; }

  [Required]
  public Guid LevelId { get; set; }

  [Required]
  public Guid ChargingId { get; set; }

  public int MealCredits { get; set; }
  public Tenant Tenant { get; set; } = Tenant.Maribago;
}

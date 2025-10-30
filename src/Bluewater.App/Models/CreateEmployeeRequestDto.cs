using System;
using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.App.Models;

public class CreateEmployeeRequestDto
{
  public const string Route = "Employees";

  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string? MiddleName { get; set; }
  public DateTime? DateOfBirth { get; set; }
  public Gender Gender { get; set; } = Gender.NotSet;
  public CivilStatus CivilStatus { get; set; } = CivilStatus.NotSet;
  public BloodType BloodType { get; set; } = BloodType.NotSet;
  public Status Status { get; set; } = Status.NotSet;
  public int MealCredits { get; set; }
  public Tenant Tenant { get; set; } = Tenant.Maribago;
  public string? Remarks { get; set; }
  public decimal? Height { get; set; }
  public decimal? Weight { get; set; }
  public CreateEmployeeContactInfoDto? ContactInfo { get; set; }
}

public class CreateEmployeeContactInfoDto
{
  public string? Email { get; set; }
  public string? MobileNumber { get; set; }
  public string? TelNumber { get; set; }
  public string? Address { get; set; }
}

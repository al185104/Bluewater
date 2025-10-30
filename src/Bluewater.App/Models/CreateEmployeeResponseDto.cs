using System;
using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.App.Models;

public class CreateEmployeeResponseDto
{
  public Guid Id { get; set; }
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string? MiddleName { get; set; }
  public Tenant Tenant { get; set; }
}

using System;
using System.Collections.Generic;

namespace Bluewater.App.Models;

public class EmployeeListResponseDto
{
  public List<EmployeeDto> Employees { get; set; } = new();
}

public class EmployeeDto
{
  public Guid Id { get; set; }
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string? MiddleName { get; set; }
  public ContactInfoDto? ContactInfo { get; set; }
  public string? Position { get; set; }
  public string? Section { get; set; }
  public string? Department { get; set; }
  public string? Type { get; set; }
  public string? Level { get; set; }
  public string? Image { get; set; }
}

public class ContactInfoDto
{
  public string? Email { get; set; }
  public string? TelNumber { get; set; }
  public string? MobileNumber { get; set; }
}

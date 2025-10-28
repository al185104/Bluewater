using System;

namespace Bluewater.App.Models;

public class EmployeeTypeSummary
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;
  public bool IsActive { get; set; }
}

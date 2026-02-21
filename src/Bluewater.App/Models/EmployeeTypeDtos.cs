using System;
using System.Collections.Generic;

namespace Bluewater.App.Models;

public class EmployeeTypeListResponseDto
{
  public List<EmployeeTypeDto?> EmployeeTypes { get; set; } = new();
}

public class EmployeeTypeDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;
  public bool IsActive { get; set; }
}

public class CreateEmployeeTypeRequestDto
{
  public string Name { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;
  public bool IsActive { get; set; } = true;
}

using System;
using System.Collections.Generic;

namespace Bluewater.App.Models;

public class DepartmentListResponseDto
{
  public List<DepartmentDto?> Departments { get; set; } = new();
}

public class DepartmentDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public Guid DivisionId { get; set; }
}

public class CreateDepartmentRequestDto
{
  public string? Name { get; set; }
  public string? Description { get; set; }
  public Guid DivisionId { get; set; }
}

public class UpdateDepartmentRequestDto
{
  public Guid DepartmentId { get; set; }
  public Guid Id { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }
  public Guid DivisionId { get; set; }

  public static string BuildRoute(Guid departmentId) => $"Departments/{departmentId}";
}

public class UpdateDepartmentResponseDto
{
  public DepartmentDto? Department { get; set; }
}

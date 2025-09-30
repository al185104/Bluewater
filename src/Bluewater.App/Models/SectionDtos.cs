using System;
using System.Collections.Generic;

namespace Bluewater.App.Models;

public class SectionListResponseDto
{
  public List<SectionDto?> Sections { get; set; } = new();
}

public class SectionDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public string? Approved1Id { get; set; }
  public string? Approved2Id { get; set; }
  public string? Approved3Id { get; set; }
  public Guid DepartmentId { get; set; }
}

public class CreateSectionRequestDto
{
  public string? Name { get; set; }
  public string? Description { get; set; }
  public string? Approved1Id { get; set; }
  public string? Approved2Id { get; set; }
  public string? Approved3Id { get; set; }
  public Guid DepartmentId { get; set; }
}

public class UpdateSectionRequestDto
{
  public Guid SectionId { get; set; }
  public Guid Id { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }
  public string? Approved1Id { get; set; }
  public string? Approved2Id { get; set; }
  public string? Approved3Id { get; set; }
  public Guid DepartmentId { get; set; }

  public static string BuildRoute(Guid sectionId) => $"Sections/{sectionId}";
}

public class UpdateSectionResponseDto
{
  public SectionDto? Section { get; set; }
}

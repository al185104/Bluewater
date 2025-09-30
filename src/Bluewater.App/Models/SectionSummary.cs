using System;

namespace Bluewater.App.Models;

public class SectionSummary
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public string? Approved1Id { get; set; }
  public string? Approved2Id { get; set; }
  public string? Approved3Id { get; set; }
  public Guid DepartmentId { get; set; }
}

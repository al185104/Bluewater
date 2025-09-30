using System;

namespace Bluewater.App.Models;

public class DepartmentSummary
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public Guid DivisionId { get; set; }
}

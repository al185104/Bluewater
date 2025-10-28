using System;

namespace Bluewater.App.Models;

public class HolidaySummary
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public DateTime Date { get; set; }
  public bool IsRegular { get; set; }
}

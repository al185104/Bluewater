using System;

namespace Bluewater.App.Models;

public class ShiftSummary
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? ShiftStartTime { get; set; }
  public string? ShiftBreakTime { get; set; }
  public string? ShiftBreakEndTime { get; set; }
  public string? ShiftEndTime { get; set; }
  public decimal BreakHours { get; set; }

  public string BreakHoursDisplay => BreakHours.ToString("0.##");
}

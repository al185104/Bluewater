using System;
using System.Collections.Generic;

namespace Bluewater.App.Models;

public class ShiftListResponseDto
{
  public List<ShiftDto> Shifts { get; set; } = new();
}

public class ShiftDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public TimeOnly? ShiftStartTime { get; set; }
  public TimeOnly? ShiftBreakTime { get; set; }
  public TimeOnly? ShiftBreakEndTime { get; set; }
  public TimeOnly? ShiftEndTime { get; set; }
  public decimal BreakHours { get; set; }
}

public class CreateShiftRequestDto
{
  public string? Name { get; set; }
  public TimeOnly? ShiftStartTime { get; set; }
  public TimeOnly? ShiftBreakTime { get; set; }
  public TimeOnly? ShiftBreakEndTime { get; set; }
  public TimeOnly? ShiftEndTime { get; set; }
  public decimal BreakHours { get; set; }
}

public class UpdateShiftRequestDto
{
  public Guid ShiftId { get; set; }
  public Guid Id { get; set; }
  public string? Name { get; set; }
  public TimeOnly? ShiftStartTime { get; set; }
  public TimeOnly? ShiftBreakTime { get; set; }
  public TimeOnly? ShiftBreakEndTime { get; set; }
  public TimeOnly? ShiftEndTime { get; set; }
  public decimal BreakHours { get; set; }

  public static string BuildRoute(Guid shiftId) => $"Shifts/{shiftId}";
}

public class UpdateShiftResponseDto
{
  public ShiftDto? Shift { get; set; }
}

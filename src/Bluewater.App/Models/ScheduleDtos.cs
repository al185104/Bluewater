using System;
using System.Collections.Generic;

namespace Bluewater.App.Models;

public class ScheduleListResponseDto
{
  public List<EmployeeScheduleDto?> Employees { get; set; } = new();
  public int TotalCount { get; set; }
}

public class ScheduleDto
{
  public Guid Id { get; set; }
  public Guid EmployeeId { get; set; }
  public string Name { get; set; } = string.Empty;
  public Guid ShiftId { get; set; }
  public DateOnly ScheduleDate { get; set; }
  public bool IsDefault { get; set; }
  public ScheduleShiftDetailsDto? Shift { get; set; }
}

public class ScheduleShiftDetailsDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public TimeOnly? ShiftStartTime { get; set; }
  public TimeOnly? ShiftBreakTime { get; set; }
  public TimeOnly? ShiftBreakEndTime { get; set; }
  public TimeOnly? ShiftEndTime { get; set; }
  public decimal BreakHours { get; set; }
}

public class EmployeeScheduleDto
{
  public Guid EmployeeId { get; set; }
  public string Barcode { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Section { get; set; } = string.Empty;
  public string Charging { get; set; } = string.Empty;
  public List<ScheduleShiftInfoDto?> Shifts { get; set; } = new();
}

public class ScheduleShiftInfoDto
{
  public Guid ScheduleId { get; set; }
  public ScheduleShiftDetailsDto? Shift { get; set; }
  public DateOnly ScheduleDate { get; set; }
  public bool IsDefault { get; set; }
  public bool IsUpdated { get; set; }
}

public class CreateScheduleRequestDto
{
  public const string Route = "Schedules";

  public Guid EmployeeId { get; set; }
  public Guid ShiftId { get; set; }
  public DateOnly ScheduleDate { get; set; }
  public bool IsDefault { get; set; }
}

public class CreateScheduleResponseDto
{
  public Guid ScheduleId { get; set; }
}

public class UpdateScheduleRequestDto
{
  public Guid ScheduleId { get; set; }
  public Guid EmployeeId { get; set; }
  public Guid ShiftId { get; set; }
  public DateOnly ScheduleDate { get; set; }
  public bool IsDefault { get; set; }

  public static string BuildRoute(Guid scheduleId) => $"Schedules/{scheduleId}";
}

public class UpdateScheduleResponseDto
{
  public ScheduleDto? Schedule { get; set; }
}

public static class ScheduleRequestRoutes
{
  public static string BuildGetRoute(Guid scheduleId) => $"Schedules/{scheduleId}";
  public static string BuildDeleteRoute(Guid scheduleId) => $"Schedules/{scheduleId}";
}

public class ScheduleSummary
{
  public Guid Id { get; set; }
  public Guid EmployeeId { get; set; }
  public string Name { get; set; } = string.Empty;
  public Guid ShiftId { get; set; }
  public DateOnly ScheduleDate { get; set; }
  public bool IsDefault { get; set; }
  public ScheduleShiftDetailsSummary? Shift { get; set; }
}

public class ScheduleShiftDetailsSummary
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public TimeOnly? ShiftStartTime { get; set; }
  public TimeOnly? ShiftBreakTime { get; set; }
  public TimeOnly? ShiftBreakEndTime { get; set; }
  public TimeOnly? ShiftEndTime { get; set; }
  public decimal BreakHours { get; set; }
}

public class EmployeeScheduleSummary
{
  public Guid EmployeeId { get; set; }
  public string Barcode { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Section { get; set; } = string.Empty;
  public string Charging { get; set; } = string.Empty;
  public List<ScheduleShiftInfoSummary> Shifts { get; set; } = new();
}

public class ScheduleShiftInfoSummary
{
  public Guid ScheduleId { get; set; }
  public ScheduleShiftDetailsSummary? Shift { get; set; }
  public DateOnly ScheduleDate { get; set; }
  public bool IsDefault { get; set; }
  public bool IsUpdated { get; set; }
}

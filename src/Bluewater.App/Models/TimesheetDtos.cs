using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Bluewater.App.Models;

public class TimesheetListResponseDto
{
  public EmployeeTimesheetDto? Employee { get; set; }
}

public class TimesheetListAllResponseDto
{
  public List<AllEmployeeTimesheetDto?> Employees { get; set; } = new();
}

public class EmployeeTimesheetDto
{
  public Guid EmployeeId { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Department { get; set; } = string.Empty;
  public string Section { get; set; } = string.Empty;
  public string Charging { get; set; } = string.Empty;
  public List<TimesheetInfoDto?> Timesheets { get; set; } = new();
}

public class AllEmployeeTimesheetDto : EmployeeTimesheetDto
{
  public decimal TotalWorkHours { get; set; }
  public decimal TotalBreak { get; set; }
  public decimal TotalLates { get; set; }
  public int TotalAbsents { get; set; }
}

public class TimesheetInfoDto
{
  public Guid TimesheetId { get; set; }
  public DateTime? TimeIn1 { get; set; }
  public DateTime? TimeOut1 { get; set; }
  public DateTime? TimeIn2 { get; set; }
  public DateTime? TimeOut2 { get; set; }
  public DateOnly? EntryDate { get; set; }
  public bool IsEdited { get; set; }
}

public class UpdateTimesheetRequestDto
{
  public Guid Id { get; set; }
  public Guid EmployeeId { get; set; }
  public DateTime? TimeIn1 { get; set; }
  public DateTime? TimeOut1 { get; set; }
  public DateTime? TimeIn2 { get; set; }
  public DateTime? TimeOut2 { get; set; }
  public DateOnly? EntryDate { get; set; }
  public bool IsLocked { get; set; }
}

public class UpdateTimesheetResponseDto
{
  public TimesheetDto? Timesheet { get; set; }
}

public class TimesheetDto
{
  public Guid Id { get; set; }
  public Guid EmployeeId { get; set; }
  public DateTime? TimeIn1 { get; set; }
  public DateTime? TimeOut1 { get; set; }
  public DateTime? TimeIn2 { get; set; }
  public DateTime? TimeOut2 { get; set; }
  public DateOnly? EntryDate { get; set; }
  public bool IsEdited { get; set; }
}

public enum TimesheetInputType
{
  TimeIn1 = 0,
  TimeOut1 = 1,
  TimeIn2 = 2,
  TimeOut2 = 3
}

public class CreateTimesheetRequestDto
{
  public const string Route = "Timesheets";

  [JsonPropertyName("username")]
  public string Username { get; set; } = string.Empty;

  [JsonPropertyName("timeInput")]
  public DateTime? TimeInput { get; set; }

  [JsonPropertyName("entryDate")]
  public DateOnly? EntryDate { get; set; }

  [JsonPropertyName("inputType")]
  public int InputType { get; set; }
}

public class CreateTimesheetResponseDto
{
  [JsonPropertyName("name")]
  public string? Name { get; set; }
}

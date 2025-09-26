using System.Linq;
using Bluewater.UseCases.Timesheets;

namespace Bluewater.Web.Timesheets;

public record TimesheetRecord(
  Guid Id,
  Guid EmployeeId,
  DateTime? TimeIn1,
  DateTime? TimeOut1,
  DateTime? TimeIn2,
  DateTime? TimeOut2,
  DateOnly? EntryDate,
  bool IsEdited);

public record TimesheetInfoRecord(
  Guid TimesheetId,
  DateTime? TimeIn1,
  DateTime? TimeOut1,
  DateTime? TimeIn2,
  DateTime? TimeOut2,
  DateOnly? EntryDate,
  bool IsEdited);

public record EmployeeTimesheetRecord(
  Guid EmployeeId,
  string Name,
  string Department,
  string Section,
  string Charging,
  List<TimesheetInfoRecord> Timesheets);

public record AllEmployeeTimesheetRecord(
  Guid EmployeeId,
  string Name,
  string Department,
  string Section,
  string Charging,
  List<TimesheetInfoRecord> Timesheets,
  decimal TotalWorkHours,
  decimal TotalBreak,
  decimal TotalLates,
  int TotalAbsents)
  : EmployeeTimesheetRecord(EmployeeId, Name, Department, Section, Charging, Timesheets);

public static class TimesheetMapper
{
  public static TimesheetRecord ToRecord(TimesheetDTO dto) =>
    new(dto.Id, dto.EmployeeId, dto.TimeIn1, dto.TimeOut1, dto.TimeIn2, dto.TimeOut2, dto.EntryDate, dto.IsEdited);

  public static TimesheetInfoRecord ToRecord(TimesheetInfo dto) =>
    new(dto.TimesheetId, dto.TimeIn1, dto.TimeOut1, dto.TimeIn2, dto.TimeOut2, dto.EntryDate, dto.IsEdited);

  public static EmployeeTimesheetRecord ToRecord(EmployeeTimesheetDTO dto) =>
    new(
      dto.EmployeeId,
      dto.Name,
      dto.Department,
      dto.Section,
      dto.Charging,
      dto.Timesheets.Select(ToRecord).ToList());

  public static AllEmployeeTimesheetRecord ToRecord(AllEmployeeTimesheetDTO dto) =>
    new(
      dto.EmployeeId,
      dto.Name,
      dto.Department,
      dto.Section,
      dto.Charging,
      dto.Timesheets.Select(ToRecord).ToList(),
      dto.TotalWorkHours,
      dto.TotalBreak,
      dto.TotalLates,
      dto.TotalAbsents);
}

namespace Bluewater.UseCases.Timesheets;
public record EmployeeTimesheetDTO(Guid EmployeeId, string Name, string Department, string Section, string Charging, List<TimesheetInfo> Timesheets);
public record TimesheetInfo(Guid TimesheetId, DateTime? TimeIn1, DateTime? TimeOut1, DateTime? TimeIn2, DateTime? TimeOut2, DateOnly? EntryDate, bool IsEdited = false);
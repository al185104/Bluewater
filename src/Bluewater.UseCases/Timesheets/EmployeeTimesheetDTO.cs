namespace Bluewater.UseCases.Timesheets;
public record EmployeeTimesheetDTO(Guid EmployeeId, string Name, string Department, string Section, string Charging, List<TimesheetInfo> Timesheets);
public record TimesheetInfo() {
    public Guid TimesheetId { get; set; }
    public DateTime? TimeIn1 { get; set; }
    public DateTime? TimeOut1 { get; set; }
    public DateTime? TimeIn2 { get; set; }
    public DateTime? TimeOut2 { get; set; }
    public DateOnly? EntryDate { get; set; }
    public bool IsEdited { get; set; } = false;

    public TimesheetInfo(Guid timesheetId, DateTime? timeIn1, DateTime? timeOut1, DateTime? timeIn2, DateTime? timeOut2, DateOnly? entryDate, bool isEdited = false) : this()
    {
        TimesheetId = timesheetId;
        TimeIn1 = timeIn1;
        TimeOut1 = timeOut1;
        TimeIn2 = timeIn2;
        TimeOut2 = timeOut2;
        EntryDate = entryDate;
        IsEdited = isEdited;
    }
    
}
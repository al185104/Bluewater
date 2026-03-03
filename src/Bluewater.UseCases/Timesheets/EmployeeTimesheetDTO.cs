namespace Bluewater.UseCases.Timesheets;
public record EmployeeTimesheetDTO(Guid EmployeeId, string Name, string Department, string Section, string Charging, List<TimesheetInfo> Timesheets);
public record TimesheetInfo() {
    public Guid TimesheetId { get; set; }
    public Guid? ScheduleId { get; set; }
    public Guid? ShiftId { get; set; }
    public string? ShiftName { get; set; }
    public DateTime? TimeIn1 { get; set; }
    public DateTime? TimeOut1 { get; set; }
    public DateTime? TimeIn2 { get; set; }
    public DateTime? TimeOut2 { get; set; }
    public DateOnly? EntryDate { get; set; }
    public bool IsEdited { get; set; } = false;

    public TimesheetInfo(Guid timesheetId, Guid? scheduleId, Guid? shiftId, string? shiftName, DateTime? timeIn1, DateTime? timeOut1, DateTime? timeIn2, DateTime? timeOut2, DateOnly? entryDate, bool isEdited = false) : this()
    {
        TimesheetId = timesheetId;
        ScheduleId = scheduleId;
        ShiftId = shiftId;
        ShiftName = shiftName;
        TimeIn1 = timeIn1;
        TimeOut1 = timeOut1;
        TimeIn2 = timeIn2;
        TimeOut2 = timeOut2;
        EntryDate = entryDate;
        IsEdited = isEdited;
    }
    
}

namespace Bluewater.UseCases.Timesheets;
public record TimesheetDTO(){
    public Guid Id { get; init; }
    public Guid EmployeeId { get; set; }
    public DateTime? TimeIn1 { get; set; }
    public DateTime? TimeOut1 { get; set; }
    public DateTime? TimeIn2 { get; set; }
    public DateTime? TimeOut2 { get; set; }
    public DateOnly? EntryDate { get; set; }
    public bool IsEdited { get; set; } = false;

    public TimesheetDTO(Guid id, Guid employeeId, DateTime? timeIn1, DateTime? timeOut1, DateTime? timeIn2, DateTime? timeOut2, DateOnly? entryDate, bool isEdited = false) : this()
    {
        Id = id;
        EmployeeId = employeeId;
        TimeIn1 = timeIn1;
        TimeOut1 = timeOut1;
        TimeIn2 = timeIn2;
        TimeOut2 = timeOut2;
        EntryDate = entryDate;
        IsEdited = isEdited;
    }
}
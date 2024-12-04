using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;

namespace Bluewater.Core.TimesheetAggregate;
public class Timesheet(Guid employeeId, DateTime? timeIn1, DateTime? timeOut1, DateTime? timeIn2, DateTime? timeOut2, DateOnly? entryDate) : EntityBase<Guid>, IAggregateRoot
{
    // foreign keys
    public Guid EmployeeId { get; private set; } = employeeId;
    public DateTime? TimeIn1 { get; set; } = timeIn1;
    public DateTime? TimeOut1 { get; set; } = timeOut1;
    public DateTime? TimeIn2 { get; set; } = timeIn2;
    public DateTime? TimeOut2 { get; set; } = timeOut2;
    public DateTime? TimeIn1Orig { get; set; } = timeIn1;
    public DateTime? TimeOut1Orig { get; set; } = timeOut1;
    public DateTime? TimeIn2Orig { get; set; } = timeIn2;
    public DateTime? TimeOut2Orig { get; set; } = timeOut2;
    public bool IsEdited { get; set; } = false;
    public bool IsLocked { get; set; } = false;
    public DateOnly? EntryDate { get; set; } = entryDate;

    public DateTime CreatedDate { get; private set; } = DateTime.Now;
    public Guid CreateBy { get; private set; } = Guid.Empty;
    public DateTime UpdatedDate { get; private set; } = DateTime.Now;
    public Guid UpdateBy { get; private set; } = Guid.Empty;

    // virtual properties
    public virtual Employee Employee { get; set; } = null!;

    public Timesheet() : this(Guid.Empty, null, null, null, null, null) { }


    public void UpdateTimesheet(Guid employeeId, DateTime? timeIn1, DateTime? timeOut1, DateTime? timeIn2, DateTime? timeOut2, DateOnly? entryDate = null, bool isLocked = false)
    {
        EmployeeId = employeeId;
        TimeIn1 = timeIn1;
        TimeOut1 = timeOut1;
        TimeIn2 = timeIn2;
        TimeOut2 = timeOut2;
        EntryDate = entryDate ?? null;
        IsLocked = isLocked;

        IsEdited = true;
        UpdatedDate = DateTime.Now;
    }

    public void UpdateTimesheetByInput(DateTime? timein, int input)
    {
        switch(input){
            case 0:
                TimeIn1 = timein;
                TimeIn1Orig = timein;
                break;
            case 1:
                TimeOut1 = timein;
                TimeOut1Orig = timein;
                break;
            case 2:
                TimeIn2 = timein;
                TimeIn2Orig = timein;
                break;
            case 3:
                TimeOut2 = timein;
                TimeOut2Orig = timein;
                break;
        }
        UpdatedDate = DateTime.Now;
    }
}

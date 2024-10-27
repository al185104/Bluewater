using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.ShiftAggregate;
using Bluewater.Core.TimesheetAggregate;

namespace Bluewater.Core.AttendanceAggregate;
public class Attendance(Guid employeeId, Guid? shiftId, Guid? timesheetId, Guid? leaveId, DateOnly? entryDate, decimal? workHrs, decimal? lateHrs, decimal? underHrs, bool isLocked = false) : EntityBase<Guid>, IAggregateRoot
{
    public Guid EmployeeId { get; private set; } = employeeId;
    public Guid? ShiftId { get; private set; } = shiftId;
    public Guid? TimesheetId { get; private set; } = timesheetId;
    public Guid? LeaveId { get; private set; } = leaveId;
    public DateOnly? EntryDate { get; private set; } = entryDate;
    public decimal? WorkHrs { get; private set; } = workHrs;
    public decimal? LateHrs { get; private set; } = lateHrs;
    public decimal? UnderHrs { get; private set; } = underHrs;
    public bool IsLocked { get; private set; } = isLocked;
    public bool IsEdited { get; private set; } = false;
    public string? Remarks { get; private set; } = string.Empty;

    public DateTime CreatedDate { get; private set; } = DateTime.Now;
    public Guid CreateBy { get; private set; } = Guid.Empty;
    public DateTime UpdatedDate { get; private set; } = DateTime.Now;
    public Guid UpdateBy { get; private set; } = Guid.Empty;    

    // virtual properties
    public virtual Employee Employee { get; set; } = null!;
    public virtual Shift Shift { get; set; } = null!;
    public virtual Timesheet Timesheet { get; set; } = null!;

    public void Update(Guid? shiftId, Guid? timesheetId, Guid? leaveId, decimal? workHrs, decimal? lateHrs, decimal? underHrs, string? remarks)
    {
        ShiftId = shiftId;
        TimesheetId = timesheetId;
        LeaveId = leaveId;
        WorkHrs = workHrs;
        LateHrs = lateHrs;
        UnderHrs = underHrs;
        Remarks = remarks;
        IsEdited = true;

        UpdatedDate = DateTime.Now;
    }

}

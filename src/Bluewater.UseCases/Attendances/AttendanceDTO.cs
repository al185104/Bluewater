using Bluewater.UseCases.Shifts;
using Bluewater.UseCases.Timesheets;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Attendances;
public record AttendanceDTO()
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? ShiftId { get; set; }
    public Guid? TimesheetId { get; set; }
    public Guid? LeaveId { get; set; }
    public DateOnly? EntryDate { get; set; }
    public ApplicationStatusDTO? LeaveStatus { get; set; }
    public decimal? WorkHrs { get; set; }
    public decimal? LateHrs { get; set; }
    public decimal? UnderHrs { get; set; }
    public decimal? OverbreakHrs { get; set; }
    public decimal? NightShiftHours { get; set; }
    public bool IsLocked { get; set; }
    public ShiftDTO? Shift { get; set; } = null;
    public TimesheetDTO? Timesheet { get; set; } = null;

    public AttendanceDTO(Guid id, Guid employeeId, Guid? shiftId, Guid? timesheetId, Guid? leaveId, DateOnly? entryDate, decimal? workHrs, decimal? lateHrs, decimal? underHrs, decimal? overbreakHrs, decimal? nightShiftHrs, ApplicationStatusDTO? leaveStatus = null, bool isLocked = false, ShiftDTO? shift = null, TimesheetDTO? timesheet = null) : this()
    {
        Id = id;
        EmployeeId = employeeId;
        ShiftId = shiftId;
        TimesheetId = timesheetId;
        LeaveId = leaveId;
        EntryDate = entryDate;
        LeaveStatus = leaveStatus;
        WorkHrs = workHrs;
        LateHrs = lateHrs;
        UnderHrs = underHrs;
        OverbreakHrs = overbreakHrs;
        NightShiftHours = nightShiftHrs;
        IsLocked = isLocked;
        Shift = shift;
        Timesheet = timesheet;
        
    }
}


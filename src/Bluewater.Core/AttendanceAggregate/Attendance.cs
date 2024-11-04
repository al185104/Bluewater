using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.ShiftAggregate;
using Bluewater.Core.TimesheetAggregate;

namespace Bluewater.Core.AttendanceAggregate;
public class Attendance(Guid employeeId, Guid? shiftId, Guid? timesheetId, Guid? leaveId, DateOnly? entryDate, decimal? workHrs, decimal? lateHrs, decimal? underHrs, decimal? overbreakHrs, decimal? nightShiftHours, bool isLocked = false) : EntityBase<Guid>, IAggregateRoot
{
    public Guid EmployeeId { get; private set; } = employeeId;
    public Guid? ShiftId { get; private set; } = shiftId;
    public Guid? TimesheetId { get; private set; } = timesheetId;
    public Guid? LeaveId { get; private set; } = leaveId;
    public DateOnly? EntryDate { get; private set; } = entryDate;
    public decimal? WorkHrs { get; private set; } = workHrs;
    public decimal? LateHrs { get; private set; } = lateHrs;
    public decimal? UnderHrs { get; private set; } = underHrs;
    public decimal? OverbreakHrs { get; private set; } = overbreakHrs;
    public decimal? NightShiftHours { get; private set; } = nightShiftHours;
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

    public void Update(Guid? shiftId, Guid? timesheetId, Guid? leaveId, decimal? workHrs, decimal? lateHrs, decimal? underHrs, decimal? overbreakHrs, decimal? nightShiftHours, string? remarks, bool isLocked = false)
    {
        ShiftId = shiftId;
        TimesheetId = timesheetId;
        LeaveId = leaveId;
        WorkHrs = workHrs;
        LateHrs = lateHrs;
        UnderHrs = underHrs;
        OverbreakHrs = overbreakHrs;
        NightShiftHours = nightShiftHours;
        Remarks = remarks;
        IsEdited = true;
        IsLocked = isLocked;

        UpdatedDate = DateTime.Now;
    }

    public (decimal totalWorkHours, decimal totalLateHours, decimal totalUndertimeHours, decimal totalOverbreakHours, decimal totalNightShiftHours) CalculateWorkHours()
    {
        try
        {
            if (Timesheet == null || Shift == null || Timesheet.TimeIn1 == null || Timesheet.TimeOut1 == null)
                return (-1,0,0,0,0);

            //DateTime TimeIn1, DateTime TimeOut1, DateTime? TimeIn2, DateTime? TimeOut2,    
            DateTime TimeIn1 = Timesheet!.TimeIn1.Value;
            DateTime TimeOut1 = Timesheet!.TimeOut1.Value;
            DateTime? TimeIn2 = Timesheet!.TimeIn2;
            DateTime? TimeOut2 = Timesheet!.TimeOut2;

            //TimeSpan ShiftStartTime, TimeSpan ShiftBreakTime, TimeSpan ShiftBreakEndTime, TimeSpan ShiftEndTime
            var ShiftStartTime = Shift!.ShiftStartTime;
            var ShiftBreakTime = Shift!.ShiftBreakTime;
            var ShiftBreakEndTime = Shift!.ShiftBreakEndTime;
            var ShiftEndTime = Shift!.ShiftEndTime;


            double totalWorkHours = 0;
            double totalWorkHoursAM = 0;
            double totalWorkHoursPM = 0;

            DateTime adjustedShiftStartTime = TimeIn1.Date + (ShiftStartTime ?? TimeOnly.MinValue).ToTimeSpan();
            DateTime adjustedShiftBreakTime = TimeIn1.Date + (ShiftBreakTime ?? TimeOnly.MinValue).ToTimeSpan();
            DateTime adjustedShiftBreakEndTime = TimeIn1.Date + (ShiftBreakEndTime ?? TimeOnly.MinValue).ToTimeSpan();
            DateTime adjustedShiftEndTime = TimeIn1.Date + (ShiftEndTime ?? TimeOnly.MinValue).ToTimeSpan();

            // correct the dates for shfits.
            if (ShiftBreakTime < ShiftStartTime)
                adjustedShiftBreakTime = adjustedShiftBreakTime.AddDays(1); // move to the next day
            if (adjustedShiftBreakEndTime < adjustedShiftBreakTime)
                adjustedShiftBreakEndTime = adjustedShiftBreakEndTime.AddDays(1);
            if (adjustedShiftEndTime < adjustedShiftBreakEndTime)
                adjustedShiftEndTime = adjustedShiftEndTime.AddDays(1);

            // Adjust TimeIn1 for dynamic lateness rule
            TimeIn1 = AdjustForLateness(TimeIn1, adjustedShiftStartTime);

            // calculate AM first.
            // combine the TimeOut1.Date and ShiftBreakTime to be DateTime
            var _timeOut1 = ShiftBreakTime.HasValue 
                ? new DateTime(TimeOut1.Date.Year, TimeOut1.Date.Month, TimeOut1.Date.Day, ShiftBreakTime.Value.Hour, ShiftBreakTime.Value.Minute, ShiftBreakTime.Value.Second)
                : TimeOut1;
            totalWorkHoursAM = (_timeOut1 - TimeIn1).TotalHours;


            // intercepter for null TimeIn2 and TimeOut2. Fill in with possible values.
            if(!TimeIn2.HasValue && !TimeOut2.HasValue)
            {
                TimeIn2 = ShiftBreakEndTime.HasValue ? TimeOut1.Date + ShiftBreakEndTime.Value.ToTimeSpan() : TimeOut1;
                TimeOut2 = TimeOut1.Date + TimeOut1.TimeOfDay;
            }

            // Calculate actual break time
            if (TimeIn2.HasValue && TimeOut2.HasValue)
            {
                TimeSpan actualBreak = TimeIn2.Value - TimeOut1;
                TimeSpan expectedBreak = adjustedShiftBreakEndTime - adjustedShiftBreakTime;

                // Adjust TimeIn2 if the break is more than 10 minutes longer than expected
                if (actualBreak < expectedBreak.Add(TimeSpan.FromMinutes(10))) {
                    TimeIn2 = TimeIn2.Value.Date + (ShiftBreakEndTime.HasValue ? new TimeSpan(ShiftBreakEndTime.Value.Hour, ShiftBreakEndTime.Value.Minute, ShiftBreakEndTime.Value.Second) : TimeSpan.Zero);
                }
                else
                {
                    int i = 1;
                    while ((actualBreak - expectedBreak).TotalMinutes > 10)
                    {
                        TimeIn2 = TimeIn2.Value.Date + (ShiftBreakEndTime.HasValue ? new TimeSpan(ShiftBreakEndTime.Value.Hour + i, ShiftBreakEndTime.Value.Minute, ShiftBreakEndTime.Value.Second) : TimeSpan.Zero);
                        expectedBreak = TimeIn2.Value - TimeOut1;
                        i++;
                    }
                }

                // Adjust TimeOut2 for dynamic early exit rule
                TimeOut2 = AdjustForEarlyExit(TimeOut2.Value, adjustedShiftEndTime);

                if(TimeOut2.Value > TimeIn2.Value)
                    totalWorkHoursPM = (TimeOut2.Value - TimeIn2.Value).TotalHours;
            }

            if(totalWorkHoursAM != 0 && totalWorkHoursPM != 0)
                totalWorkHours = (adjustedShiftBreakEndTime - adjustedShiftBreakTime).TotalHours;

            totalWorkHours += totalWorkHoursAM + totalWorkHoursPM;

            // Convert the total work time to a decimal number of hours
            decimal totalLateHours = ShiftStartTime.HasValue 
                ? (decimal)((TimeIn1.TimeOfDay - ShiftStartTime.Value.ToTimeSpan()).TotalHours % 24) 
                : 0;

            decimal totalUndertimeHours = (decimal)((adjustedShiftEndTime - (TimeOut2 ?? adjustedShiftEndTime)).TotalHours % 24); 
            totalUndertimeHours = totalUndertimeHours > 8 ? 0 : totalUndertimeHours; // catch for undertime

            decimal totalOverbreakHours = (decimal)(((TimeIn2 ?? adjustedShiftBreakEndTime)  - adjustedShiftBreakEndTime).TotalHours % 24);
            totalOverbreakHours = TimeOut2.HasValue ? totalOverbreakHours : 0;
            totalOverbreakHours = totalOverbreakHours > 8 ? 0 : totalOverbreakHours; // catch for overbreak

            decimal totalNightShiftHours = (decimal)(CalculateNightShiftHours(TimeIn1, TimeOut1) +
                            (TimeIn2.HasValue && TimeOut2.HasValue ? CalculateNightShiftHours(TimeIn2.Value, TimeOut2.Value) : 0));
            totalNightShiftHours += totalNightShiftHours > 0 ? 0.5m : 0;
            // Return the total work hours

            // update the properties
            WorkHrs = (decimal)Math.Round(totalWorkHours,2);
            LateHrs = Math.Round(totalLateHours,2);
            UnderHrs = Math.Round(totalUndertimeHours,2);
            OverbreakHrs = Math.Round(totalOverbreakHours,2);
            NightShiftHours = Math.Round(totalNightShiftHours,2);

            return ((decimal)Math.Round(totalWorkHours,2), Math.Round(totalLateHours,2), Math.Round(totalUndertimeHours,2), Math.Round(totalOverbreakHours,2), Math.Round(totalNightShiftHours,2));
        }
        catch (Exception)
        {
            return (-1,0,0,0,0);
        }
    }

    private DateTime AdjustForLateness(DateTime timeIn, DateTime adjustedShiftStartTime)
    {
        if (timeIn > adjustedShiftStartTime)
        {
            TimeSpan lateness = timeIn - adjustedShiftStartTime;
            while (lateness.TotalMinutes > 10)
            {
                adjustedShiftStartTime = adjustedShiftStartTime.AddHours(1);
                lateness = timeIn - adjustedShiftStartTime;
            }
            timeIn = adjustedShiftStartTime;
        }
        else
        {
            timeIn = adjustedShiftStartTime;
        }
        return timeIn;            
    }

    private DateTime AdjustForEarlyExit(DateTime timeOut, DateTime adjustedShiftEndTime)
    {
        if (timeOut < adjustedShiftEndTime)
        {
            TimeSpan earlyExit = adjustedShiftEndTime - timeOut;
            int earlyHours = (int)Math.Ceiling(earlyExit.TotalMinutes / 60.0);
            timeOut = adjustedShiftEndTime.AddHours(-earlyHours);
        }
        else
        {
            timeOut = adjustedShiftEndTime;
        }
        return timeOut;
    }  

    private double CalculateNightShiftHours(DateTime timeIn, DateTime timeOut)
    {
        TimeSpan nightShiftStart = new TimeSpan(22, 0, 0); // 10:00 PM
        TimeSpan nightShiftEnd = new TimeSpan(6, 0, 0); // 6:00 AM

        double nightShiftHours = 0;

        DateTime current = timeIn;

        while (current < timeOut)
        {
            if (current.TimeOfDay >= nightShiftStart || current.TimeOfDay < nightShiftEnd)
            {
                DateTime end = current.Date + nightShiftEnd;
                if (current.TimeOfDay >= nightShiftStart)
                {
                    end = current.Date.AddDays(1) + nightShiftEnd;
                }
                if (timeOut < end)
                {
                    end = timeOut;
                }
                nightShiftHours += (end - current).TotalHours;
                current = end;
            }
            else
            {
                DateTime nextNightShiftStart = current.Date + nightShiftStart;
                if (current.TimeOfDay >= nightShiftEnd)
                {
                    nextNightShiftStart = current.Date.AddDays(1) + nightShiftStart;
                }
                current = nextNightShiftStart;
            }
        }

        return nightShiftHours;
    }    

}

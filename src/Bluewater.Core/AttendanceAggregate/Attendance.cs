using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.ShiftAggregate;
using Bluewater.Core.TimesheetAggregate;

namespace Bluewater.Core.AttendanceAggregate;
public class Attendance(Guid employeeId, Guid? shiftId, Guid? timesheetId, Guid? leaveId, DateOnly? entryDate, decimal? workHrs, decimal? lateHrs, decimal? underHrs, decimal? overbreakHrs, decimal? nightShiftHours, bool isLocked = false) : EntityBase<Guid>, IAggregateRoot
{
    private const double LatenessThresholdMinutes = 15;
    private const double BreakToleranceMinutes = 10;

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

  public (decimal totalWorkHours, decimal totalLateHours, decimal totalUndertimeHours,
          decimal totalOverbreakHours, decimal totalNightShiftHours) CalculateWorkHours()
  {
    try
    {
      // Check required objects.
      if (this.Timesheet == null || this.Shift == null)
        return (-1, 0, 0, 0, 0);

      var ts = this.Timesheet;
      var sh = this.Shift;

      // Ensure scheduled shift start and end are provided.
      if (!sh.ShiftStartTime.HasValue || !sh.ShiftEndTime.HasValue)
        return (-1, 0, 0, 0, 0);

      // Build scheduled start DateTime based on TimeIn1's date.
      DateTime scheduledStart = new DateTime(
          ts.TimeIn1!.Value.Year,
          ts.TimeIn1.Value.Month,
          ts.TimeIn1.Value.Day,
          sh.ShiftStartTime.Value.Hour,
          sh.ShiftStartTime.Value.Minute,
          sh.ShiftStartTime.Value.Second);

      // Determine scheduledEnd (account for overnight shifts)
      DateTime scheduledEnd = (sh.ShiftEndTime.Value.ToTimeSpan() >= sh.ShiftStartTime.Value.ToTimeSpan())
          ? new DateTime(
              ts.TimeIn1.Value.Year,
              ts.TimeIn1.Value.Month,
              ts.TimeIn1.Value.Day,
              sh.ShiftEndTime.Value.Hour,
              sh.ShiftEndTime.Value.Minute,
              sh.ShiftEndTime.Value.Second)
          : new DateTime(
              ts.TimeIn1.Value.Year,
              ts.TimeIn1.Value.Month,
              ts.TimeIn1.Value.Day,
              sh.ShiftEndTime.Value.Hour,
              sh.ShiftEndTime.Value.Minute,
              sh.ShiftEndTime.Value.Second).AddDays(1);

      decimal scheduledWorkHours = (decimal)(scheduledEnd - scheduledStart).TotalHours;

      // Determine if we have a full (two-session) record.
      // A full record exists when TimeOut1, TimeIn2, and TimeOut2 all have values.
      bool hasTwoSessions = ts.TimeOut1.HasValue && ts.TimeIn2.HasValue && ts.TimeOut2.HasValue;

      decimal penaltyLate = 0m, penaltyEarly = 0m, penaltyBreak = 0m;
      decimal netWorkHours = 0m;
      decimal nightShiftHrs = 0m;

      // Compute lateness penalty (common for both record types)
      {
        double lateMinutes = (ts.TimeIn1.Value - scheduledStart).TotalMinutes;
        if (lateMinutes >= LatenessThresholdMinutes)
          penaltyLate = (decimal)(Math.Floor((lateMinutes - 1) / 60) + 1);
      }

      if (!hasTwoSessions)
      {
        // SINGLE SESSION RECORD:
        // Use whichever clock-out is available (prefer TimeOut1 over TimeOut2)
        DateTime actualEnd = ts.TimeOut1.HasValue
            ? ts.TimeOut1.Value
            : ts.TimeOut2.HasValue
                ? ts.TimeOut2.Value
                : throw new InvalidOperationException("No clock-out time provided in single-session record.");

        // Compute early-exit penalty if actual end is before scheduled end.
        if (actualEnd < scheduledEnd)
        {
          double earlyMinutes = (scheduledEnd - actualEnd).TotalMinutes;
          penaltyEarly = (decimal)(Math.Floor((earlyMinutes - 1) / 60) + 1);
        }

        netWorkHours = scheduledWorkHours - (penaltyLate + penaltyEarly);
        netWorkHours = Math.Max(0, Math.Min(netWorkHours, scheduledWorkHours));

        // Compute night shift hours for single session if shift qualifies.
        if (sh.ShiftStartTime.Value.ToTimeSpan().Hours >= 22 || sh.ShiftEndTime.Value.ToTimeSpan().Hours <= 6)
          nightShiftHrs = netWorkHours;
      }
      else
      {
        // TWO-SESSION (FULL) RECORD:
        // Use TimeOut2 as the actual end for early exit penalty.
        DateTime actualEnd = ts.TimeOut2!.Value;
        if (actualEnd < scheduledEnd)
        {
          double earlyMinutes = (scheduledEnd - actualEnd).TotalMinutes;
          penaltyEarly = (decimal)(Math.Floor((earlyMinutes - 1) / 60) + 1);
        }

        // Overbreak penalty calculation:
        // Calculate scheduled break period.
        DateTime scheduledBreakStart, scheduledBreakEnd;
        if (sh.ShiftBreakTime.HasValue && sh.ShiftBreakEndTime.HasValue)
        {
          scheduledBreakStart = new DateTime(
              ts.TimeIn1.Value.Year,
              ts.TimeIn1.Value.Month,
              ts.TimeIn1.Value.Day,
              sh.ShiftBreakTime.Value.Hour,
              sh.ShiftBreakTime.Value.Minute,
              sh.ShiftBreakTime.Value.Second);

          scheduledBreakEnd = (sh.ShiftBreakEndTime.Value.ToTimeSpan() >= sh.ShiftBreakTime.Value.ToTimeSpan())
              ? new DateTime(
                  ts.TimeIn1.Value.Year,
                  ts.TimeIn1.Value.Month,
                  ts.TimeIn1.Value.Day,
                  sh.ShiftBreakEndTime.Value.Hour,
                  sh.ShiftBreakEndTime.Value.Minute,
                  sh.ShiftBreakEndTime.Value.Second)
              : new DateTime(
                  ts.TimeIn1.Value.Year,
                  ts.TimeIn1.Value.Month,
                  ts.TimeIn1.Value.Day,
                  sh.ShiftBreakEndTime.Value.Hour,
                  sh.ShiftBreakEndTime.Value.Minute,
                  sh.ShiftBreakEndTime.Value.Second).AddDays(1);
        }
        else
        {
          // Fallback: use BreakHours to calculate a default break period centered in the shift.
          double breakMinutes = (double)sh.BreakHours * 60;
          scheduledBreakStart = scheduledStart.AddHours((double)scheduledWorkHours / 2);
          scheduledBreakEnd = scheduledBreakStart.AddMinutes(breakMinutes);
        }

        double scheduledBreakMins = (scheduledBreakEnd - scheduledBreakStart).TotalMinutes;
        // Use the later of scheduledBreakStart or TimeOut1 as the effective break start.
        DateTime effectiveBreakStart = ts.TimeOut1!.Value < scheduledBreakStart ? scheduledBreakStart : ts.TimeOut1.Value;
        double actualBreakMins = (ts.TimeIn2!.Value - effectiveBreakStart).TotalMinutes;
        double excessBreak = actualBreakMins - scheduledBreakMins;
        if (excessBreak > BreakToleranceMinutes)
          penaltyBreak = (decimal)(Math.Floor((excessBreak - BreakToleranceMinutes - 1) / 60) + 1);

        netWorkHours = scheduledWorkHours - (penaltyLate + penaltyEarly + penaltyBreak);
        netWorkHours = Math.Max(0, Math.Min(netWorkHours, scheduledWorkHours));

        // Compute night shift hours for full record.
        if (sh.ShiftStartTime.Value.ToTimeSpan().Hours >= 22 || sh.ShiftEndTime.Value.ToTimeSpan().Hours <= 6)
          nightShiftHrs = netWorkHours;
      }

      // Round final values.
      WorkHrs = Math.Round(netWorkHours, 2);
      LateHrs = Math.Round(penaltyLate, 2);
      UnderHrs = Math.Round(penaltyEarly, 2);
      OverbreakHrs = Math.Round(penaltyBreak, 2);
      NightShiftHours = Math.Round(nightShiftHrs, 2);

      return (WorkHrs ?? 0, LateHrs ?? 0, UnderHrs ?? 0, OverbreakHrs ?? 0, NightShiftHours ?? 0);
    }
    catch (Exception ex)
    {
      // Optionally log the exception using your preferred logging framework.
      System.Diagnostics.Debug.WriteLine($"Error in CalculateWorkHours: {ex}");
      return (-1, 0, 0, 0, 0);
    }
  }


  // deprecated
  //public (decimal totalWorkHours, decimal totalLateHours, decimal totalUndertimeHours, decimal totalOverbreakHours, decimal totalNightShiftHours) CalculateWorkHours()
  //{
  //    try
  //    {
  //        if (Timesheet == null || Shift == null || Timesheet.TimeIn1 == null)
  //            return (-1,0,0,0,0);

  //        if(Timesheet.TimeOut1 == null && Timesheet.TimeIn2 == null)
  //        { 
  //            Timesheet.TimeOut1 = Timesheet.TimeOut2;
  //            Timesheet.TimeIn2 = null;
  //            Timesheet.TimeOut2 = null;  
  //        }

  //        //DateTime TimeIn1, DateTime TimeOut1, DateTime? TimeIn2, DateTime? TimeOut2,    
  //        DateTime TimeIn1 = Timesheet!.TimeIn1.Value;
  //        DateTime TimeOut1 = Timesheet!.TimeOut1 ?? TimeIn1;
  //        DateTime? TimeIn2 = Timesheet!.TimeIn2;
  //        DateTime? TimeOut2 = Timesheet!.TimeOut2;

  //        //TimeSpan ShiftStartTime, TimeSpan ShiftBreakTime, TimeSpan ShiftBreakEndTime, TimeSpan ShiftEndTime
  //        var ShiftStartTime = Shift!.ShiftStartTime;
  //        var ShiftBreakTime = Shift!.ShiftBreakTime;
  //        var ShiftBreakEndTime = Shift!.ShiftBreakEndTime;
  //        var ShiftEndTime = Shift!.ShiftEndTime;


  //        double totalWorkHours = 0;
  //        double totalWorkHoursAM = 0;
  //        double totalWorkHoursPM = 0;

  //        DateTime adjustedShiftStartTime = TimeIn1.Date + (ShiftStartTime ?? TimeOnly.MinValue).ToTimeSpan();
  //        DateTime adjustedShiftBreakTime = TimeIn1.Date + (ShiftBreakTime ?? TimeOnly.MinValue).ToTimeSpan();
  //        DateTime adjustedShiftBreakEndTime = TimeIn1.Date + (ShiftBreakEndTime ?? TimeOnly.MinValue).ToTimeSpan();
  //        DateTime adjustedShiftEndTime = TimeIn1.Date + (ShiftEndTime ?? TimeOnly.MinValue).ToTimeSpan();

  //        // correct the dates for shfits.
  //        if (ShiftBreakTime < ShiftStartTime)
  //            adjustedShiftBreakTime = adjustedShiftBreakTime.AddDays(1); // move to the next day
  //        if (adjustedShiftBreakEndTime < adjustedShiftBreakTime)
  //            adjustedShiftBreakEndTime = adjustedShiftBreakEndTime.AddDays(1);
  //        if (adjustedShiftEndTime < adjustedShiftBreakEndTime)
  //            adjustedShiftEndTime = adjustedShiftEndTime.AddDays(1);

  //        // Adjust TimeIn1 for dynamic lateness rule
  //        TimeIn1 = AdjustForLateness(TimeIn1, adjustedShiftStartTime);

  //        // calculate AM first.
  //        // combine the TimeOut1.Date and ShiftBreakTime to be DateTime
  //        var _timeOut1 = ShiftBreakTime.HasValue 
  //            ? new DateTime(TimeOut1.Date.Year, TimeOut1.Date.Month, TimeOut1.Date.Day, ShiftBreakTime.Value.Hour, ShiftBreakTime.Value.Minute, ShiftBreakTime.Value.Second)
  //            : TimeOut1;
  //        totalWorkHoursAM = (_timeOut1 - TimeIn1).TotalHours;


  //        // intercepter for null TimeIn2 and TimeOut2. Fill in with possible values.
  //        if(!TimeIn2.HasValue && !TimeOut2.HasValue)
  //        {
  //            TimeIn2 = ShiftBreakEndTime.HasValue ? TimeOut1.Date + ShiftBreakEndTime.Value.ToTimeSpan() : TimeOut1;
  //            TimeOut2 = TimeOut1.Date + TimeOut1.TimeOfDay;
  //        }

  //        // Calculate actual break time
  //        if (TimeIn2.HasValue && TimeOut2.HasValue)
  //        {
  //            TimeSpan actualBreak = TimeIn2.Value - TimeOut1;
  //            TimeSpan expectedBreak = adjustedShiftBreakEndTime - adjustedShiftBreakTime;

  //            // Adjust TimeIn2 if the break is more than 10 minutes longer than expected
  //            if (actualBreak < expectedBreak.Add(TimeSpan.FromMinutes(10))) {
  //                TimeIn2 = TimeIn2.Value.Date + (ShiftBreakEndTime.HasValue ? new TimeSpan(ShiftBreakEndTime.Value.Hour, ShiftBreakEndTime.Value.Minute, ShiftBreakEndTime.Value.Second) : TimeSpan.Zero);
  //            }
  //            else
  //            {
  //                int i = 1;
  //                while ((actualBreak - expectedBreak).TotalMinutes > 10)
  //                {
  //                    TimeIn2 = TimeIn2.Value.Date + (ShiftBreakEndTime.HasValue ? new TimeSpan(ShiftBreakEndTime.Value.Hour + i, ShiftBreakEndTime.Value.Minute, ShiftBreakEndTime.Value.Second) : TimeSpan.Zero);
  //                    expectedBreak = TimeIn2.Value - TimeOut1;
  //                    i++;
  //                }
  //            }

  //            // Adjust TimeOut2 for dynamic early exit rule
  //            TimeOut2 = AdjustForEarlyExit(TimeOut2.Value, adjustedShiftEndTime);

  //            if(TimeOut2.Value > TimeIn2.Value)
  //                totalWorkHoursPM = (TimeOut2.Value - TimeIn2.Value).TotalHours;
  //        }

  //        if(totalWorkHoursAM != 0 && totalWorkHoursPM != 0)
  //            totalWorkHours = (adjustedShiftBreakEndTime - adjustedShiftBreakTime).TotalHours;

  //        totalWorkHours += totalWorkHoursAM + totalWorkHoursPM;

  //        // Convert the total work time to a decimal number of hours
  //        decimal totalLateHours = ShiftStartTime.HasValue 
  //            ? (decimal)((TimeIn1.TimeOfDay - ShiftStartTime.Value.ToTimeSpan()).TotalHours % 24) 
  //            : 0;

  //        decimal totalUndertimeHours = (decimal)((adjustedShiftEndTime - (TimeOut2 ?? adjustedShiftEndTime)).TotalHours % 24); 
  //        totalUndertimeHours = totalUndertimeHours > 8 ? 0 : totalUndertimeHours; // catch for undertime

  //        decimal totalOverbreakHours = (decimal)(((TimeIn2 ?? adjustedShiftBreakEndTime)  - adjustedShiftBreakEndTime).TotalHours % 24);
  //        totalOverbreakHours = TimeOut2.HasValue ? totalOverbreakHours : 0;
  //        totalOverbreakHours = totalOverbreakHours > 8 ? 0 : totalOverbreakHours; // catch for overbreak

  //        decimal totalNightShiftHours = (decimal)(CalculateNightShiftHours(TimeIn1, TimeOut1) +
  //                        (TimeIn2.HasValue && TimeOut2.HasValue ? CalculateNightShiftHours(TimeIn2.Value, TimeOut2.Value) : 0));
  //        totalNightShiftHours += totalNightShiftHours > 0 ? 0.5m : 0;
  //        // Return the total work hours

  //        // update the properties
  //        WorkHrs = (decimal)Math.Round(totalWorkHours,2);
  //        LateHrs = Math.Round(totalLateHours,2);
  //        UnderHrs = Math.Round(totalUndertimeHours,2);
  //        OverbreakHrs = Math.Round(totalOverbreakHours,2);
  //        NightShiftHours = Math.Round(totalNightShiftHours,2);

  //        return ((decimal)Math.Round(totalWorkHours,2), Math.Round(totalLateHours,2), Math.Round(totalUndertimeHours,2), Math.Round(totalOverbreakHours,2), Math.Round(totalNightShiftHours,2));
  //    }
  //    catch (Exception)
  //    {
  //        return (-1,0,0,0,0);
  //    }
  //}


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

using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.ShiftAggregate;
using Bluewater.Core.TimesheetAggregate;

namespace Bluewater.Core.AttendanceAggregate;
public class Attendance(Guid employeeId, Guid? shiftId, Guid? timesheetId, Guid? leaveId, DateOnly? entryDate, decimal? workHrs, decimal? lateHrs, decimal? underHrs, decimal? overbreakHrs, decimal? nightShiftHours, bool isLocked = false) : EntityBase<Guid>, IAggregateRoot
{
    private const double LatenessThresholdMinutes = 15;
    private const double UndertimeThresholdMinutes = 15;
    private const double BreakToleranceMinutes = 10;
    private static readonly TimeOnly DefaultNightShiftStart = new(22, 0);
    private static readonly TimeOnly DefaultNightShiftEnd = new(6, 0);

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
          decimal totalOverbreakHours, decimal totalNightShiftHours) CalculateWorkHours(
            double lateThresholdMinutes = LatenessThresholdMinutes,
            double undertimeThresholdMinutes = UndertimeThresholdMinutes,
            double overbreakThresholdMinutes = BreakToleranceMinutes,
            bool enableNightShiftComputation = true,
            TimeOnly? nightShiftStartTime = null,
            TimeOnly? nightShiftEndTime = null)
  {
    try
    {
      if (Timesheet == null || Shift == null || !Timesheet.TimeIn1.HasValue || !Shift.ShiftStartTime.HasValue || !Shift.ShiftEndTime.HasValue)
      {
        return (-1, 0, 0, 0, 0);
      }

      Timesheet ts = Timesheet;
      Shift sh = Shift;

      DateTime scheduledStart = BuildShiftDateTime(ts.TimeIn1.Value, sh.ShiftStartTime.Value);
      DateTime scheduledEnd = BuildShiftDateTime(ts.TimeIn1.Value, sh.ShiftEndTime.Value);
      if (scheduledEnd <= scheduledStart)
      {
        scheduledEnd = scheduledEnd.AddDays(1);
      }

      (DateTime breakStart, DateTime breakEnd) = GetScheduledBreakWindow(scheduledStart, scheduledEnd, sh, ts.TimeIn1.Value);

      DateTime? effectiveTimeOut1 = ts.TimeOut1;
      if (!effectiveTimeOut1.HasValue && ts.TimeOut2.HasValue && sh.ShiftBreakTime.HasValue)
      {
        effectiveTimeOut1 = breakStart;
      }

      DateTime? effectiveTimeIn2 = ts.TimeIn2;
      if (!effectiveTimeIn2.HasValue && ts.TimeOut2.HasValue && sh.ShiftBreakEndTime.HasValue)
      {
        effectiveTimeIn2 = breakEnd;
      }

      DateTime? firstClockOut = effectiveTimeOut1 ?? ts.TimeOut2;
      if (!firstClockOut.HasValue)
      {
        WorkHrs = 0;
        LateHrs = 0;
        UnderHrs = 0;
        OverbreakHrs = 0;
        NightShiftHours = 0;
        return (0, 0, 0, 0, 0);
      }

      decimal shiftBreakHours = CalculateOverlapHours(breakStart, breakEnd, scheduledStart, scheduledEnd);
      decimal shiftWorkHours = Math.Max(0, (decimal)(scheduledEnd - scheduledStart).TotalHours - shiftBreakHours);

      bool hasSecondSession = effectiveTimeIn2.HasValue && ts.TimeOut2.HasValue;

      decimal workWithinSchedule = CalculateNetSessionHours(ts.TimeIn1.Value, firstClockOut.Value, scheduledStart, scheduledEnd, breakStart, breakEnd);
      if (hasSecondSession)
      {
        workWithinSchedule += CalculateNetSessionHours(effectiveTimeIn2!.Value, ts.TimeOut2!.Value, scheduledStart, scheduledEnd, breakStart, breakEnd);
      }

      decimal penaltyLate = CalculateRoundedHourPenalty((ts.TimeIn1.Value - scheduledStart).TotalMinutes, lateThresholdMinutes);

      DateTime attendanceEnd = hasSecondSession ? ts.TimeOut2!.Value : firstClockOut.Value;
      decimal penaltyEarly = 0m;
      if (attendanceEnd < scheduledEnd)
      {
        penaltyEarly = CalculateRoundedHourPenalty((scheduledEnd - attendanceEnd).TotalMinutes, undertimeThresholdMinutes);
      }

      decimal penaltyBreak = 0m;
      if (hasSecondSession && effectiveTimeOut1.HasValue)
      {
        double scheduledBreakMinutes = Math.Max(0, (breakEnd - breakStart).TotalMinutes);
        DateTime effectiveBreakStart = effectiveTimeOut1.Value < breakStart ? breakStart : effectiveTimeOut1.Value;
        double actualBreakMinutes = (effectiveTimeIn2!.Value - effectiveBreakStart).TotalMinutes;
        double excessBreakMinutes = actualBreakMinutes - scheduledBreakMinutes;

        penaltyBreak = CalculateRoundedHourPenalty(excessBreakMinutes, overbreakThresholdMinutes);
      }

      decimal netWorkHours = Math.Max(0, Math.Min(workWithinSchedule, shiftWorkHours));
      decimal roundedWorkHours = RoundToWholeHours(netWorkHours);
      decimal roundedLateHours = RoundToWholeHours(penaltyLate);
      decimal roundedUnderHours = RoundToWholeHours(penaltyEarly);
      decimal roundedOverbreakHours = RoundToWholeHours(penaltyBreak);

      decimal nightShiftHours = 0m;
      if (enableNightShiftComputation)
      {
        TimeOnly nsStart = nightShiftStartTime ?? DefaultNightShiftStart;
        TimeOnly nsEnd = nightShiftEndTime ?? DefaultNightShiftEnd;
        nightShiftHours = CalculateNightShiftHours(ts, effectiveTimeOut1, effectiveTimeIn2, scheduledStart, scheduledEnd, breakStart, breakEnd, nsStart, nsEnd);
      }
      decimal roundedNightShiftHours = RoundToWholeHours(nightShiftHours);

      WorkHrs = roundedWorkHours;
      LateHrs = roundedLateHours;
      UnderHrs = roundedUnderHours;
      OverbreakHrs = roundedOverbreakHours;
      NightShiftHours = roundedNightShiftHours;

      return (WorkHrs ?? 0, LateHrs ?? 0, UnderHrs ?? 0, OverbreakHrs ?? 0, NightShiftHours ?? 0);
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"Error in CalculateWorkHours: {ex}");
      return (-1, 0, 0, 0, 0);
    }
  }

  private static DateTime BuildShiftDateTime(DateTime referenceDate, TimeOnly shiftTime)
  {
    return new DateTime(referenceDate.Year, referenceDate.Month, referenceDate.Day, shiftTime.Hour, shiftTime.Minute, shiftTime.Second);
  }

  private static decimal CalculateOverlapHours(DateTime intervalStart, DateTime intervalEnd, DateTime rangeStart, DateTime rangeEnd)
  {
    if (intervalEnd <= intervalStart)
    {
      return 0m;
    }

    DateTime overlapStart = intervalStart > rangeStart ? intervalStart : rangeStart;
    DateTime overlapEnd = intervalEnd < rangeEnd ? intervalEnd : rangeEnd;

    return overlapEnd > overlapStart
      ? (decimal)(overlapEnd - overlapStart).TotalHours
      : 0m;
  }

  private static decimal CalculateNetSessionHours(
    DateTime sessionStart,
    DateTime sessionEnd,
    DateTime scheduledStart,
    DateTime scheduledEnd,
    DateTime breakStart,
    DateTime breakEnd)
  {
    decimal grossOverlap = CalculateOverlapHours(sessionStart, sessionEnd, scheduledStart, scheduledEnd);
    decimal breakOverlap = CalculateOverlapHours(sessionStart, sessionEnd, breakStart, breakEnd);
    return Math.Max(0m, grossOverlap - breakOverlap);
  }

  private static decimal CalculateRoundedHourPenalty(double minutes, double thresholdMinutes)
  {
    if (minutes <= thresholdMinutes)
    {
      return 0m;
    }

    return (decimal)(Math.Floor((minutes - thresholdMinutes - 1) / 60) + 1);
  }

  private static decimal RoundToWholeHours(decimal value)
  {
    return Math.Round(value, 0, MidpointRounding.AwayFromZero);
  }

  private static (DateTime breakStart, DateTime breakEnd) GetScheduledBreakWindow(DateTime scheduledStart, DateTime scheduledEnd, Shift shift, DateTime referenceDate)
  {
    if (shift.ShiftBreakTime.HasValue && shift.ShiftBreakEndTime.HasValue)
    {
      DateTime breakStart = BuildShiftDateTime(referenceDate, shift.ShiftBreakTime.Value);
      DateTime breakEnd = BuildShiftDateTime(referenceDate, shift.ShiftBreakEndTime.Value);
      if (breakEnd <= breakStart)
      {
        breakEnd = breakEnd.AddDays(1);
      }

      return (breakStart, breakEnd);
    }

    DateTime defaultBreakStart = scheduledStart.AddHours((scheduledEnd - scheduledStart).TotalHours / 2d);
    DateTime defaultBreakEnd = defaultBreakStart.AddHours((double)shift.BreakHours);
    return (defaultBreakStart, defaultBreakEnd);
  }

  private static decimal CalculateNightShiftHours(
    Timesheet timesheet,
    DateTime? effectiveTimeOut1,
    DateTime? effectiveTimeIn2,
    DateTime scheduledStart,
    DateTime scheduledEnd,
    DateTime breakStart,
    DateTime breakEnd,
    TimeOnly nightShiftStartTime,
    TimeOnly nightShiftEndTime)
  {
    DateTime nightStart = BuildShiftDateTime(scheduledStart, nightShiftStartTime);
    DateTime nightEnd = BuildShiftDateTime(scheduledStart, nightShiftEndTime);
    if (nightEnd <= nightStart)
    {
      nightEnd = nightEnd.AddDays(1);
    }

    DateTime effectiveNightStart = nightStart > scheduledStart ? nightStart : scheduledStart;
    DateTime effectiveNightEnd = nightEnd < scheduledEnd ? nightEnd : scheduledEnd;
    if (effectiveNightEnd <= effectiveNightStart)
    {
      return 0m;
    }

    decimal total = 0m;
    if (effectiveTimeOut1.HasValue)
    {
      total += CalculateNetSessionHours(timesheet.TimeIn1!.Value, effectiveTimeOut1.Value, effectiveNightStart, effectiveNightEnd, breakStart, breakEnd);
    }

    if (effectiveTimeIn2.HasValue && timesheet.TimeOut2.HasValue)
    {
      total += CalculateNetSessionHours(effectiveTimeIn2.Value, timesheet.TimeOut2.Value, effectiveNightStart, effectiveNightEnd, breakStart, breakEnd);
    }

    return total;
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

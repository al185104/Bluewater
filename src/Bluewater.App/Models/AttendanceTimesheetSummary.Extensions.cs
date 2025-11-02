using System;

namespace Bluewater.App.Models;

public partial class AttendanceTimesheetSummary
{
  public bool HasTimeIn1 => TimeIn1.HasValue;
  public bool HasTimeOut1 => TimeOut1.HasValue;
  public bool HasTimeIn2 => TimeIn2.HasValue;
  public bool HasTimeOut2 => TimeOut2.HasValue;

  public DateTime TimeIn1Date => GetDateOrFallback(TimeIn1);
  public DateTime TimeOut1Date => GetDateOrFallback(TimeOut1);
  public DateTime TimeIn2Date => GetDateOrFallback(TimeIn2);
  public DateTime TimeOut2Date => GetDateOrFallback(TimeOut2);

  public TimeSpan TimeIn1Time => TimeIn1?.TimeOfDay ?? TimeSpan.Zero;
  public TimeSpan TimeOut1Time => TimeOut1?.TimeOfDay ?? TimeSpan.Zero;
  public TimeSpan TimeIn2Time => TimeIn2?.TimeOfDay ?? TimeSpan.Zero;
  public TimeSpan TimeOut2Time => TimeOut2?.TimeOfDay ?? TimeSpan.Zero;

  private DateTime GetDateOrFallback(DateTime? value)
  {
    if (value is DateTime dateTime)
    {
      return dateTime.Date;
    }

    if (EntryDate is DateOnly entryDate)
    {
      return entryDate.ToDateTime(TimeOnly.MinValue);
    }

    return DateTime.Today;
  }
}

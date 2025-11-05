using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.UI.WebUI;

namespace Bluewater.App.Models;

public partial class EditableTimesheetEntry : ObservableObject
{
  private DateTime? originalTimeIn1;
  private DateTime? originalTimeOut1;
  private DateTime? originalTimeIn2;
  private DateTime? originalTimeOut2;
  private DateOnly? originalEntryDate;
  private bool originalIsLocked;

  [ObservableProperty]
  public partial Guid Id { get; set; }

  [ObservableProperty]
  public partial Guid EmployeeId { get; set; }

  [ObservableProperty]
  public partial DateOnly? EntryDate { get; set; }

  [ObservableProperty]
  public partial DateTime? TimeIn1 { get; set; }

  [ObservableProperty]
  public partial DateTime? TimeOut1 { get; set; }

  [ObservableProperty]
  public partial DateTime? TimeIn2 { get; set; }

  [ObservableProperty]
  public partial DateTime? TimeOut2 { get; set; }

  [ObservableProperty]
  public partial bool IsEdited { get; set; }

  [ObservableProperty]
  public partial bool IsLocked { get; set; }

  public DateTime EntryDateDateTime
  {
    get => (EntryDate ?? DateOnly.FromDateTime(DateTime.Today)).ToDateTime(TimeOnly.MinValue);
    set => EntryDate = DateOnly.FromDateTime(value);
  }

  public bool HasTimeIn1
  {
    get => TimeIn1.HasValue;
    set
    {
      if (value == HasTimeIn1)
      {
        return;
      }

      if (!value)
      {
        TimeIn1 = null;
      }
      else
      {
        TimeIn1 = CombineDateAndTime(EntryDate, TimeIn1Time);
      }

      OnPropertyChanged(nameof(HasTimeIn1));
      OnPropertyChanged(nameof(TimeIn1Time));
      OnPropertyChanged(nameof(HasChanges));
    }
  }

  public bool HasTimeOut1
  {
    get => TimeOut1.HasValue;
    set
    {
      if (value == HasTimeOut1)
      {
        return;
      }

      if (!value)
      {
        TimeOut1 = null;
      }
      else
      {
        TimeOut1 = CombineDateAndTime(EntryDate, TimeOut1Time);
      }

      OnPropertyChanged(nameof(HasTimeOut1));
      OnPropertyChanged(nameof(TimeOut1Time));
      OnPropertyChanged(nameof(HasChanges));
    }
  }

  public bool HasTimeIn2
  {
    get => TimeIn2.HasValue;
    set
    {
      if (value == HasTimeIn2)
      {
        return;
      }

      if (!value)
      {
        TimeIn2 = null;
      }
      else
      {
        TimeIn2 = CombineDateAndTime(EntryDate, TimeIn2Time);
      }

      OnPropertyChanged(nameof(HasTimeIn2));
      OnPropertyChanged(nameof(TimeIn2Time));
      OnPropertyChanged(nameof(HasChanges));
    }
  }

  public bool HasTimeOut2
  {
    get => TimeOut2.HasValue;
    set
    {
      if (value == HasTimeOut2)
      {
        return;
      }

      if (!value)
      {
        TimeOut2 = null;
      }
      else
      {
        TimeOut2 = CombineDateAndTime(EntryDate, TimeOut2Time);
      }

      OnPropertyChanged(nameof(HasTimeOut2));
      OnPropertyChanged(nameof(TimeOut2Time));
      OnPropertyChanged(nameof(HasChanges));
    }
  }

  public TimeSpan TimeIn1Time
  {
    get => TimeIn1?.TimeOfDay ?? TimeSpan.Zero;
    set
    {
      if (!HasTimeIn1 && value == TimeSpan.Zero)
      {
        return;
      }

      TimeIn1 = CombineDateAndTime(EntryDate, value);
    }
  }

  public TimeSpan TimeOut1Time
  {
    get => TimeOut1?.TimeOfDay ?? TimeSpan.Zero;
    set
    {
      if (!HasTimeOut1 && value == TimeSpan.Zero)
      {
        return;
      }

      TimeOut1 = CombineDateAndTime(EntryDate, value);
    }
  }

  public TimeSpan TimeIn2Time
  {
    get => TimeIn2?.TimeOfDay ?? TimeSpan.Zero;
    set
    {
      if (!HasTimeIn2 && value == TimeSpan.Zero)
      {
        return;
      }

      TimeIn2 = CombineDateAndTime(EntryDate, value);
    }
  }

  public TimeSpan TimeOut2Time
  {
    get => TimeOut2?.TimeOfDay ?? TimeSpan.Zero;
    set
    {
      if (!HasTimeOut2 && value == TimeSpan.Zero)
      {
        return;
      }

      TimeOut2 = CombineDateAndTime(EntryDate, value);
    }
  }

  public bool HasChanges =>
    EntryDate != originalEntryDate ||
    TimeIn1 != originalTimeIn1 ||
    TimeOut1 != originalTimeOut1 ||
    TimeIn2 != originalTimeIn2 ||
    TimeOut2 != originalTimeOut2 ||
    IsLocked != originalIsLocked;

  partial void OnEntryDateChanged(DateOnly? value)
  {
    if (value is DateOnly date)
    {
      TimeIn1 = UpdateDateComponent(TimeIn1, date);
      TimeOut1 = UpdateDateComponent(TimeOut1, date);
      TimeIn2 = UpdateDateComponent(TimeIn2, date);
      TimeOut2 = UpdateDateComponent(TimeOut2, date);
    }

    OnPropertyChanged(nameof(EntryDateDateTime));
    OnPropertyChanged(nameof(HasChanges));
  }

  partial void OnTimeIn1Changed(DateTime? value)
  {
    OnPropertyChanged(nameof(HasTimeIn1));
    OnPropertyChanged(nameof(TimeIn1Time));
    OnPropertyChanged(nameof(HasChanges));
  }

  partial void OnTimeOut1Changed(DateTime? value)
  {
    OnPropertyChanged(nameof(HasTimeOut1));
    OnPropertyChanged(nameof(TimeOut1Time));
    OnPropertyChanged(nameof(HasChanges));
  }

  partial void OnTimeIn2Changed(DateTime? value)
  {
    OnPropertyChanged(nameof(HasTimeIn2));
    OnPropertyChanged(nameof(TimeIn2Time));
    OnPropertyChanged(nameof(HasChanges));
  }

  partial void OnTimeOut2Changed(DateTime? value)
  {
    OnPropertyChanged(nameof(HasTimeOut2));
    OnPropertyChanged(nameof(TimeOut2Time));
    OnPropertyChanged(nameof(HasChanges));
  }

  partial void OnIsLockedChanged(bool value)
  {
    OnPropertyChanged(nameof(HasChanges));
  }

  public static EditableTimesheetEntry FromSummary(AttendanceTimesheetSummary summary)
  {
    var entry = new EditableTimesheetEntry
    {
      Id = summary.Id,
      EmployeeId = summary.EmployeeId,
      EntryDate = summary.EntryDate,
      TimeIn1 = new DateTime(summary.TimeIn1Date.Year, summary.TimeIn1Date.Month, summary.TimeIn1Date.Day, summary.TimeIn1Time.Hours, summary.TimeIn1Time.Minutes, summary.TimeIn1Time.Seconds),
      TimeOut1 = new DateTime(summary.TimeOut1Date.Year, summary.TimeOut1Date.Month, summary.TimeOut1Date.Day, summary.TimeOut1Time.Hours, summary.TimeOut1Time.Minutes, summary.TimeOut1Time.Seconds),
      TimeIn2 = new DateTime(summary.TimeIn2Date.Year, summary.TimeIn2Date.Month, summary.TimeIn2Date.Day, summary.TimeIn2Time.Hours, summary.TimeIn2Time.Minutes, summary.TimeIn2Time.Seconds),
      TimeOut2 = new DateTime(summary.TimeOut2Date.Year, summary.TimeOut2Date.Month, summary.TimeOut2Date.Day, summary.TimeOut2Time.Hours, summary.TimeOut2Time.Minutes, summary.TimeOut2Time.Seconds),
      IsEdited = summary.IsEdited,
      IsLocked = false
    };

    entry.ResetChangeTracking();
    return entry;
  }

  public void ApplySummary(AttendanceTimesheetSummary summary)
  {
    EntryDate = summary.EntryDate;
    TimeIn1 = summary.TimeIn1;
    TimeOut1 = summary.TimeOut1;
    TimeIn2 = summary.TimeIn2;
    TimeOut2 = summary.TimeOut2;
    IsEdited = summary.IsEdited;
    ResetChangeTracking();
  }

  public UpdateTimesheetRequestDto ToUpdateRequest()
  {
    return new UpdateTimesheetRequestDto
    {
      Id = Id,
      EmployeeId = EmployeeId,
      TimeIn1 = TimeIn1,
      TimeOut1 = TimeOut1,
      TimeIn2 = TimeIn2,
      TimeOut2 = TimeOut2,
      EntryDate = EntryDate,
      IsLocked = IsLocked
    };
  }

  public void ResetChangeTracking()
  {
    originalEntryDate = EntryDate;
    originalTimeIn1 = TimeIn1;
    originalTimeOut1 = TimeOut1;
    originalTimeIn2 = TimeIn2;
    originalTimeOut2 = TimeOut2;
    originalIsLocked = IsLocked;
    OnPropertyChanged(nameof(HasChanges));
  }

  private static DateTime CombineDateAndTime(DateOnly? date, TimeSpan time)
  {
    DateOnly resolvedDate = date ?? DateOnly.FromDateTime(DateTime.Today);
    TimeOnly resolvedTime = TimeOnly.FromTimeSpan(time);
    return resolvedDate.ToDateTime(resolvedTime);
  }

  private static DateTime? UpdateDateComponent(DateTime? value, DateOnly date)
  {
    if (!value.HasValue)
    {
      return value;
    }

    TimeOnly time = TimeOnly.FromDateTime(value.Value);
    value = date.ToDateTime(time);
    return value;
  }
}

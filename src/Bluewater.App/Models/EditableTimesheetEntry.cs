using System;
using CommunityToolkit.Mvvm.ComponentModel;

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
  private Guid id;

  [ObservableProperty]
  private Guid employeeId;

  [ObservableProperty]
  private DateOnly? entryDate;

  [ObservableProperty]
  private DateTime? timeIn1;

  [ObservableProperty]
  private DateTime? timeOut1;

  [ObservableProperty]
  private DateTime? timeIn2;

  [ObservableProperty]
  private DateTime? timeOut2;

  [ObservableProperty]
  private bool isEdited;

  [ObservableProperty]
  private bool isLocked;

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
      UpdateDateComponent(ref timeIn1, date);
      UpdateDateComponent(ref timeOut1, date);
      UpdateDateComponent(ref timeIn2, date);
      UpdateDateComponent(ref timeOut2, date);
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
      TimeIn1 = summary.TimeIn1,
      TimeOut1 = summary.TimeOut1,
      TimeIn2 = summary.TimeIn2,
      TimeOut2 = summary.TimeOut2,
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

  private static void UpdateDateComponent(ref DateTime? value, DateOnly date)
  {
    if (!value.HasValue)
    {
      return;
    }

    TimeOnly time = TimeOnly.FromDateTime(value.Value);
    value = date.ToDateTime(time);
  }
}

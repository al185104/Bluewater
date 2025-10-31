using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Bluewater.App.Helpers;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

public partial class ScheduleViewModel : BaseViewModel
{
  private readonly IScheduleApiService scheduleApiService;
  private readonly IShiftApiService shiftApiService;
  private readonly IReferenceDataService referenceDataService;
  private readonly SemaphoreSlim updateSemaphore = new(1, 1);
  private readonly Dictionary<Guid, ShiftOption> shiftOptionLookup = new();
  private bool hasInitialized;
  private bool suppressWeekChangeReload;

  public ScheduleViewModel(
    IScheduleApiService scheduleApiService,
    IShiftApiService shiftApiService,
    IReferenceDataService referenceDataService,
    IActivityTraceService activityTraceService,
    IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
    this.scheduleApiService = scheduleApiService;
    this.shiftApiService = shiftApiService;
    this.referenceDataService = referenceDataService;
    ImportStatusMessage = string.Empty;

    WeekStart = GetStartOfWeek(DateOnly.FromDateTime(DateTime.Today));

    EmployeeSchedules.CollectionChanged += OnEmployeeSchedulesCollectionChanged;
  }

  public ObservableCollection<EmployeeWeeklyScheduleViewModel> EmployeeSchedules { get; } = new();

  public ObservableCollection<ShiftOption> ShiftOptions { get; } = new();

  public ObservableCollection<ChargingSummary> Chargings { get; } = new();

  public ObservableCollection<WeekDayHeaderViewModel> WeekDays { get; } = new();

  [ObservableProperty]
  public partial ChargingSummary? SelectedCharging { get; set; }

  [ObservableProperty]
  [NotifyPropertyChangedFor(nameof(WeekEnd))]
  [NotifyPropertyChangedFor(nameof(WeekRangeDisplay))]
  public partial DateOnly WeekStart { get; set; }

  [ObservableProperty]
  public partial bool IsLoadingSchedules { get; set; }

  [ObservableProperty]
  public partial string ImportStatusMessage { get; set; }

  [ObservableProperty]
  public partial bool HasImportStatusMessage { get; set; }

  public DateOnly WeekEnd => WeekStart.AddDays(6);

  public string WeekRangeDisplay => $"{WeekStart:MMM d} – {WeekEnd:MMM d, yyyy}";

  public bool HasSchedules => EmployeeSchedules.Count > 0;

  public bool HasPendingChanges => EmployeeSchedules.Any(s => s.HasPendingChanges);

  partial void OnImportStatusMessageChanged(string value)
  {
    HasImportStatusMessage = !string.IsNullOrWhiteSpace(value);
  }

  partial void OnWeekStartChanged(DateOnly value)
  {
    UpdateWeekDays();

    if (hasInitialized && !suppressWeekChangeReload)
    {
      _ = LoadSchedulesAsync();
    }
  }

  partial void OnSelectedChargingChanged(ChargingSummary? value)
  {
    if (hasInitialized && value is not null)
    {
      _ = LoadSchedulesAsync();
    }
  }

  private void OnEmployeeSchedulesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
  {
    OnPropertyChanged(nameof(HasSchedules));

    if (e.OldItems is not null)
    {
      foreach (EmployeeWeeklyScheduleViewModel schedule in e.OldItems.Cast<EmployeeWeeklyScheduleViewModel>())
      {
        schedule.PropertyChanged -= OnEmployeeSchedulePropertyChanged;
      }
    }

    if (e.NewItems is not null)
    {
      foreach (EmployeeWeeklyScheduleViewModel schedule in e.NewItems.Cast<EmployeeWeeklyScheduleViewModel>())
      {
        schedule.PropertyChanged += OnEmployeeSchedulePropertyChanged;
      }
    }

    if (e.Action == NotifyCollectionChangedAction.Reset)
    {
      foreach (EmployeeWeeklyScheduleViewModel schedule in EmployeeSchedules)
      {
        schedule.PropertyChanged -= OnEmployeeSchedulePropertyChanged;
        schedule.PropertyChanged += OnEmployeeSchedulePropertyChanged;
      }
    }

    RefreshPendingChangesState();
  }

  public override async Task InitializeAsync()
  {
    if (IsBusy || hasInitialized)
    {
      return;
    }

    try
    {
      IsBusy = true;

      await referenceDataService.InitializeAsync().ConfigureAwait(false);
      LoadChargings();
      await LoadShiftsAsync().ConfigureAwait(false);
      //UpdateWeekDays();

      hasInitialized = true;

      await LoadSchedulesAsync().ConfigureAwait(false);
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Loading schedules");
    }
    finally
    {
      IsBusy = false;
    }
  }

  [RelayCommand(CanExecute = nameof(CanUpdateSchedules))]
  private async Task UpdateSchedulesAsync()
  {
    if (!HasPendingChanges)
    {
      return;
    }

    try
    {
      IsBusy = true;

      foreach (EmployeeWeeklyScheduleViewModel employee in EmployeeSchedules)
      {
        foreach (EmployeeScheduleDayViewModel day in employee.Days.Where(d => d.HasPendingChanges))
        {
          ShiftOption? option = day.SelectedShift;
          await HandleDaySelectionChangedAsync(day, option).ConfigureAwait(false);
        }
      }

      await TraceCommandAsync(nameof(UpdateSchedulesAsync), new
      {
        Charging = SelectedCharging?.Name,
        WeekStart,
        WeekEnd,
        EmployeeCount = EmployeeSchedules.Count
      }).ConfigureAwait(false);
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Updating schedules");
    }
    finally
    {
      IsBusy = false;
      RefreshPendingChangesState();
    }
  }

  private bool CanUpdateSchedules()
  {
    return HasPendingChanges && !IsBusy && !IsLoadingSchedules;
  }

  [RelayCommand]
  private async Task NextWeekAsync()
  {
    await ChangeWeekAsync(7).ConfigureAwait(false);
  }

  [RelayCommand]
  private async Task PreviousWeekAsync()
  {
    await ChangeWeekAsync(-7).ConfigureAwait(false);
  }

  [RelayCommand]
  private async Task ImportSchedulesAsync()
  {
    if (IsBusy)
    {
      return;
    }

    ImportStatusMessage = string.Empty;

    try
    {
      PickOptions options = new()
      {
        PickerTitle = "Select schedules CSV file",
        FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
          [DevicePlatform.iOS] = new[] { "public.comma-separated-values-text", "public.text" },
          [DevicePlatform.Android] = new[] { "text/csv", "text/comma-separated-values" },
          [DevicePlatform.WinUI] = new[] { ".csv" },
          [DevicePlatform.MacCatalyst] = new[] { "public.comma-separated-values-text", "public.text" }
        })
      };

      FileResult? file = await FilePicker.Default.PickAsync(options).ConfigureAwait(false);

      if (file is null)
      {
        return;
      }

      await using Stream stream = await file.OpenReadAsync().ConfigureAwait(false);
      IReadOnlyList<ScheduleCsvRecord> records = await ScheduleCsvImporter.ParseAsync(stream).ConfigureAwait(false);

      if (records.Count == 0)
      {
        ImportStatusMessage = "No schedules were found in the selected file.";
        return;
      }

      IsBusy = true;

      int successCount = 0;
      foreach (ScheduleCsvRecord record in records)
      {
        try
        {
          await PersistImportedScheduleAsync(record).ConfigureAwait(false);
          successCount++;
        }
        catch (Exception ex)
        {
          ExceptionHandlingService.Handle(ex, $"Importing schedule for employee {record.EmployeeId}");
        }
      }

      ImportStatusMessage = $"Imported {successCount} of {records.Count} schedules.";

      await TraceCommandAsync(nameof(ImportSchedulesAsync), new
      {
        Count = successCount,
        Total = records.Count,
        File = file.FileName
      }).ConfigureAwait(false);

      await LoadSchedulesAsync().ConfigureAwait(false);
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Importing schedules");
      ImportStatusMessage = "Unable to import schedules. Please verify the CSV file.";
    }
    finally
    {
      IsBusy = false;
    }
  }

  internal ShiftOption GetShiftOption(Guid? shiftId, ScheduleShiftDetailsSummary? shiftDetails)
  {
    if (!shiftId.HasValue || shiftId.Value == Guid.Empty)
    {
      return EnsureNoneOption();
    }

    if (shiftOptionLookup.TryGetValue(shiftId.Value, out ShiftOption? existing))
    {
      return existing;
    }

    string name = string.IsNullOrWhiteSpace(shiftDetails?.Name) ? "Shift" : shiftDetails!.Name;
    string? range = shiftDetails is null ? null : FormatTimeRange(shiftDetails.ShiftStartTime, shiftDetails.ShiftEndTime);

    var option = new ShiftOption(shiftId.Value, name, range);
    return AddShiftOption(option);
  }

  internal async Task HandleDaySelectionChangedAsync(EmployeeScheduleDayViewModel day, ShiftOption? option)
  {
    if (day is null)
    {
      return;
    }

    ShiftOption targetOption = option ?? EnsureNoneOption();

    await updateSemaphore.WaitAsync().ConfigureAwait(false);
    try
    {
      day.IsProcessing = true;

      if (targetOption.Id == Guid.Empty)
      {
        if (day.ScheduleId != Guid.Empty)
        {
          try
          {
            bool deleted = await scheduleApiService.DeleteScheduleAsync(day.ScheduleId).ConfigureAwait(false);

            if (!deleted)
            {
              day.RestoreCommittedSelection();
              return;
            }

            day.UpdateScheduleState(Guid.Empty, day.IsDefault);
            day.SetCommittedSelection(targetOption);
          }
          catch (Exception ex)
          {
            day.RestoreCommittedSelection();
            ExceptionHandlingService.Handle(ex, "Removing schedule");
            return;
          }
        }
        else
        {
          day.SetCommittedSelection(targetOption);
        }

        await TraceCommandAsync("ClearScheduleDay", new
        {
          day.EmployeeId,
          day.Date
        }).ConfigureAwait(false);

        return;
      }

      var summary = new ScheduleSummary
      {
        Id = day.ScheduleId,
        EmployeeId = day.EmployeeId,
        ShiftId = targetOption.Id,
        ScheduleDate = day.Date,
        Name = targetOption.Name,
        IsDefault = day.IsDefault
      };

      try
      {
        if (day.ScheduleId == Guid.Empty)
        {
          Guid? newId = await scheduleApiService.CreateScheduleAsync(summary).ConfigureAwait(false);

          if (newId.HasValue)
          {
            day.UpdateScheduleState(newId.Value, summary.IsDefault);
            day.SetCommittedSelection(targetOption);
          }
          else
          {
            day.RestoreCommittedSelection();
            return;
          }
        }
        else
        {
          ScheduleSummary? updated = await scheduleApiService.UpdateScheduleAsync(summary).ConfigureAwait(false);

          if (updated is not null)
          {
            day.UpdateScheduleState(updated.Id, updated.IsDefault);
            day.SetCommittedSelection(targetOption);
          }
          else
          {
            day.RestoreCommittedSelection();
            return;
          }
        }
      }
      catch (Exception ex)
      {
        day.RestoreCommittedSelection();
        ExceptionHandlingService.Handle(ex, "Updating schedule");
        return;
      }

      await TraceCommandAsync("AssignScheduleDay", new
      {
        day.EmployeeId,
        day.Date,
        ShiftId = targetOption.Id,
        targetOption.Name
      }).ConfigureAwait(false);
    }
    finally
    {
      day.IsProcessing = false;
      updateSemaphore.Release();
    }
  }

  private async Task ChangeWeekAsync(int days)
  {
    if (!hasInitialized)
    {
      return;
    }

    suppressWeekChangeReload = true;
    WeekStart = WeekStart.AddDays(days);
    suppressWeekChangeReload = false;

    await TraceCommandAsync(days > 0 ? "NextWeek" : "PreviousWeek", new
    {
      WeekStart,
      WeekEnd
    }).ConfigureAwait(false);

    await LoadSchedulesAsync().ConfigureAwait(false);
  }

  private void LoadChargings()
  {
    Chargings.Clear();

    foreach (ChargingSummary charging in referenceDataService.Chargings
               .OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase))
    {
      Chargings.Add(charging);
    }

    if (Chargings.Count == 0)
    {
      SelectedCharging = null;
      return;
    }

    if (SelectedCharging is null || Chargings.All(c => c.Id != SelectedCharging.Id))
    {
      SelectedCharging = Chargings[0];
    }
  }

  private async Task LoadShiftsAsync()
  {
    ShiftOptions.Clear();
    shiftOptionLookup.Clear();

    ShiftOption none = EnsureNoneOption();

    IReadOnlyList<ShiftSummary> shifts = await shiftApiService.GetShiftsAsync().ConfigureAwait(false);

    foreach (ShiftSummary shift in shifts.OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase))
    {
      string? range = FormatTimeRange(shift);
      _ = AddShiftOption(new ShiftOption(shift.Id, shift.Name, range));
    }

    await TraceCommandAsync(nameof(LoadShiftsAsync), new
    {
      ShiftCount = ShiftOptions.Count,
      HasNoneOption = none is not null
    }).ConfigureAwait(false);
  }




  private async Task LoadSchedulesAsync()
  {
    if (SelectedCharging is null)
    {
      DetachScheduleHandlers();
      EmployeeSchedules.Clear();
      RefreshPendingChangesState();
      return;
    }

    try
    {
      IsLoadingSchedules = true;

      IReadOnlyList<EmployeeScheduleSummary> summaries = await scheduleApiService
        .GetSchedulesAsync(SelectedCharging.Name, WeekStart, WeekEnd)
        .ConfigureAwait(false);

      DetachScheduleHandlers();
      EmployeeSchedules.Clear();

      foreach (EmployeeScheduleSummary summary in summaries
                 .OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase))
      {
        var employee = new EmployeeWeeklyScheduleViewModel(this, summary, WeekStart, WeekEnd);
        EmployeeSchedules.Add(employee);
      }

      RefreshPendingChangesState();

      await TraceCommandAsync(nameof(LoadSchedulesAsync), new
      {
        WeekStart,
        WeekEnd,
        Charging = SelectedCharging.Name,
        EmployeeCount = EmployeeSchedules.Count
      }).ConfigureAwait(false);
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Loading schedules");
    }
    finally
    {
      IsLoadingSchedules = false;
    }
  }

  internal void NotifyDaySelectionChanged()
  {
    RefreshPendingChangesState();
  }

  private void RefreshPendingChangesState()
  {
    OnPropertyChanged(nameof(HasPendingChanges));
    UpdateSchedulesCommand.NotifyCanExecuteChanged();
  }

  private void DetachScheduleHandlers()
  {
    foreach (EmployeeWeeklyScheduleViewModel schedule in EmployeeSchedules)
    {
      schedule.PropertyChanged -= OnEmployeeSchedulePropertyChanged;
    }
  }

  private void OnEmployeeSchedulePropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(EmployeeWeeklyScheduleViewModel.HasPendingChanges))
    {
      RefreshPendingChangesState();
    }
  }

  partial void OnIsLoadingSchedulesChanged(bool value)
  {
    UpdateSchedulesCommand.NotifyCanExecuteChanged();
  }


  private async Task PersistImportedScheduleAsync(ScheduleCsvRecord record)
  {
    if (record.ShiftId == Guid.Empty)
    {
      if (record.ScheduleId.HasValue && record.ScheduleId.Value != Guid.Empty)
      {
        await scheduleApiService.DeleteScheduleAsync(record.ScheduleId.Value).ConfigureAwait(false);
      }

      return;
    }

    var summary = new ScheduleSummary
    {
      Id = record.ScheduleId ?? Guid.Empty,
      EmployeeId = record.EmployeeId,
      ShiftId = record.ShiftId,
      ScheduleDate = record.ScheduleDate,
      IsDefault = record.IsDefault,
      Name = record.ShiftName ?? string.Empty
    };

    if (summary.Id == Guid.Empty)
    {
      Guid? createdId = await scheduleApiService.CreateScheduleAsync(summary).ConfigureAwait(false);
      if (createdId.HasValue)
      {
        summary.Id = createdId.Value;
      }
    }
    else
    {
      ScheduleSummary? updated = await scheduleApiService.UpdateScheduleAsync(summary).ConfigureAwait(false);
      if (updated is not null)
      {
        summary = updated;
      }
    }
  }

  private ShiftOption EnsureNoneOption()
  {
    if (shiftOptionLookup.TryGetValue(Guid.Empty, out ShiftOption? existing))
    {
      return existing;
    }

    var option = new ShiftOption(Guid.Empty, "Unassigned");
    return AddShiftOption(option, insertAtStart: true);
  }

  private ShiftOption AddShiftOption(ShiftOption option, bool insertAtStart = false)
  {
    if (shiftOptionLookup.TryGetValue(option.Id, out ShiftOption? existing))
    {
      return existing;
    }

    shiftOptionLookup[option.Id] = option;

    if (insertAtStart)
    {
      ShiftOptions.Insert(0, option);
    }
    else
    {
      ShiftOptions.Add(option);
    }

    return option;
  }

  private ShiftOption AddShiftOption(ShiftOption option)
  {
    return AddShiftOption(option, insertAtStart: false);
  }

  private void UpdateWeekDays()
  {
    WeekDays.Clear();

    DateOnly current = WeekStart;
    for (int i = 0; i < 7; i++)
    {
      WeekDays.Add(new WeekDayHeaderViewModel(current));
      current = current.AddDays(1);
    }
  }

  private static string? FormatTimeRange(ShiftSummary shift)
  {
    return FormatTimeRange(ParseTime(shift.ShiftStartTime), ParseTime(shift.ShiftEndTime));
  }

  private static string? FormatTimeRange(TimeOnly? start, TimeOnly? end)
  {
    if (start.HasValue && end.HasValue)
    {
      return $"{start.Value:HH:mm}-{end.Value:HH:mm}";
    }

    if (start.HasValue)
    {
      return start.Value.ToString("HH:mm", CultureInfo.InvariantCulture);
    }

    if (end.HasValue)
    {
      return end.Value.ToString("HH:mm", CultureInfo.InvariantCulture);
    }

    return null;
  }

  private static TimeOnly? ParseTime(string? value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      return null;
    }

    if (TimeOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out TimeOnly invariantResult))
    {
      return invariantResult;
    }

    if (TimeOnly.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out TimeOnly cultureResult))
    {
      return cultureResult;
    }

    if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out TimeSpan span))
    {
      return TimeOnly.FromTimeSpan(span);
    }

    return null;
  }

  private static DateOnly GetStartOfWeek(DateOnly date, DayOfWeek startOfWeek = DayOfWeek.Monday)
  {
    int diff = (7 + (date.DayOfWeek - startOfWeek)) % 7;
    return date.AddDays(-diff);
  }
}

public sealed class ShiftOption : IEquatable<ShiftOption>
{
  public ShiftOption(Guid id, string name, string? details = null)
  {
    Id = id;
    Name = name;
    Details = details;
  }

  public Guid Id { get; }

  public string Name { get; }

  public string? Details { get; }

  public string DisplayName => string.IsNullOrWhiteSpace(Details) ? Name : $"{Name} ({Details})";

  public override string ToString() => DisplayName;

  public bool Equals(ShiftOption? other)
  {
    if (ReferenceEquals(null, other))
    {
      return false;
    }

    if (ReferenceEquals(this, other))
    {
      return true;
    }

    return Id == other.Id;
  }

  public override bool Equals(object? obj) => Equals(obj as ShiftOption);

  public override int GetHashCode() => Id.GetHashCode();

  public static bool operator ==(ShiftOption? left, ShiftOption? right)
  {
    if (ReferenceEquals(left, right))
    {
      return true;
    }

    if (left is null || right is null)
    {
      return false;
    }

    return left.Equals(right);
  }

  public static bool operator !=(ShiftOption? left, ShiftOption? right) => !(left == right);
}

public partial class EmployeeWeeklyScheduleViewModel : ObservableObject
{
  public EmployeeWeeklyScheduleViewModel(
    ScheduleViewModel parent,
    EmployeeScheduleSummary summary,
    DateOnly weekStart,
    DateOnly weekEnd)
  {
    EmployeeId = summary.EmployeeId;
    Barcode = summary.Barcode;
    Name = summary.Name;
    Section = summary.Section;
    Charging = summary.Charging;

    Days = new ObservableCollection<EmployeeScheduleDayViewModel>();

    Dictionary<DateOnly, ScheduleShiftInfoSummary> lookup = summary.Shifts?
      .Where(shift => shift is not null)
      .ToDictionary(shift => shift!.ScheduleDate, shift => shift!, DateOnlyComparer.Instance)
      ?? new Dictionary<DateOnly, ScheduleShiftInfoSummary>(DateOnlyComparer.Instance);

    for (DateOnly date = weekStart; date <= weekEnd; date = date.AddDays(1))
    {
      lookup.TryGetValue(date, out ScheduleShiftInfoSummary? info);
      ShiftOption option = parent.GetShiftOption(info?.Shift?.Id, info?.Shift);
      var day = new EmployeeScheduleDayViewModel(parent, EmployeeId, date);
      day.Initialize(info?.ScheduleId ?? Guid.Empty, info?.IsDefault ?? false, option);
      day.PropertyChanged += OnDayPropertyChanged;
      Days.Add(day);
    }
  }

  public Guid EmployeeId { get; }

  public string Barcode { get; }

  public string Name { get; }

  public string Section { get; }

  public string Charging { get; }

  public ObservableCollection<EmployeeScheduleDayViewModel> Days { get; }

  public bool HasPendingChanges => Days.Any(day => day.HasPendingChanges);

  public bool HasSection => !string.IsNullOrWhiteSpace(Section);

  public bool HasBarcode => !string.IsNullOrWhiteSpace(Barcode);

  public string BarcodeDisplay => Barcode;

  private void OnDayPropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(EmployeeScheduleDayViewModel.HasPendingChanges))
    {
      OnPropertyChanged(nameof(HasPendingChanges));
    }
  }
}

public partial class EmployeeScheduleDayViewModel : ObservableObject
{
  private readonly ScheduleViewModel parent;
  private bool suppressSelectionChanged;

  public EmployeeScheduleDayViewModel(ScheduleViewModel parent, Guid employeeId, DateOnly date)
  {
    this.parent = parent;
    EmployeeId = employeeId;
    Date = date;
  }

  public Guid EmployeeId { get; }

  public DateOnly Date { get; }

  [ObservableProperty]
  public partial Guid ScheduleId { get; set; }

  [ObservableProperty]
  public partial bool IsDefault { get; set; }

  [ObservableProperty]
  [NotifyPropertyChangedFor(nameof(HasAssignedShift))]
  [NotifyPropertyChangedFor(nameof(HasPendingChanges))]
  public partial ShiftOption? SelectedShift { get; set; }

  [ObservableProperty]
  [NotifyPropertyChangedFor(nameof(IsEnabled))]
  public partial bool IsProcessing { get; set; }

  public ShiftOption? CurrentCommittedShift { get; private set; }

  public bool HasAssignedShift => SelectedShift is not null && SelectedShift.Id != Guid.Empty;

  public bool IsEnabled => !IsProcessing;

  public bool HasPendingChanges
  {
    get
    {
      Guid currentId = CurrentCommittedShift?.Id ?? Guid.Empty;
      Guid selectedId = SelectedShift?.Id ?? Guid.Empty;
      return currentId != selectedId;
    }
  }

  public void Initialize(Guid scheduleId, bool isDefault, ShiftOption option)
  {
    ScheduleId = scheduleId;
    IsDefault = isDefault;
    SetSelectionSilently(option);
    CurrentCommittedShift = option;
  }

  public void UpdateScheduleState(Guid scheduleId, bool isDefault)
  {
    ScheduleId = scheduleId;
    IsDefault = isDefault;
  }

  public void SetCommittedSelection(ShiftOption option)
  {
    CurrentCommittedShift = option;
    OnPropertyChanged(nameof(HasPendingChanges));
  }

  public void RestoreCommittedSelection()
  {
    ShiftOption fallback = CurrentCommittedShift ?? parent.GetShiftOption(Guid.Empty, null);
    SetSelectionSilently(fallback);
  }

  public void SetSelectionSilently(ShiftOption? option)
  {
    ShiftOption target = option ?? parent.GetShiftOption(Guid.Empty, null);
    suppressSelectionChanged = true;
    SelectedShift = target;
    suppressSelectionChanged = false;
  }

  partial void OnSelectedShiftChanged(ShiftOption? value)
  {
    if (suppressSelectionChanged)
    {
      return;
    }

    parent.NotifyDaySelectionChanged();
  }
}

public sealed class WeekDayHeaderViewModel
{
  public WeekDayHeaderViewModel(DateOnly date)
  {
    Date = date;
    DayName = date.ToString("ddd", CultureInfo.InvariantCulture);
    DateDisplay = date.ToString("MMM d", CultureInfo.InvariantCulture);
  }

  public DateOnly Date { get; }

  public string DayName { get; }

  public string DateDisplay { get; }
}

internal sealed class DateOnlyComparer : IEqualityComparer<DateOnly>
{
  public static DateOnlyComparer Instance { get; } = new();

  public bool Equals(DateOnly x, DateOnly y) => x == y;

  public int GetHashCode(DateOnly obj)
  {
    // Avoid converting to DateTime which can throw for extreme DateOnly values.
    return HashCode.Combine(obj.Year, obj.Month, obj.Day);
  }
}


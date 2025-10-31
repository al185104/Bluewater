using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bluewater.App.Helpers;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;

namespace Bluewater.App.ViewModels;

public partial class ScheduleViewModel : BaseViewModel
{
  private readonly IScheduleApiService scheduleApiService;
  private readonly IShiftApiService shiftApiService;
  private readonly IReferenceDataService referenceDataService;

  private readonly List<DailyShiftSelection> trackedDays = new();
  private readonly Dictionary<Guid, ShiftPickerItem> shiftLookup = new();
  private readonly ShiftPickerItem noShiftOption = ShiftPickerItem.CreateNone();

  private bool hasInitialized;
  private bool shiftsLoaded;
  private bool suppressSelectedChargingChanged;

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

    ShiftOptions.Add(noShiftOption);
  }

  public ObservableCollection<WeeklyEmployeeSchedule> Employees { get; } = new();

  public ObservableCollection<ShiftPickerItem> ShiftOptions { get; } = new();

  public ObservableCollection<ChargingSummary> Chargings { get; } = new();

  public IReadOnlyList<TenantDto> TenantOptions { get; } = Array.AsReadOnly(Enum.GetValues<TenantDto>());

  [ObservableProperty]
  public partial ChargingSummary? SelectedCharging { get; set; }

  [ObservableProperty]
  public partial TenantDto SelectedTenant { get; set; } = TenantDto.Maribago;

  [ObservableProperty]
  public partial DateOnly CurrentWeekStart { get; set; }

  [ObservableProperty]
  public partial DateOnly CurrentWeekEnd { get; set; }

  [ObservableProperty]
  public partial bool HasPendingChanges { get; set; }

  [ObservableProperty]
  public partial string ImportStatusMessage { get; set; } = string.Empty;

  [ObservableProperty]
  public partial bool HasImportStatusMessage { get; set; }

  public bool IsNotBusy => !IsBusy;

  public string WeekRangeDisplay => $"{CurrentWeekStart:MMM dd} - {CurrentWeekEnd:MMM dd}";

  public string SundayHeader => FormatHeader(0);
  public string MondayHeader => FormatHeader(1);
  public string TuesdayHeader => FormatHeader(2);
  public string WednesdayHeader => FormatHeader(3);
  public string ThursdayHeader => FormatHeader(4);
  public string FridayHeader => FormatHeader(5);
  public string SaturdayHeader => FormatHeader(6);

  public override async Task InitializeAsync()
  {
    if (!hasInitialized)
    {
      try
      {
        IsBusy = true;

        SetWeek(DateOnly.FromDateTime(DateTime.Today));
        LoadChargings();
        await EnsureShiftOptionsAsync().ConfigureAwait(false);

        if (SelectedCharging is not null && Chargings.All(c => c.Id != SelectedCharging.Id))
        {
          suppressSelectedChargingChanged = true;
          SelectedCharging = null;
          suppressSelectedChargingChanged = false;
        }

        if (SelectedCharging is null && Chargings.Count > 0)
        {
          suppressSelectedChargingChanged = true;
          SelectedCharging = Chargings[0];
          suppressSelectedChargingChanged = false;
        }

        hasInitialized = true;
      }
      catch (Exception ex)
      {
        ExceptionHandlingService.Handle(ex, "Initializing schedules");
      }
      finally
      {
        IsBusy = false;
      }
    }

    if (!HasPendingChanges)
    {
      await LoadSchedulesAsync().ConfigureAwait(false);
    }
  }

  [RelayCommand(CanExecute = nameof(CanChangeWeek))]
  private async Task PreviousWeekAsync()
  {
    SetWeek(CurrentWeekStart.AddDays(-7));
    await LoadSchedulesAsync().ConfigureAwait(false);
  }

  [RelayCommand(CanExecute = nameof(CanChangeWeek))]
  private async Task NextWeekAsync()
  {
    SetWeek(CurrentWeekStart.AddDays(7));
    await LoadSchedulesAsync().ConfigureAwait(false);
  }

  [RelayCommand(CanExecute = nameof(CanSaveChanges))]
  private async Task SaveChangesAsync()
  {
    IReadOnlyList<DailyShiftSelection> dirtyDays = trackedDays.Where(day => day.IsDirty).ToList();

    if (dirtyDays.Count == 0)
    {
      UpdateHasPendingChanges();
      return;
    }

    List<Exception> errors = new();
    int successCount = 0;

    try
    {
      IsBusy = true;

      foreach (DailyShiftSelection day in dirtyDays)
      {
        try
        {
          bool result = await PersistDayAsync(day).ConfigureAwait(false);
          if (result)
          {
            successCount++;
          }
        }
        catch (Exception ex)
        {
          errors.Add(ex);
        }
      }
    }
    finally
    {
      IsBusy = false;
    }

    if (successCount > 0)
    {
      await LoadSchedulesAsync().ConfigureAwait(false);
      await TraceCommandAsync(nameof(SaveChangesAsync), new
      {
        Changes = successCount,
        WeekStart = CurrentWeekStart,
        WeekEnd = CurrentWeekEnd,
        Charging = SelectedCharging?.Name,
        Tenant = SelectedTenant.ToString()
      }).ConfigureAwait(false);
    }

    if (errors.Count > 0)
    {
      Exception exception = errors.Count == 1 ? errors[0] : new AggregateException(errors);
      ExceptionHandlingService.Handle(exception, "Saving schedules");
    }

    UpdateHasPendingChanges();
  }

  [RelayCommand(CanExecute = nameof(CanImportSchedules))]
  private async Task ImportSchedulesAsync()
  {
    if (IsBusy)
    {
      return;
    }

    ImportStatusMessage = string.Empty;

    bool importedAny = false;
    int successCount = 0;
    int total = 0;
    List<Exception> errors = new();

    try
    {
      PickOptions options = new()
      {
        PickerTitle = "Select schedule CSV file",
        FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
          [DevicePlatform.iOS] = new[] { "public.comma-separated-values-text", "public.text" },
          [DevicePlatform.Android] = new[] { "text/csv", "text/comma-separated-values" },
          [DevicePlatform.WinUI] = new[] { ".csv" },
          [DevicePlatform.MacCatalyst] = new[] { "public.comma-separated-values-text", "public.text" }
        })
      };

      FileResult? file = await FilePicker.Default.PickAsync(options);

      if (file is null)
      {
        return;
      }

      await using Stream stream = await file.OpenReadAsync();
      IReadOnlyList<ScheduleCsvRecord> records = await ScheduleCsvImporter.ParseAsync(stream).ConfigureAwait(false);
      total = records.Count;

      if (total == 0)
      {
        ImportStatusMessage = "No schedules were found in the selected file.";
        return;
      }

      IsBusy = true;

      foreach (ScheduleCsvRecord record in records)
      {
        try
        {
          bool result = await PersistImportedRecordAsync(record).ConfigureAwait(false);
          if (result)
          {
            successCount++;
          }
        }
        catch (Exception ex)
        {
          errors.Add(ex);
        }
      }

      importedAny = successCount > 0;

      ImportStatusMessage = successCount switch
      {
        0 => "No schedules were imported.",
        1 when total == 1 => "Imported 1 schedule successfully.",
        _ when successCount == total => $"Imported {successCount} schedules successfully.",
        _ => $"Imported {successCount} of {total} schedules successfully."
      };

      await TraceCommandAsync(nameof(ImportSchedulesAsync), new
      {
        File = file.FileName,
        Success = successCount,
        Total = total
      }).ConfigureAwait(false);

      if (errors.Count > 0)
      {
        Exception exception = errors.Count == 1 ? errors[0] : new AggregateException(errors);
        ExceptionHandlingService.Handle(exception, "Importing schedules");
      }
    }
    catch (OperationCanceledException)
    {
      // The user cancelled the picker; no action needed.
    }
    catch (FormatException ex)
    {
      ImportStatusMessage = ex.Message;
      ExceptionHandlingService.Handle(ex, "Importing schedules");
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Importing schedules");
    }
    finally
    {
      if (IsBusy)
      {
        IsBusy = false;
      }
    }

    if (importedAny)
    {
      await LoadSchedulesAsync().ConfigureAwait(false);
    }
  }

  private bool CanChangeWeek() => !IsBusy;

  private bool CanSaveChanges() => !IsBusy && HasPendingChanges;

  private bool CanImportSchedules() => !IsBusy;

  partial void OnSelectedChargingChanged(ChargingSummary? value)
  {
    if (suppressSelectedChargingChanged)
    {
      return;
    }

    if (!hasInitialized)
    {
      return;
    }

    if (value is null)
    {
      DetachDayHandlers();
      Employees.Clear();
      UpdateHasPendingChanges();
      return;
    }

    ImportStatusMessage = string.Empty;
    _ = LoadSchedulesAsync();
  }

  partial void OnSelectedTenantChanged(TenantDto value)
  {
    if (!hasInitialized)
    {
      return;
    }

    _ = LoadSchedulesAsync();
  }

  partial void OnCurrentWeekStartChanged(DateOnly value)
  {
    RaiseWeekHeaderProperties();
  }

  partial void OnCurrentWeekEndChanged(DateOnly value)
  {
    RaiseWeekHeaderProperties();
  }

  partial void OnHasPendingChangesChanged(bool value)
  {
    SaveChangesCommand.NotifyCanExecuteChanged();
    PreviousWeekCommand.NotifyCanExecuteChanged();
    NextWeekCommand.NotifyCanExecuteChanged();
  }

  partial void OnImportStatusMessageChanged(string value)
  {
    HasImportStatusMessage = !string.IsNullOrWhiteSpace(value);
  }

  partial void OnIsBusyChanged(bool value)
  {
    OnPropertyChanged(nameof(IsNotBusy));
    SaveChangesCommand.NotifyCanExecuteChanged();
    PreviousWeekCommand.NotifyCanExecuteChanged();
    NextWeekCommand.NotifyCanExecuteChanged();
    ImportSchedulesCommand.NotifyCanExecuteChanged();
  }

  private async Task LoadSchedulesAsync()
  {
    if (SelectedCharging is null)
    {
      DetachDayHandlers();
      Employees.Clear();
      UpdateHasPendingChanges();
      return;
    }

    try
    {
      IsBusy = true;

      await EnsureShiftOptionsAsync().ConfigureAwait(false);

      IReadOnlyList<EmployeeScheduleSummary> schedules = await scheduleApiService
        .GetSchedulesAsync(
          SelectedCharging.Name,
          CurrentWeekStart,
          CurrentWeekEnd,
          tenant: SelectedTenant)
        .ConfigureAwait(false);

      DetachDayHandlers();
      Employees.Clear();

      foreach (EmployeeScheduleSummary employee in schedules.OrderBy(emp => emp.Name, StringComparer.OrdinalIgnoreCase))
      {
        List<DailyShiftSelection> days = new(7);

        for (int offset = 0; offset < 7; offset++)
        {
          DateOnly date = CurrentWeekStart.AddDays(offset);
          ScheduleShiftInfoSummary? shiftInfo = employee.Shifts.FirstOrDefault(info => info.ScheduleDate == date);

          Guid? scheduleId = shiftInfo?.ScheduleId;
          if (scheduleId == Guid.Empty)
          {
            scheduleId = null;
          }

          ShiftPickerItem shiftItem = shiftInfo?.Shift is null
            ? noShiftOption
            : ResolveShiftOption(shiftInfo.Shift);

          bool isDefault = shiftInfo?.IsDefault ?? false;

          var day = new DailyShiftSelection(employee.EmployeeId, date, scheduleId, isDefault, shiftItem);
          days.Add(day);
        }

        var weeklySchedule = new WeeklyEmployeeSchedule(
          employee.EmployeeId,
          employee.Barcode,
          employee.Name,
          employee.Section,
          employee.Charging,
          days);

        AttachDayHandlers(weeklySchedule);
        Employees.Add(weeklySchedule);
      }

      UpdateHasPendingChanges();

      await TraceCommandAsync(nameof(LoadSchedulesAsync), new
      {
        WeekStart = CurrentWeekStart,
        WeekEnd = CurrentWeekEnd,
        Charging = SelectedCharging.Name,
        Tenant = SelectedTenant.ToString(),
        EmployeeCount = Employees.Count
      }).ConfigureAwait(false);
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

  private async Task<bool> PersistDayAsync(DailyShiftSelection day)
  {
    Guid? selectedShiftId = day.SelectedShiftId;
    bool hasShift = selectedShiftId.HasValue && selectedShiftId.Value != Guid.Empty;

    Guid? scheduleId = day.ScheduleId;
    bool hasSchedule = scheduleId.HasValue && scheduleId.Value != Guid.Empty;

    if (hasSchedule)
    {
      if (!hasShift)
      {
        if (day.IsDefault)
        {
          day.RevertToOriginal();
          return false;
        }

        bool deleted = await scheduleApiService.DeleteScheduleAsync(scheduleId!.Value).ConfigureAwait(false);

        if (deleted)
        {
          day.AcceptChanges(null, false);
        }

        return deleted;
      }

      if (day.IsDefault)
      {
        Guid? createdId = await scheduleApiService.CreateScheduleAsync(new ScheduleSummary
        {
          EmployeeId = day.EmployeeId,
          ShiftId = selectedShiftId!.Value,
          ScheduleDate = day.Date,
          IsDefault = false
        }).ConfigureAwait(false);

        if (createdId.HasValue)
        {
          day.AcceptChanges(createdId, false);
          return true;
        }

        return false;
      }

      ScheduleSummary summary = new()
      {
        Id = scheduleId!.Value,
        EmployeeId = day.EmployeeId,
        ShiftId = selectedShiftId!.Value,
        ScheduleDate = day.Date,
        IsDefault = false
      };

      ScheduleSummary? updated = await scheduleApiService.UpdateScheduleAsync(summary).ConfigureAwait(false);

      if (updated is not null)
      {
        if (updated.Shift is not null)
        {
          ResolveShiftOption(updated.Shift);
        }

        day.AcceptChanges(updated.Id, updated.IsDefault);
        return true;
      }

      return false;
    }

    if (!hasShift)
    {
      day.AcceptChanges(null, day.IsDefault);
      return false;
    }

    Guid? newScheduleId = await scheduleApiService.CreateScheduleAsync(new ScheduleSummary
    {
      EmployeeId = day.EmployeeId,
      ShiftId = selectedShiftId!.Value,
      ScheduleDate = day.Date,
      IsDefault = false
    }).ConfigureAwait(false);

    if (newScheduleId.HasValue)
    {
      day.AcceptChanges(newScheduleId, false);
      return true;
    }

    return false;
  }

  private async Task<bool> PersistImportedRecordAsync(ScheduleCsvRecord record)
  {
    Guid shiftId = record.ShiftId;
    bool hasShift = shiftId != Guid.Empty;

    if (record.ScheduleId.HasValue && record.ScheduleId.Value != Guid.Empty)
    {
      if (!hasShift)
      {
        return await scheduleApiService.DeleteScheduleAsync(record.ScheduleId.Value).ConfigureAwait(false);
      }

      ScheduleSummary updateSummary = new()
      {
        Id = record.ScheduleId.Value,
        EmployeeId = record.EmployeeId,
        ShiftId = shiftId,
        ScheduleDate = record.ScheduleDate,
        IsDefault = record.IsDefault
      };

      ScheduleSummary? updated = await scheduleApiService.UpdateScheduleAsync(updateSummary).ConfigureAwait(false);
      return updated is not null;
    }

    if (!hasShift)
    {
      return false;
    }

    Guid? scheduleId = await scheduleApiService.CreateScheduleAsync(new ScheduleSummary
    {
      EmployeeId = record.EmployeeId,
      ShiftId = shiftId,
      ScheduleDate = record.ScheduleDate,
      IsDefault = record.IsDefault
    }).ConfigureAwait(false);

    return scheduleId.HasValue;
  }

  private void LoadChargings()
  {
    suppressSelectedChargingChanged = true;

    Guid? previousId = SelectedCharging?.Id;

    Chargings.Clear();

    foreach (ChargingSummary charging in referenceDataService.Chargings
      .OrderBy(charging => charging.Name, StringComparer.OrdinalIgnoreCase))
    {
      Chargings.Add(charging);
    }

    if (previousId.HasValue)
    {
      ChargingSummary? existing = Chargings.FirstOrDefault(charging => charging.Id == previousId.Value);
      if (existing is not null)
      {
        SelectedCharging = existing;
      }
    }

    if (SelectedCharging is null && Chargings.Count > 0)
    {
      SelectedCharging = Chargings[0];
    }

    suppressSelectedChargingChanged = false;
  }

  private async Task EnsureShiftOptionsAsync()
  {
    if (shiftsLoaded)
    {
      return;
    }

    try
    {
      IReadOnlyList<ShiftSummary> shifts = await shiftApiService.GetShiftsAsync().ConfigureAwait(false);

      ShiftOptions.Clear();
      ShiftOptions.Add(noShiftOption);
      shiftLookup.Clear();

      foreach (ShiftSummary shift in shifts
        .Where(shift => shift.Id != Guid.Empty)
        .OrderBy(shift => shift.Name, StringComparer.OrdinalIgnoreCase))
      {
        ShiftPickerItem option = CreateShiftOption(shift);
        InsertShiftOption(option);
      }

      shiftsLoaded = true;
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Loading shifts");
    }
  }

  private void AttachDayHandlers(WeeklyEmployeeSchedule schedule)
  {
    foreach (DailyShiftSelection day in schedule.Days)
    {
      day.PropertyChanged += OnDayPropertyChanged;
      trackedDays.Add(day);
    }
  }

  private void DetachDayHandlers()
  {
    foreach (DailyShiftSelection day in trackedDays)
    {
      day.PropertyChanged -= OnDayPropertyChanged;
    }

    trackedDays.Clear();
  }

  private void OnDayPropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(DailyShiftSelection.IsDirty))
    {
      UpdateHasPendingChanges();
    }
  }

  private void UpdateHasPendingChanges()
  {
    HasPendingChanges = trackedDays.Any(day => day.IsDirty);
  }

  private void SetWeek(DateOnly referenceDate)
  {
    DateOnly start = GetStartOfWeek(referenceDate);
    CurrentWeekStart = start;
    CurrentWeekEnd = start.AddDays(6);
  }

  private static DateOnly GetStartOfWeek(DateOnly date)
  {
    int difference = (int)date.DayOfWeek;
    return date.AddDays(-difference);
  }

  private string FormatHeader(int offset)
  {
    DateOnly date = CurrentWeekStart.AddDays(offset);
    return date.ToString("ddd MMM d", CultureInfo.CurrentCulture);
  }

  private void RaiseWeekHeaderProperties()
  {
    OnPropertyChanged(nameof(WeekRangeDisplay));
    OnPropertyChanged(nameof(SundayHeader));
    OnPropertyChanged(nameof(MondayHeader));
    OnPropertyChanged(nameof(TuesdayHeader));
    OnPropertyChanged(nameof(WednesdayHeader));
    OnPropertyChanged(nameof(ThursdayHeader));
    OnPropertyChanged(nameof(FridayHeader));
    OnPropertyChanged(nameof(SaturdayHeader));
  }

  private ShiftPickerItem ResolveShiftOption(ScheduleShiftDetailsSummary shift)
  {
    if (shift.Id == Guid.Empty)
    {
      return noShiftOption;
    }

    if (shiftLookup.TryGetValue(shift.Id, out ShiftPickerItem? existing))
    {
      return existing;
    }

    ShiftPickerItem option = new(
      shift.Id,
      shift.Name,
      BuildShiftDisplay(shift.Name, shift.ShiftStartTime, shift.ShiftEndTime),
      BuildShiftDescription(shift.ShiftStartTime, shift.ShiftBreakTime, shift.ShiftBreakEndTime, shift.ShiftEndTime, shift.BreakHours));

    InsertShiftOption(option);
    return option;
  }

  private void InsertShiftOption(ShiftPickerItem option)
  {
    if (option.Id.HasValue && shiftLookup.ContainsKey(option.Id.Value))
    {
      return;
    }

    int insertIndex = 1;

    while (insertIndex < ShiftOptions.Count && string.Compare(ShiftOptions[insertIndex].DisplayName, option.DisplayName, StringComparison.CurrentCultureIgnoreCase) < 0)
    {
      insertIndex++;
    }

    ShiftOptions.Insert(insertIndex, option);

    if (option.Id.HasValue)
    {
      shiftLookup[option.Id.Value] = option;
    }
  }

  private ShiftPickerItem CreateShiftOption(ShiftSummary summary)
  {
    TimeOnly? start = ParseTime(summary.ShiftStartTime);
    TimeOnly? breakStart = ParseTime(summary.ShiftBreakTime);
    TimeOnly? breakEnd = ParseTime(summary.ShiftBreakEndTime);
    TimeOnly? end = ParseTime(summary.ShiftEndTime);

    string display = BuildShiftDisplay(summary.Name, start, end);
    string description = BuildShiftDescription(start, breakStart, breakEnd, end, summary.BreakHours);

    return new ShiftPickerItem(summary.Id, summary.Name, display, description);
  }

  private static TimeOnly? ParseTime(string? value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      return null;
    }

    if (TimeOnly.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out TimeOnly parsed) ||
        TimeOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
    {
      return parsed;
    }

    if (TimeSpan.TryParse(value, CultureInfo.CurrentCulture, out TimeSpan span) ||
        TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out span))
    {
      return TimeOnly.FromTimeSpan(span);
    }

    return null;
  }

  private static string BuildShiftDisplay(string name, TimeOnly? start, TimeOnly? end)
  {
    string startText = FormatTime(start);
    string endText = FormatTime(end);

    if (!string.IsNullOrEmpty(startText) && !string.IsNullOrEmpty(endText))
    {
      return $"{name} ({startText} - {endText})";
    }

    return name;
  }

  private static string BuildShiftDescription(TimeOnly? start, TimeOnly? breakStart, TimeOnly? breakEnd, TimeOnly? end, decimal breakHours)
  {
    List<string> segments = new();

    string startText = FormatTime(start);
    string breakStartText = FormatTime(breakStart);
    string breakEndText = FormatTime(breakEnd);
    string endText = FormatTime(end);

    if (!string.IsNullOrEmpty(startText) && !string.IsNullOrEmpty(breakStartText))
    {
      segments.Add($"{startText} - {breakStartText}");
    }

    if (!string.IsNullOrEmpty(breakEndText) && !string.IsNullOrEmpty(endText))
    {
      segments.Add($"{breakEndText} - {endText}");
    }

    if (segments.Count == 0 && !string.IsNullOrEmpty(startText) && !string.IsNullOrEmpty(endText))
    {
      segments.Add($"{startText} - {endText}");
    }

    if (breakHours > 0)
    {
      string breakText = breakHours == 1 ? "1 hour break" : $"{breakHours:0.##} hour break";
      segments.Add(breakText);
    }

    return string.Join(" • ", segments);
  }

  private static string FormatTime(TimeOnly? time)
  {
    return time?.ToString("hh:mm tt", CultureInfo.CurrentCulture) ?? string.Empty;
  }
}

public sealed class WeeklyEmployeeSchedule
{
  public WeeklyEmployeeSchedule(
    Guid employeeId,
    string barcode,
    string name,
    string section,
    string charging,
    IReadOnlyList<DailyShiftSelection> days)
  {
    if (days.Count != 7)
    {
      throw new ArgumentException("Seven day entries are required.", nameof(days));
    }

    EmployeeId = employeeId;
    Barcode = barcode;
    Name = name;
    Section = section;
    Charging = charging;
    Days = days;

    Sunday = days[0];
    Monday = days[1];
    Tuesday = days[2];
    Wednesday = days[3];
    Thursday = days[4];
    Friday = days[5];
    Saturday = days[6];
  }

  public Guid EmployeeId { get; }
  public string Barcode { get; }
  public string Name { get; }
  public string Section { get; }
  public string Charging { get; }

  public IReadOnlyList<DailyShiftSelection> Days { get; }

  public DailyShiftSelection Sunday { get; }
  public DailyShiftSelection Monday { get; }
  public DailyShiftSelection Tuesday { get; }
  public DailyShiftSelection Wednesday { get; }
  public DailyShiftSelection Thursday { get; }
  public DailyShiftSelection Friday { get; }
  public DailyShiftSelection Saturday { get; }
}

public partial class DailyShiftSelection : ObservableObject
{
  private Guid? originalShiftId;
  private ShiftPickerItem? originalShiftItem;
  private bool suppressSelectionChanged;

  public DailyShiftSelection(
    Guid employeeId,
    DateOnly date,
    Guid? scheduleId,
    bool isDefault,
    ShiftPickerItem? selectedShift)
  {
    EmployeeId = employeeId;
    Date = date;
    this.scheduleId = scheduleId;
    this.isDefault = isDefault;

    ShiftPickerItem initialShift = selectedShift ?? ShiftPickerItem.CreateNone();
    originalShiftItem = initialShift;
    originalShiftId = initialShift?.Id;
    isDirty = false;

    ApplyInitialSelection(initialShift);
  }

  public Guid EmployeeId { get; }

  public DateOnly Date { get; }

  [ObservableProperty]
  private Guid? scheduleId;

  [ObservableProperty]
  private bool isDefault;

  [ObservableProperty]
  private ShiftPickerItem? selectedShift;

  [ObservableProperty]
  private bool isDirty;

  public Guid? SelectedShiftId => SelectedShift?.Id;

  public ShiftPickerItem? OriginalShift => originalShiftItem;

  private void ApplyInitialSelection(ShiftPickerItem initialShift)
  {
    if (MainThread.IsMainThread)
    {
      SetSelectedShift(initialShift);
    }
    else
    {
      MainThread
        .InvokeOnMainThreadAsync(() => SetSelectedShift(initialShift))
        .GetAwaiter()
        .GetResult();
    }
  }

  private void SetSelectedShift(ShiftPickerItem initialShift)
  {
    suppressSelectionChanged = true;
    SelectedShift = initialShift;
    suppressSelectionChanged = false;
  }

  partial void OnSelectedShiftChanged(ShiftPickerItem? value)
  {
    if (suppressSelectionChanged)
    {
      return;
    }

    Guid? newId = value?.Id;
    IsDirty = !NullableEquals(newId, originalShiftId);
  }

  public void AcceptChanges(Guid? newScheduleId, bool isDefault)
  {
    suppressSelectionChanged = true;
    ScheduleId = newScheduleId;
    IsDefault = isDefault;
    suppressSelectionChanged = false;

    originalShiftItem = SelectedShift;
    originalShiftId = SelectedShift?.Id;
    IsDirty = false;
  }

  public void RevertToOriginal()
  {
    suppressSelectionChanged = true;
    SelectedShift = originalShiftItem;
    suppressSelectionChanged = false;
    IsDirty = false;
  }

  private static bool NullableEquals(Guid? left, Guid? right)
  {
    return left.GetValueOrDefault() == right.GetValueOrDefault() && left.HasValue == right.HasValue;
  }
}

public sealed class ShiftPickerItem
{
  public ShiftPickerItem(Guid? id, string name, string displayName, string? description)
  {
    Id = id;
    Name = name;
    DisplayName = displayName;
    Description = string.IsNullOrWhiteSpace(description) ? null : description;
  }

  private ShiftPickerItem()
  {
    Id = null;
    Name = "No Shift";
    DisplayName = "No Shift";
    Description = "No shift scheduled.";
  }

  public Guid? Id { get; }

  public string Name { get; }

  public string DisplayName { get; }

  public string? Description { get; }

  public bool IsNone => !Id.HasValue;

  public static ShiftPickerItem CreateNone() => new();

  public override string ToString() => DisplayName;
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;

namespace Bluewater.App.ViewModels;

public partial class ScheduleViewModel : BaseViewModel
{
  private readonly IScheduleApiService scheduleApiService;
  private readonly IShiftApiService shiftApiService;
  private readonly IReferenceDataService referenceDataService;

  private readonly Dictionary<Guid, ShiftPickerItem> shiftLookup = new();
  private readonly ShiftPickerItem noShiftOption = ShiftPickerItem.CreateNone();
  private readonly Dictionary<DailyShiftSelection, ShiftPickerItem> previousSelections = new();

  private bool hasInitialized;
  private bool shiftsLoaded;
  private bool suppressSelectedChargingChanged;
  private bool suppressSelectionChange;

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

  [ObservableProperty] public partial ChargingSummary? SelectedCharging { get; set; }
  [ObservableProperty] public partial TenantDto SelectedTenant { get; set; } = TenantDto.Maribago;
  [ObservableProperty] public partial DateOnly CurrentWeekStart { get; set; }
  [ObservableProperty] public partial DateOnly CurrentWeekEnd { get; set; }
  [ObservableProperty] public partial bool IsSaving { get; set; }

  public string WeekRangeDisplay => $"{CurrentWeekStart:MMM dd} - {CurrentWeekEnd:MMM dd}";
  public string SundayHeader => FormatHeader(0);
  public string MondayHeader => FormatHeader(1);
  public string TuesdayHeader => FormatHeader(2);
  public string WednesdayHeader => FormatHeader(3);
  public string ThursdayHeader => FormatHeader(4);
  public string FridayHeader => FormatHeader(5);
  public string SaturdayHeader => FormatHeader(6);

  public bool IsLoading => IsBusy || IsSaving;
  public bool CanEditShifts => !IsBusy && !IsSaving;

  public override async Task InitializeAsync()
  {
    if (hasInitialized)
    {
      await LoadSchedulesAsync().ConfigureAwait(false);
      return;
    }

    try
    {
      IsBusy = true;

      SetWeek(DateOnly.FromDateTime(DateTime.Today));
      LoadChargings();
      await EnsureShiftOptionsAsync().ConfigureAwait(false);

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

    if (SelectedCharging is not null)
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

  private bool CanChangeWeek() => !IsBusy && !IsSaving;

  partial void OnSelectedChargingChanged(ChargingSummary? value)
  {
    if (suppressSelectedChargingChanged || !hasInitialized) return;
    if (value is null)
    {
      ResetEmployeesCollection();
      return;
    }
    _ = LoadSchedulesAsync();
  }

  partial void OnSelectedTenantChanged(TenantDto value)
  {
    if (!hasInitialized) return;
    _ = LoadSchedulesAsync();
  }

  partial void OnCurrentWeekStartChanged(DateOnly value) => RaiseWeekHeaderProperties();
  partial void OnCurrentWeekEndChanged(DateOnly value) => RaiseWeekHeaderProperties();
  partial void OnIsBusyChanged(bool value) => RaiseEditingStateProperties();
  partial void OnIsSavingChanged(bool value) => RaiseEditingStateProperties();

  private async Task LoadSchedulesAsync()
  {
    if (SelectedCharging is null)
    {
      await MainThread.InvokeOnMainThreadAsync(ResetEmployeesCollection);
      return;
    }

    try
    {
      IsBusy = true;

      await EnsureShiftOptionsAsync().ConfigureAwait(false);

      IReadOnlyList<EmployeeScheduleSummary> schedules = await scheduleApiService
        .GetSchedulesAsync(SelectedCharging.Name, CurrentWeekStart, CurrentWeekEnd, tenant: SelectedTenant)
        .ConfigureAwait(false);

      var weeklySchedules = new List<WeeklyEmployeeSchedule>();
      int rowIndex = 0;

      foreach (var employee in schedules.OrderBy(emp => emp.Name, StringComparer.OrdinalIgnoreCase))
      {
        var days = new List<DailyShiftSelection>(7);

        for (int offset = 0; offset < 7; offset++)
        {
          var date = CurrentWeekStart.AddDays(offset);
          var shiftInfo = employee.Shifts.FirstOrDefault(info => info.ScheduleDate == date);

          var selected = shiftInfo?.Shift is null ? noShiftOption : ResolveShiftOption(shiftInfo.Shift);
          Guid? scheduleId = shiftInfo is { ScheduleId: not Guid.Empty } ? shiftInfo.ScheduleId : null;
          bool isDefault = shiftInfo?.IsDefault ?? false;

          var day = new DailyShiftSelection(employee.EmployeeId, date, ShiftOptions, selected, scheduleId, isDefault);
          AttachDaySelectionHandlers(day);
          days.Add(day);
        }

        var weekly = new WeeklyEmployeeSchedule(
          employee.EmployeeId,
          employee.Barcode,
          employee.Name,
          employee.Section,
          employee.Charging,
          days,
          rowIndex++);

        weeklySchedules.Add(weekly);
      }

      await MainThread.InvokeOnMainThreadAsync(() =>
      {
        ResetEmployeesCollection();

        foreach (var schedule in weeklySchedules)
        {
          Employees.Add(schedule);
        }
      });

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

  private void LoadChargings()
  {
    suppressSelectedChargingChanged = true;

    Guid? previousId = SelectedCharging?.Id;
    Chargings.Clear();

    foreach (var charging in referenceDataService.Chargings.OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase))
      Chargings.Add(charging);

    if (previousId is Guid pid)
    {
      var existing = Chargings.FirstOrDefault(c => c.Id == pid);
      if (existing is not null) SelectedCharging = existing;
    }

    if (SelectedCharging is null && Chargings.Count > 0)
      SelectedCharging = Chargings[0];

    suppressSelectedChargingChanged = false;
  }

  private async Task EnsureShiftOptionsAsync()
  {
    if (shiftsLoaded) return;

    try
    {
      var shifts = await shiftApiService.GetShiftsAsync().ConfigureAwait(false);

      ShiftOptions.Clear();
      ShiftOptions.Add(noShiftOption);
      shiftLookup.Clear();

      foreach (var shift in shifts.Where(s => s.Id != Guid.Empty)
                                  .OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase))
      {
        var option = CreateShiftOption(shift);
        InsertShiftOption(option);
      }

      shiftsLoaded = true;
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Loading shifts");
    }
  }

  private void ResetEmployeesCollection()
  {
    foreach (var weekly in Employees)
    {
      foreach (var day in weekly.Days)
      {
        DetachDaySelectionHandlers(day);
      }
    }

    Employees.Clear();
    previousSelections.Clear();
  }

  private void AttachDaySelectionHandlers(DailyShiftSelection day)
  {
    day.PropertyChanging += OnDayPropertyChanging;
    day.PropertyChanged += OnDayPropertyChanged;
  }

  private void DetachDaySelectionHandlers(DailyShiftSelection day)
  {
    day.PropertyChanging -= OnDayPropertyChanging;
    day.PropertyChanged -= OnDayPropertyChanged;
    previousSelections.Remove(day);
  }

  private void OnDayPropertyChanging(object? sender, PropertyChangingEventArgs e)
  {
    if (sender is DailyShiftSelection day && e.PropertyName == nameof(DailyShiftSelection.SelectedShift))
    {
      previousSelections[day] = day.SelectedShift;
    }
  }

  private async void OnDayPropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    if (suppressSelectionChange)
    {
      return;
    }

    if (sender is not DailyShiftSelection day || e.PropertyName != nameof(DailyShiftSelection.SelectedShift))
    {
      return;
    }

    if (!previousSelections.TryGetValue(day, out var previousShift))
    {
      return;
    }

    var newShift = day.SelectedShift;

    try
    {
      await PersistShiftChangeAsync(day, previousShift, newShift);
    }
    finally
    {
      previousSelections.Remove(day);
    }
  }

  private async Task PersistShiftChangeAsync(DailyShiftSelection day, ShiftPickerItem previousShift, ShiftPickerItem newShift)
  {
    if (ReferenceEquals(previousShift, newShift))
    {
      return;
    }

    if (newShift.Id is null && day.ScheduleId is null)
    {
      return;
    }

    string action = "None";
    Guid? resultingScheduleId = day.ScheduleId;
    Guid? newShiftId = newShift.Id;
    Guid? previousShiftId = previousShift.Id;

    try
    {
      IsSaving = true;

      if (newShift.Id is Guid shiftId)
      {
        if (day.ScheduleId is Guid existingId)
        {
          var update = new ScheduleSummary
          {
            Id = existingId,
            EmployeeId = day.EmployeeId,
            Name = newShift.Name,
            ShiftId = shiftId,
            ScheduleDate = day.Date,
            IsDefault = day.IsDefault
          };

          var updated = await scheduleApiService.UpdateScheduleAsync(update).ConfigureAwait(false);
          if (updated is null)
          {
            throw new InvalidOperationException("Failed to update schedule.");
          }

          await MainThread.InvokeOnMainThreadAsync(() => day.UpdateScheduleInfo(updated.Id, updated.IsDefault));
          resultingScheduleId = updated.Id;
          action = "Update";
        }
        else
        {
          var create = new ScheduleSummary
          {
            EmployeeId = day.EmployeeId,
            Name = newShift.Name,
            ShiftId = shiftId,
            ScheduleDate = day.Date,
            IsDefault = false
          };

          Guid? createdId = await scheduleApiService.CreateScheduleAsync(create).ConfigureAwait(false);
          if (createdId is not Guid scheduleId)
          {
            throw new InvalidOperationException("Failed to create schedule.");
          }

          await MainThread.InvokeOnMainThreadAsync(() => day.UpdateScheduleInfo(scheduleId, create.IsDefault));
          resultingScheduleId = scheduleId;
          action = "Create";
        }
      }
      else if (day.ScheduleId is Guid scheduleId)
      {
        bool deleted = await scheduleApiService.DeleteScheduleAsync(scheduleId).ConfigureAwait(false);
        if (!deleted)
        {
          throw new InvalidOperationException("Failed to delete schedule.");
        }

        await MainThread.InvokeOnMainThreadAsync(() => day.UpdateScheduleInfo(null, false));
        resultingScheduleId = null;
        action = "Delete";
      }

      if (action != "None")
      {
        await TraceCommandAsync(nameof(PersistShiftChangeAsync), new
        {
          Action = action,
          day.EmployeeId,
          day.Date,
          NewShiftId = newShiftId,
          PreviousShiftId = previousShiftId,
          ScheduleId = resultingScheduleId
        }).ConfigureAwait(false);
      }
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Saving schedule changes");

      await MainThread.InvokeOnMainThreadAsync(() =>
      {
        suppressSelectionChange = true;
        day.SelectedShift = previousShift;
        suppressSelectionChange = false;
      });
    }
    finally
    {
      IsSaving = false;
    }
  }

  private void SetWeek(DateOnly referenceDate)
  {
    var start = GetStartOfWeek(referenceDate); // Sunday start
    CurrentWeekStart = start;
    CurrentWeekEnd = start.AddDays(6);
  }

  private static DateOnly GetStartOfWeek(DateOnly date)
  {
    int difference = (int)date.DayOfWeek; // Sunday=0
    return date.AddDays(-difference);
  }

  private string FormatHeader(int offset)
  {
    var date = CurrentWeekStart.AddDays(offset);
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

  private void RaiseEditingStateProperties()
  {
    OnPropertyChanged(nameof(IsLoading));
    OnPropertyChanged(nameof(CanEditShifts));
    PreviousWeekAsyncCommand.NotifyCanExecuteChanged();
    NextWeekAsyncCommand.NotifyCanExecuteChanged();
  }

  private ShiftPickerItem ResolveShiftOption(ScheduleShiftDetailsSummary shift)
  {
    if (shift.Id == Guid.Empty) return noShiftOption;

    if (shiftLookup.TryGetValue(shift.Id, out var existing))
      return existing;

    var option = new ShiftPickerItem(
      shift.Id,
      shift.Name,
      BuildShiftDisplay(shift.Name, shift.ShiftStartTime, shift.ShiftEndTime),
      BuildShiftDescription(shift.ShiftStartTime, shift.ShiftBreakTime, shift.ShiftBreakEndTime, shift.ShiftEndTime, shift.BreakHours));

    InsertShiftOption(option);
    return option;
  }

  private void InsertShiftOption(ShiftPickerItem option)
  {
    if (option.Id.HasValue && shiftLookup.ContainsKey(option.Id.Value)) return;

    int insertIndex = 1;
    while (insertIndex < ShiftOptions.Count &&
           string.Compare(ShiftOptions[insertIndex].DisplayName, option.DisplayName, StringComparison.CurrentCultureIgnoreCase) < 0)
    {
      insertIndex++;
    }

    ShiftOptions.Insert(insertIndex, option);
    if (option.Id.HasValue) shiftLookup[option.Id.Value] = option;
  }

  private ShiftPickerItem CreateShiftOption(ShiftSummary summary)
  {
    var start = ParseTime(summary.ShiftStartTime);
    var breakStart = ParseTime(summary.ShiftBreakTime);
    var breakEnd = ParseTime(summary.ShiftBreakEndTime);
    var end = ParseTime(summary.ShiftEndTime);

    var display = BuildShiftDisplay(summary.Name, start, end);
    var description = BuildShiftDescription(start, breakStart, breakEnd, end, summary.BreakHours);

    return new ShiftPickerItem(summary.Id, summary.Name, display, description);
  }

  private static TimeOnly? ParseTime(string? value)
  {
    if (string.IsNullOrWhiteSpace(value)) return null;

    if (TimeOnly.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out var parsed) ||
        TimeOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
      return parsed;

    if (TimeSpan.TryParse(value, CultureInfo.CurrentCulture, out var span) ||
        TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out span))
      return TimeOnly.FromTimeSpan(span);

    return null;
  }

  private static string BuildShiftDisplay(string name, TimeOnly? start, TimeOnly? end)
  {
    string startText = FormatTime(start);
    string endText = FormatTime(end);
    return (!string.IsNullOrEmpty(startText) && !string.IsNullOrEmpty(endText))
      ? $"{name} ({startText} - {endText})"
      : name;
  }

  private static string BuildShiftDescription(TimeOnly? start, TimeOnly? breakStart, TimeOnly? breakEnd, TimeOnly? end, decimal breakHours)
  {
    var segments = new List<string>();

    string startText = FormatTime(start);
    string breakStartText = FormatTime(breakStart);
    string breakEndText = FormatTime(breakEnd);
    string endText = FormatTime(end);

    if (!string.IsNullOrEmpty(startText) && !string.IsNullOrEmpty(breakStartText))
      segments.Add($"{startText} - {breakStartText}");

    if (!string.IsNullOrEmpty(breakEndText) && !string.IsNullOrEmpty(endText))
      segments.Add($"{breakEndText} - {endText}");

    if (segments.Count == 0 && !string.IsNullOrEmpty(startText) && !string.IsNullOrEmpty(endText))
      segments.Add($"{startText} - {endText}");

    if (breakHours > 0)
      segments.Add(breakHours == 1 ? "1 hour break" : $"{breakHours:0.##} hour break");

    return string.Join(" • ", segments);
  }

  private static string FormatTime(TimeOnly? time) => time?.ToString("hh:mm tt", CultureInfo.CurrentCulture) ?? string.Empty;
}

public sealed class WeeklyEmployeeSchedule : IRowIndexed
{
  public WeeklyEmployeeSchedule(Guid employeeId, string barcode, string name, string section, string charging, IReadOnlyList<DailyShiftSelection> days, int rowIndex)
  {
    if (days.Count != 7) throw new ArgumentException("Seven day entries are required.", nameof(days));
    EmployeeId = employeeId;
    Barcode = barcode;
    Name = name;
    Section = section;
    Charging = charging;
    Days = days;
    RowIndex = rowIndex;

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

  public int RowIndex { get; set; }

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
  private readonly IList<ShiftPickerItem> shiftOptions;

  public DailyShiftSelection(
    Guid employeeId,
    DateOnly date,
    IList<ShiftPickerItem> shiftOptions,
    ShiftPickerItem? selectedShift,
    Guid? scheduleId,
    bool isDefault)
  {
    this.shiftOptions = shiftOptions ?? throw new ArgumentNullException(nameof(shiftOptions));
    EmployeeId = employeeId;
    Date = date;
    ScheduleId = scheduleId;
    IsDefault = isDefault;
    SelectedShift = ResolveShiftReference(selectedShift);
  }

  public Guid EmployeeId { get; }
  public DateOnly Date { get; }
  public IList<ShiftPickerItem> ShiftOptions => shiftOptions;
  public Guid? ScheduleId { get; private set; }
  public bool IsDefault { get; private set; }

  [ObservableProperty]
  public partial ShiftPickerItem SelectedShift { get; set; } = ShiftPickerItem.CreateNone();

  // If you ever need index binding, you can expose it; otherwise SelectedItem binding is enough.
  partial void OnSelectedShiftChanged(ShiftPickerItem value)
  {
    // no-op: SelectedItem binding is sufficient for MAUI Picker
    // (left here in case you want to hook persistence later)
  }

  private ShiftPickerItem ResolveShiftReference(ShiftPickerItem? shift)
  {
    if (shiftOptions.Count == 0) return shift ?? ShiftPickerItem.CreateNone();
    if (shift is null) return shiftOptions[0];

    int index = shiftOptions.IndexOf(shift);
    if (index >= 0) return shiftOptions[index];

    var id = shift.Id;
    if (id is null) return shiftOptions[0];

    var match = shiftOptions.FirstOrDefault(option => option.Id == id);
    return match ?? shiftOptions[0];
  }

  internal void UpdateScheduleInfo(Guid? scheduleId, bool isDefault)
  {
    ScheduleId = scheduleId;
    IsDefault = isDefault;
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

  public static ShiftPickerItem CreateNone() => new();
}

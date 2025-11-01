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

  private readonly HashSet<DailyShiftSelection> pendingDailyUpdates = new();

  private bool hasInitialized;
  private bool shiftsLoaded;
  private bool suppressSelectedChargingChanged;
  private bool isLoadingSchedules;

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

  private bool CanChangeWeek() => !IsBusy;

  partial void OnSelectedChargingChanged(ChargingSummary? value)
  {
    if (suppressSelectedChargingChanged || !hasInitialized)
    {
      return;
    }

    if (value is null)
    {
      DetachEmployeeSubscriptions();
      Employees.Clear();
      return;
    }

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

  partial void OnIsBusyChanged(bool value)
  {
    PreviousWeekCommand.NotifyCanExecuteChanged();
    NextWeekCommand.NotifyCanExecuteChanged();
  }

  private async Task LoadSchedulesAsync()
  {
    if (SelectedCharging is null)
    {
      DetachEmployeeSubscriptions();
      Employees.Clear();
      return;
    }

    try
    {
      isLoadingSchedules = true;
      IsBusy = true;

      await EnsureShiftOptionsAsync().ConfigureAwait(false);

      IReadOnlyList<EmployeeScheduleSummary> schedules = await scheduleApiService
        .GetSchedulesAsync(
          SelectedCharging.Name,
          CurrentWeekStart,
          CurrentWeekEnd,
          tenant: SelectedTenant)
        .ConfigureAwait(false);

      DetachEmployeeSubscriptions();
      Employees.Clear();

      int rowIndex = 0;

      foreach (EmployeeScheduleSummary employee in schedules.OrderBy(emp => emp.Name, StringComparer.OrdinalIgnoreCase))
      {
        List<DailyShiftSelection> days = new(7);

        for (int offset = 0; offset < 7; offset++)
        {
          DateOnly date = CurrentWeekStart.AddDays(offset);
          ScheduleShiftInfoSummary? shiftInfo = employee.Shifts.FirstOrDefault(info => info.ScheduleDate == date);

          ShiftPickerItem shiftItem = shiftInfo?.Shift is null
            ? noShiftOption
            : ResolveShiftOption(shiftInfo.Shift);

          Guid? scheduleId = shiftInfo is null || shiftInfo.ScheduleId == Guid.Empty
            ? null
            : shiftInfo.ScheduleId;

          DailyShiftSelection day = new(
            employee.EmployeeId,
            date,
            ShiftOptions,
            shiftItem,
            scheduleId,
            shiftInfo?.IsDefault ?? false);
          days.Add(day);
        }

        WeeklyEmployeeSchedule weeklySchedule = new(
          employee.EmployeeId,
          employee.Barcode,
          employee.Name,
          employee.Section,
          employee.Charging,
          days);

        weeklySchedule.RowIndex = rowIndex++;
        Employees.Add(weeklySchedule);
        SubscribeToEmployeeSchedule(weeklySchedule);
      }

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
      isLoadingSchedules = false;
      IsBusy = pendingDailyUpdates.Count > 0;
    }
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

  private void SubscribeToEmployeeSchedule(WeeklyEmployeeSchedule schedule)
  {
    foreach (DailyShiftSelection day in schedule.Days)
    {
      day.PropertyChanged += OnDailyShiftSelectionPropertyChanged;
    }
  }

  private void DetachEmployeeSubscriptions()
  {
    foreach (WeeklyEmployeeSchedule schedule in Employees)
    {
      foreach (DailyShiftSelection day in schedule.Days)
      {
        day.PropertyChanged -= OnDailyShiftSelectionPropertyChanged;
      }
    }
  }

  private void OnDailyShiftSelectionPropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    if (sender is not DailyShiftSelection day || e.PropertyName != nameof(DailyShiftSelection.SelectedShift))
    {
      return;
    }

    ShiftPickerItem previousShift = day.PersistedShift;
    _ = PersistShiftChangeAsync(day, previousShift);
  }

  private async Task PersistShiftChangeAsync(DailyShiftSelection day, ShiftPickerItem previousShift)
  {
    if (!pendingDailyUpdates.Add(day))
    {
      return;
    }

    try
    {
      if (!isLoadingSchedules && pendingDailyUpdates.Count == 1)
      {
        IsBusy = true;
      }

      Guid? selectedShiftId = day.SelectedShift.Id;

      if (selectedShiftId is null)
      {
        bool hadSchedule = day.ScheduleId.HasValue;

        if (hadSchedule)
        {
          bool deleted = await scheduleApiService.DeleteScheduleAsync(day.ScheduleId.Value).ConfigureAwait(false);
          if (!deleted)
          {
            RevertSelection(day, previousShift);
            return;
          }

          day.ClearSchedule();
        }

        day.UpdatePersistedShift();

        if (hadSchedule)
        {
          await TraceCommandAsync(nameof(scheduleApiService.DeleteScheduleAsync), new
          {
            day.EmployeeId,
            day.Date
          }).ConfigureAwait(false);
        }

        return;
      }

      if (day.ScheduleId.HasValue)
      {
        ScheduleSummary schedule = new()
        {
          Id = day.ScheduleId.Value,
          EmployeeId = day.EmployeeId,
          ShiftId = selectedShiftId.Value,
          ScheduleDate = day.Date,
          IsDefault = day.IsDefault,
          Name = day.SelectedShift.Name
        };

        ScheduleSummary? updated = await scheduleApiService.UpdateScheduleAsync(schedule).ConfigureAwait(false);

        if (updated is null)
        {
          RevertSelection(day, previousShift);
          return;
        }

        day.SetSchedule(updated.Id, updated.IsDefault);
        day.UpdatePersistedShift();

        await TraceCommandAsync(nameof(scheduleApiService.UpdateScheduleAsync), new
        {
          schedule.Id,
          schedule.EmployeeId,
          schedule.ScheduleDate,
          schedule.ShiftId
        }).ConfigureAwait(false);
      }
      else
      {
        ScheduleSummary schedule = new()
        {
          EmployeeId = day.EmployeeId,
          ShiftId = selectedShiftId.Value,
          ScheduleDate = day.Date,
          IsDefault = day.IsDefault,
          Name = day.SelectedShift.Name
        };

        Guid? createdId = await scheduleApiService.CreateScheduleAsync(schedule).ConfigureAwait(false);

        if (!createdId.HasValue || createdId.Value == Guid.Empty)
        {
          RevertSelection(day, previousShift);
          return;
        }

        day.SetSchedule(createdId.Value, schedule.IsDefault);
        day.UpdatePersistedShift();

        await TraceCommandAsync(nameof(scheduleApiService.CreateScheduleAsync), new
        {
          schedule.EmployeeId,
          schedule.ScheduleDate,
          schedule.ShiftId
        }).ConfigureAwait(false);
      }
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Updating schedule");
      RevertSelection(day, previousShift);
    }
    finally
    {
      pendingDailyUpdates.Remove(day);
      if (!isLoadingSchedules && pendingDailyUpdates.Count == 0)
      {
        IsBusy = false;
      }
    }
  }

  private void RevertSelection(DailyShiftSelection day, ShiftPickerItem previousShift)
  {
    MainThread.BeginInvokeOnMainThread(() =>
    {
      day.PropertyChanged -= OnDailyShiftSelectionPropertyChanged;
      day.SelectedShift = previousShift;
      day.PropertyChanged += OnDailyShiftSelectionPropertyChanged;
    });
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

public sealed class WeeklyEmployeeSchedule : IRowIndexed
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

  public int RowIndex { get; set; }
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
    PersistedShift = SelectedShift;
  }

  public Guid EmployeeId { get; }

  public DateOnly Date { get; }

  public Guid? ScheduleId { get; private set; }

  public bool IsDefault { get; private set; }

  public IList<ShiftPickerItem> ShiftOptions => shiftOptions;

  [ObservableProperty]
  public partial ShiftPickerItem SelectedShift { get; set; }

  public ShiftPickerItem PersistedShift { get; private set; }

  public void SetSchedule(Guid scheduleId, bool isDefault)
  {
    ScheduleId = scheduleId;
    IsDefault = isDefault;
  }

  public void ClearSchedule()
  {
    ScheduleId = null;
    IsDefault = false;
  }

  public void UpdatePersistedShift()
  {
    PersistedShift = SelectedShift;
  }

  private ShiftPickerItem ResolveShiftReference(ShiftPickerItem? shift)
  {
    if (shiftOptions.Count == 0)
    {
      return shift ?? ShiftPickerItem.CreateNone();
    }

    if (shift is null)
    {
      return shiftOptions[0];
    }

    int index = shiftOptions.IndexOf(shift);
    if (index >= 0)
    {
      return shiftOptions[index];
    }

    Guid? id = shift.Id;
    if (id is null)
    {
      return shiftOptions[0];
    }

    ShiftPickerItem? match = shiftOptions.FirstOrDefault(option => option.Id == id);
    return match ?? shiftOptions[0];
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

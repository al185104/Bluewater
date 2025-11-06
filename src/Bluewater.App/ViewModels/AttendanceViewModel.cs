using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;

namespace Bluewater.App.ViewModels;

public partial class AttendanceViewModel : BaseViewModel
{
  private const string DefaultDetailsPrimaryActionText = "Close";
  private const string SaveDetailsPrimaryActionText = "Save Changes";

  private readonly IAttendanceApiService attendanceApiService;
  private readonly IReferenceDataService referenceDataService;
  private readonly ITimesheetApiService timesheetApiService;
  private readonly IShiftApiService shiftApiService;
  private bool hasInitialized;
  private bool suppressSelectedChargingChanged;
  private bool hasLoadedShifts;
  private Guid? selectedAttendanceOriginalShiftId;
  private EditableTimesheetEntry? editableTimesheet;

  public AttendanceViewModel(
    IAttendanceApiService attendanceApiService,
    IReferenceDataService referenceDataService,
    ITimesheetApiService timesheetApiService,
    IShiftApiService shiftApiService,
    IActivityTraceService activityTraceService,
    IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
    this.attendanceApiService = attendanceApiService;
    this.referenceDataService = referenceDataService;
    this.timesheetApiService = timesheetApiService;
    this.shiftApiService = shiftApiService;
    SetCurrentPayslipPeriod();
  }

  public ObservableCollection<ChargingSummary> Chargings { get; } = new();
  public ObservableCollection<EmployeeAttendanceSummary> EmployeeAttendances { get; } = new();
  public ObservableCollection<AttendanceSummary> DisplayAttendances { get; } = new();
  public ObservableCollection<ShiftSummary> ShiftOptions { get; } = new();
  public IReadOnlyList<TenantDto> TenantOptions { get; } = Array.AsReadOnly(Enum.GetValues<TenantDto>());

  [ObservableProperty]
  public partial ChargingSummary? SelectedCharging { get; set; }

  [ObservableProperty]
  public partial DateOnly StartDate { get; set; }

  [ObservableProperty]
  public partial DateOnly EndDate { get; set; }

  [ObservableProperty]
  public partial TenantDto SelectedTenant { get; set; } = TenantDto.Maribago;

  [ObservableProperty]
  public partial bool IsDetailsOpen { get; set; }

  [ObservableProperty]
  public partial string DetailsTitle { get; set; } = string.Empty;

  [ObservableProperty]
  public partial string DetailsPrimaryActionText { get; set; } = DefaultDetailsPrimaryActionText;

  [ObservableProperty]
  public partial EmployeeAttendanceSummary? SelectedEmployeeAttendance { get; set; }

  [ObservableProperty]
  public partial AttendanceSummary? SelectedAttendance { get; set; }

  [ObservableProperty]
  public partial ShiftSummary? SelectedShift { get; set; }

  [ObservableProperty]
  public partial bool CanSaveAttendanceChanges { get; set; }

  [ObservableProperty]
  public partial bool IsSavingAttendance { get; set; }

  public EditableTimesheetEntry? EditableTimesheet
  {
    get => editableTimesheet;
    set
    {
      EditableTimesheetEntry? previous = editableTimesheet;

      if (!SetProperty(ref editableTimesheet, value))
      {
        return;
      }

      if (previous is not null)
      {
        previous.PropertyChanged -= OnEditableTimesheetPropertyChanged;
      }

      if (value is not null)
      {
        value.PropertyChanged += OnEditableTimesheetPropertyChanged;
      }

      UpdateCanSaveAttendanceChanges();
    }
  }

  private void OnEditableTimesheetPropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    MainThread.BeginInvokeOnMainThread(UpdateCanSaveAttendanceChanges);
  }

  partial void OnSelectedChargingChanged(ChargingSummary? value)
  {
    if (suppressSelectedChargingChanged)
    {
      return;
    }

    if (value is null)
    {
      MainThread.BeginInvokeOnMainThread(() =>
      {
        IsDetailsOpen = false;
        ClearEmployeeAttendances();
      });
      return;
    }

    _ = MainThread.InvokeOnMainThreadAsync(LoadAttendanceSummariesAsync);
  }

  partial void OnSelectedTenantChanged(TenantDto value)
  {
    if (SelectedCharging is null)
    {
      return;
    }

    _ = MainThread.InvokeOnMainThreadAsync(LoadAttendanceSummariesAsync);
  }

  partial void OnStartDateChanged(DateOnly value)
  {
    RefreshSelectedAttendanceDetails();
  }

  partial void OnEndDateChanged(DateOnly value)
  {
    RefreshSelectedAttendanceDetails();
  }

  partial void OnSelectedAttendanceChanged(AttendanceSummary? value)
  {
    selectedAttendanceOriginalShiftId = value?.ShiftId is Guid shiftId && shiftId != Guid.Empty
      ? shiftId
      : null;

    if (value is null || value.Id == Guid.Empty)
    {
      SelectedShift = null;
      EditableTimesheet = null;
      UpdateCanSaveAttendanceChanges();
      return;
    }

    SetSelectedShiftFromAttendance(value);
    SetEditableTimesheetFromAttendance(value);
    UpdateCanSaveAttendanceChanges();
  }

  partial void OnSelectedShiftChanged(ShiftSummary? value)
  {
    if (SelectedAttendance is null || SelectedAttendance.Id == Guid.Empty)
    {
      UpdateCanSaveAttendanceChanges();
      return;
    }

    Guid? shiftId = value?.Id;
    if (shiftId == Guid.Empty)
    {
      shiftId = null;
    }

    SelectedAttendance.ShiftId = shiftId;
    SelectedAttendance.Shift = MapToAttendanceShiftSummary(value);
    UpdateCanSaveAttendanceChanges();
  }

  partial void OnIsSavingAttendanceChanged(bool value)
  {
    UpdateCanSaveAttendanceChanges();
  }

  partial void OnIsDetailsOpenChanged(bool value)
  {
    if (value)
    {
      return;
    }

    MainThread.BeginInvokeOnMainThread(() =>
    {
      DisplayAttendances.Clear();
      DetailsTitle = string.Empty;
      DetailsPrimaryActionText = DefaultDetailsPrimaryActionText;
      SelectedEmployeeAttendance = null;
      SelectedAttendance = null;
      EditableTimesheet = null;
      SelectedShift = null;
      selectedAttendanceOriginalShiftId = null;
      CanSaveAttendanceChanges = false;
      IsSavingAttendance = false;
    });
  }

  public override async Task InitializeAsync()
  {
    if (hasInitialized)
    {
      await LoadAttendanceSummariesAsync().ConfigureAwait(false);
      return;
    }

    hasInitialized = true;
    await TraceCommandAsync(nameof(InitializeAsync)).ConfigureAwait(false);

    MainThread.BeginInvokeOnMainThread(() =>
    {
      LoadChargings();

      if (SelectedCharging is not null)
      {
        _ = LoadAttendanceSummariesAsync();
      }
    });
  }

  [RelayCommand]
  private async Task RefreshAsync()
  {
    await TraceCommandAsync(nameof(RefreshAsync)).ConfigureAwait(false);
    await LoadAttendanceSummariesAsync().ConfigureAwait(false);
  }

  [RelayCommand]
  private async Task ApplyDateRangeAsync()
  {
    await TraceCommandAsync(nameof(ApplyDateRangeAsync)).ConfigureAwait(false);
    await LoadAttendanceSummariesAsync().ConfigureAwait(false);
  }

  [RelayCommand]
  private async Task EditAttendanceAsync(EmployeeAttendanceSummary? summary)
  {
    if (summary is null)
    {
      return;
    }

    await TraceCommandAsync(nameof(EditAttendanceAsync), summary.EmployeeId).ConfigureAwait(false);

    await EnsureShiftOptionsLoadedAsync().ConfigureAwait(false);

    await MainThread.InvokeOnMainThreadAsync(() =>
    {
      SelectedEmployeeAttendance = summary;
      DetailsTitle = string.IsNullOrWhiteSpace(summary.Name)
        ? "Attendance Details"
        : $"Attendance Details - {summary.Name}";
      DetailsPrimaryActionText = SaveDetailsPrimaryActionText;
      LoadDisplayAttendances(summary);
      IsDetailsOpen = true;
      CanSaveAttendanceChanges = false;
    }).ConfigureAwait(false);
  }

  [RelayCommand]
  private void CloseAttendanceDetails()
  {
    IsDetailsOpen = false;
  }

  [RelayCommand]
  private async Task SaveAttendanceChangesAsync()
  {
    if (IsSavingAttendance || SelectedAttendance is null || SelectedAttendance.Id == Guid.Empty)
    {
      return;
    }

    bool shiftChanged = selectedAttendanceOriginalShiftId != (SelectedShift?.Id == Guid.Empty ? null : SelectedShift?.Id);
    bool timesheetChanged = EditableTimesheet?.HasChanges == true;

    if (!shiftChanged && !timesheetChanged)
    {
      return;
    }

    await TraceCommandAsync(nameof(SaveAttendanceChangesAsync), SelectedAttendance.Id).ConfigureAwait(false);

    try
    {
      IsSavingAttendance = true;

      AttendanceTimesheetSummary? updatedTimesheet = null;

      if (timesheetChanged && EditableTimesheet is not null)
      {
        try
        {
          UpdateTimesheetRequestDto request = EditableTimesheet.ToUpdateRequest();
          updatedTimesheet = await timesheetApiService
            .UpdateTimesheetAsync(request)
            .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
          ExceptionHandlingService.Handle(ex, "Updating timesheet");
          return;
        }
      }

      AttendanceSummary? updatedAttendance = null;

      if (shiftChanged || updatedTimesheet is not null)
      {
        try
        {
          AttendanceSummary request = new()
          {
            Id = SelectedAttendance.Id,
            EmployeeId = SelectedAttendance.EmployeeId,
            EntryDate = SelectedAttendance.EntryDate,
            ShiftId = SelectedShift?.Id,
            TimesheetId = EditableTimesheet?.Id ?? SelectedAttendance.TimesheetId,
            LeaveId = SelectedAttendance.LeaveId,
            IsLocked = SelectedAttendance.IsLocked
          };

          updatedAttendance = await attendanceApiService
            .UpdateAttendanceAsync(request)
            .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
          ExceptionHandlingService.Handle(ex, "Updating attendance");
          return;
        }
      }

      await MainThread.InvokeOnMainThreadAsync(() =>
      {
        if (updatedTimesheet is not null)
        {
          EditableTimesheet?.ApplySummary(updatedTimesheet);
          SelectedAttendance.TimesheetId = updatedTimesheet.Id;
          SelectedAttendance.Timesheet = updatedTimesheet;
          ApplyTimesheetUpdateToSummary(updatedTimesheet);
        }

        if (updatedAttendance is not null)
        {
          SelectedAttendance.ShiftId = updatedAttendance.ShiftId;
          SelectedAttendance.Shift = updatedAttendance.Shift;
          SelectedAttendance.TimesheetId = updatedAttendance.TimesheetId;

          if (updatedAttendance.Timesheet is not null)
          {
            SelectedAttendance.Timesheet = updatedAttendance.Timesheet;
            EditableTimesheet?.ApplySummary(updatedAttendance.Timesheet);
            ApplyTimesheetUpdateToSummary(updatedAttendance.Timesheet);
          }

          ApplyAttendanceUpdateToSummary(updatedAttendance);
        }
        else if (shiftChanged)
        {
          SelectedAttendance.Shift = MapToAttendanceShiftSummary(SelectedShift);
          ApplyAttendanceUpdateToSummary(SelectedAttendance);
        }

        selectedAttendanceOriginalShiftId = SelectedShift?.Id;
        EditableTimesheet?.ResetChangeTracking();
        UpdateCanSaveAttendanceChanges();
      }).ConfigureAwait(false);
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Saving attendance changes");
    }
    finally
    {
      IsSavingAttendance = false;
      UpdateCanSaveAttendanceChanges();
    }
  }

  [RelayCommand]
  private async Task SubmitAsync()
  {
    if (IsBusy || EmployeeAttendances.Count == 0)
    {
      return;
    }

    await TraceCommandAsync(nameof(SubmitAsync)).ConfigureAwait(false);

    try
    {
      IsBusy = true;

      foreach (EmployeeAttendanceSummary summary in EmployeeAttendances)
      {
        foreach (AttendanceSummary attendance in summary.Attendances)
        {
          try
          {
            if (attendance.EmployeeId == Guid.Empty)
            {
              attendance.EmployeeId = summary.EmployeeId;
            }

            if (attendance.EntryDate is null)
            {
              continue;
            }

            await attendanceApiService.CreateAttendanceAsync(attendance).ConfigureAwait(false);
          }
          catch (Exception ex)
          {
            ExceptionHandlingService.Handle(ex, "Creating attendance");
          }
        }
      }
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Submitting attendances");
    }
    finally
    {
      IsBusy = false;
    }

    await LoadAttendanceSummariesAsync().ConfigureAwait(false);
  }

  private async Task LoadAttendanceSummariesAsync()
  {
    if (SelectedCharging is null)
    {
      MainThread.BeginInvokeOnMainThread(ClearEmployeeAttendances);
      return;
    }

    try
    {
      IsBusy = true;

      await TraceCommandAsync(nameof(LoadAttendanceSummariesAsync), SelectedCharging.Id).ConfigureAwait(false);

      IReadOnlyList<EmployeeAttendanceSummary> summaries = await attendanceApiService
        .GetAttendanceSummariesAsync(
          SelectedCharging.Name,
          StartDate,
          EndDate,
          tenant: SelectedTenant)
        .ConfigureAwait(false);

      Guid? openEmployeeId = SelectedEmployeeAttendance?.EmployeeId;

      await MainThread.InvokeOnMainThreadAsync(() =>
      {
        ClearEmployeeAttendances();
        foreach (EmployeeAttendanceSummary summary in summaries)
        {
          EmployeeAttendances.Add(summary);
        }

        if (IsDetailsOpen && openEmployeeId is Guid id)
        {
          EmployeeAttendanceSummary? selected = EmployeeAttendances.FirstOrDefault(item => item.EmployeeId == id);
          if (selected is not null)
          {
            SelectedEmployeeAttendance = selected;
            LoadDisplayAttendances(selected);
          }
          else
          {
            IsDetailsOpen = false;
          }
        }
      }).ConfigureAwait(false);
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Loading attendances");
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
    foreach (ChargingSummary charging in referenceDataService.Chargings
                 .OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase))
    {
      Chargings.Add(charging);
    }

    if (previousId is Guid id)
    {
      ChargingSummary? previous = Chargings.FirstOrDefault(item => item.Id == id);
      if (previous is not null)
      {
        SelectedCharging = previous;
      }
    }

    if (SelectedCharging is null && Chargings.Count > 0)
    {
      SelectedCharging = Chargings[0];
    }

    suppressSelectedChargingChanged = false;

    if (SelectedCharging is not null)
    {
      _ = LoadAttendanceSummariesAsync();
    }
  }

  private async Task EnsureShiftOptionsLoadedAsync()
  {
    if (hasLoadedShifts)
    {
      return;
    }

    try
    {
      IReadOnlyList<ShiftSummary> shifts = await shiftApiService.GetShiftsAsync().ConfigureAwait(false);

      await MainThread.InvokeOnMainThreadAsync(() =>
      {
        ShiftOptions.Clear();
        foreach (ShiftSummary shift in shifts.OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase))
        {
          ShiftOptions.Add(shift);
        }
      }).ConfigureAwait(false);

      hasLoadedShifts = true;
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Loading shifts");
    }
  }

  private void ClearEmployeeAttendances()
  {
    EmployeeAttendances.Clear();
    DisplayAttendances.Clear();
    SelectedAttendance = null;
    EditableTimesheet = null;
    SelectedShift = null;
    selectedAttendanceOriginalShiftId = null;
    CanSaveAttendanceChanges = false;
  }

  private void RefreshSelectedAttendanceDetails()
  {
    if (!IsDetailsOpen || SelectedEmployeeAttendance is null)
    {
      return;
    }

    MainThread.BeginInvokeOnMainThread(() => LoadDisplayAttendances(SelectedEmployeeAttendance));
  }

  private void LoadDisplayAttendances(EmployeeAttendanceSummary summary)
  {
    Guid? previousAttendanceId = SelectedAttendance?.Id != Guid.Empty ? SelectedAttendance?.Id : null;
    int? previousRowIndex = SelectedAttendance?.RowIndex;

    DisplayAttendances.Clear();

    Dictionary<DateOnly, AttendanceSummary> attendancesByDate = new();
    foreach (AttendanceSummary attendance in summary.Attendances)
    {
      if (attendance.EntryDate is null)
      {
        continue;
      }

      DateOnly date = attendance.EntryDate.Value;
      if (!attendancesByDate.ContainsKey(date))
      {
        attendancesByDate[date] = attendance;
      }
    }

    int rowIndex = 0;
    for (DateOnly date = StartDate; date <= EndDate; date = date.AddDays(1), rowIndex++)
    {
      if (attendancesByDate.TryGetValue(date, out AttendanceSummary? existing))
      {
        DisplayAttendances.Add(CloneAttendance(existing, rowIndex));
        continue;
      }

      DisplayAttendances.Add(new AttendanceSummary
      {
        EmployeeId = summary.EmployeeId,
        EntryDate = date,
        RowIndex = rowIndex
      });
    }

    AttendanceSummary? selected = null;

    if (previousAttendanceId is Guid id)
    {
      selected = DisplayAttendances.FirstOrDefault(item => item.Id == id);
    }

    if (selected is null && previousRowIndex.HasValue)
    {
      selected = DisplayAttendances.FirstOrDefault(item => item.RowIndex == previousRowIndex.Value && item.Id != Guid.Empty);
    }

    if (selected is null)
    {
      selected = DisplayAttendances.FirstOrDefault(item => item.Id != Guid.Empty);
    }

    SelectedAttendance = selected;
  }

  private static AttendanceSummary CloneAttendance(AttendanceSummary source, int rowIndex)
  {
    return new AttendanceSummary
    {
      Id = source.Id,
      EmployeeId = source.EmployeeId,
      ShiftId = source.ShiftId,
      TimesheetId = source.TimesheetId,
      LeaveId = source.LeaveId,
      EntryDate = source.EntryDate,
      WorkHours = source.WorkHours,
      LateHours = source.LateHours,
      UnderHours = source.UnderHours,
      OverbreakHours = source.OverbreakHours,
      NightShiftHours = source.NightShiftHours,
      IsLocked = source.IsLocked,
      Shift = source.Shift,
      Timesheet = source.Timesheet,
      RowIndex = rowIndex
    };
  }

  private void SetSelectedShiftFromAttendance(AttendanceSummary attendance)
  {
    Guid? shiftId = attendance.ShiftId is Guid id && id != Guid.Empty ? id : null;

    if (shiftId is Guid idValue)
    {
      ShiftSummary? match = ShiftOptions.FirstOrDefault(item => item.Id == idValue);

      if (match is null && attendance.Shift is AttendanceShiftSummary shiftSummary)
      {
        match = new ShiftSummary
        {
          Id = shiftSummary.Id,
          Name = shiftSummary.Name,
          ShiftStartTime = FormatTimeOnly(shiftSummary.ShiftStartTime),
          ShiftBreakTime = FormatTimeOnly(shiftSummary.ShiftBreakTime),
          ShiftBreakEndTime = FormatTimeOnly(shiftSummary.ShiftBreakEndTime),
          ShiftEndTime = FormatTimeOnly(shiftSummary.ShiftEndTime),
          BreakHours = shiftSummary.BreakHours
        };

        ShiftOptions.Add(match);
      }

      SelectedShift = match;
    }
    else
    {
      SelectedShift = null;
    }
  }

  private void SetEditableTimesheetFromAttendance(AttendanceSummary attendance)
  {
    if (attendance.Timesheet is AttendanceTimesheetSummary summary)
    {
      EditableTimesheet = EditableTimesheetEntry.FromSummary(summary);
    }
    else
    {
      EditableTimesheet = null;
    }
  }

  private void ApplyAttendanceUpdateToSummary(AttendanceSummary updated)
  {
    if (SelectedEmployeeAttendance is null)
    {
      return;
    }

    AttendanceSummary? target = SelectedEmployeeAttendance.Attendances
      .FirstOrDefault(item => item.Id == updated.Id && item.EntryDate == updated.EntryDate);

    if (target is null && updated.EntryDate is DateOnly date)
    {
      target = SelectedEmployeeAttendance.Attendances
        .FirstOrDefault(item => item.EntryDate == date);
    }

    if (target is null)
    {
      return;
    }

    target.ShiftId = updated.ShiftId;
    target.Shift = updated.Shift;
    target.TimesheetId = updated.TimesheetId;
    target.Timesheet = updated.Timesheet;
    target.LeaveId = updated.LeaveId;
    target.IsLocked = updated.IsLocked;
  }

  private void ApplyTimesheetUpdateToSummary(AttendanceTimesheetSummary updatedTimesheet)
  {
    if (SelectedEmployeeAttendance is null)
    {
      return;
    }

    foreach (AttendanceSummary attendance in SelectedEmployeeAttendance.Attendances)
    {
      if (attendance.EntryDate == updatedTimesheet.EntryDate)
      {
        attendance.TimesheetId = updatedTimesheet.Id;
        attendance.Timesheet = updatedTimesheet;
      }
    }
  }

  private void UpdateCanSaveAttendanceChanges()
  {
    if (IsSavingAttendance)
    {
      CanSaveAttendanceChanges = false;
      return;
    }

    if (SelectedAttendance is null || SelectedAttendance.Id == Guid.Empty)
    {
      CanSaveAttendanceChanges = false;
      return;
    }

    Guid? currentShiftId = SelectedShift?.Id;
    if (currentShiftId == Guid.Empty)
    {
      currentShiftId = null;
    }

    Guid? originalShiftId = selectedAttendanceOriginalShiftId;
    bool shiftChanged = originalShiftId != currentShiftId;
    bool timesheetChanged = EditableTimesheet?.HasChanges == true;

    CanSaveAttendanceChanges = shiftChanged || timesheetChanged;
  }

  private static AttendanceShiftSummary? MapToAttendanceShiftSummary(ShiftSummary? shift)
  {
    if (shift is null || shift.Id == Guid.Empty)
    {
      return null;
    }

    return new AttendanceShiftSummary
    {
      Id = shift.Id,
      Name = shift.Name,
      ShiftStartTime = ParseTimeOnly(shift.ShiftStartTime),
      ShiftBreakTime = ParseTimeOnly(shift.ShiftBreakTime),
      ShiftBreakEndTime = ParseTimeOnly(shift.ShiftBreakEndTime),
      ShiftEndTime = ParseTimeOnly(shift.ShiftEndTime),
      BreakHours = shift.BreakHours
    };
  }

  private static TimeOnly? ParseTimeOnly(string? value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      return null;
    }

    if (TimeOnly.TryParse(value, out TimeOnly time))
    {
      return time;
    }

    if (TimeSpan.TryParse(value, out TimeSpan span))
    {
      return TimeOnly.FromTimeSpan(span);
    }

    return null;
  }

  private static string? FormatTimeOnly(TimeOnly? value)
  {
    return value?.ToString("HH\\:mm");
  }

  private void SetCurrentPayslipPeriod(DateOnly? referenceDate = null)
  {
    DateOnly date = referenceDate ?? DateOnly.FromDateTime(DateTime.Today);
    (DateOnly startDate, DateOnly endDate) = CalculatePayslipPeriod(date);
    StartDate = startDate;
    EndDate = endDate;
  }

  private static (DateOnly startDate, DateOnly endDate) CalculatePayslipPeriod(DateOnly date)
  {
    if (date.Day >= 11 && date.Day <= 25)
    {
      return (new DateOnly(date.Year, date.Month, 11), new DateOnly(date.Year, date.Month, 25));
    }

    if (date.Day >= 26)
    {
      DateOnly nextMonth = date.AddMonths(1);
      return (new DateOnly(date.Year, date.Month, 26), new DateOnly(nextMonth.Year, nextMonth.Month, 10));
    }

    DateOnly previousMonth = date.AddMonths(-1);
    return (new DateOnly(previousMonth.Year, previousMonth.Month, 26), new DateOnly(date.Year, date.Month, 10));
  }
}

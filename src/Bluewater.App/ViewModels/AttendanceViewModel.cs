using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

  private readonly IAttendanceApiService attendanceApiService;
  private readonly IReferenceDataService referenceDataService;
  private bool hasInitialized;
  private bool suppressSelectedChargingChanged;

  public AttendanceViewModel(
    IAttendanceApiService attendanceApiService,
    IReferenceDataService referenceDataService,
    IActivityTraceService activityTraceService,
    IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
    this.attendanceApiService = attendanceApiService;
    this.referenceDataService = referenceDataService;
    SetCurrentPayslipPeriod();
  }

  public ObservableCollection<ChargingSummary> Chargings { get; } = new();
  public ObservableCollection<EmployeeAttendanceSummary> EmployeeAttendances { get; } = new();
  public ObservableCollection<AttendanceSummary> DisplayAttendances { get; } = new();
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

    await MainThread.InvokeOnMainThreadAsync(() =>
    {
      SelectedEmployeeAttendance = summary;
      DetailsTitle = string.IsNullOrWhiteSpace(summary.Name)
        ? "Attendance Details"
        : $"Attendance Details - {summary.Name}";
      DetailsPrimaryActionText = DefaultDetailsPrimaryActionText;
      LoadDisplayAttendances(summary);
      IsDetailsOpen = true;
    }).ConfigureAwait(false);
  }

  [RelayCommand]
  private void CloseAttendanceDetails()
  {
    IsDetailsOpen = false;
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

  private void ClearEmployeeAttendances()
  {
    EmployeeAttendances.Clear();
    DisplayAttendances.Clear();
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

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
    StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-7));
    EndDate = DateOnly.FromDateTime(DateTime.Today);
  }

  public ObservableCollection<ChargingSummary> Chargings { get; } = new();
  public ObservableCollection<EmployeeAttendanceSummary> EmployeeAttendances { get; } = new();
  public IReadOnlyList<TenantDto> TenantOptions { get; } = Array.AsReadOnly(Enum.GetValues<TenantDto>());

  [ObservableProperty]
  public partial ChargingSummary? SelectedCharging { get; set; }

  [ObservableProperty]
  public partial DateOnly StartDate { get; set; }

  [ObservableProperty]
  public partial DateOnly EndDate { get; set; }

  [ObservableProperty]
  public partial TenantDto SelectedTenant { get; set; } = TenantDto.Maribago;

  partial void OnSelectedChargingChanged(ChargingSummary? value)
  {
    if (suppressSelectedChargingChanged)
    {
      return;
    }

    if (value is null)
    {
      MainThread.BeginInvokeOnMainThread(ClearEmployeeAttendances);
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

      await MainThread.InvokeOnMainThreadAsync(() =>
      {
        ClearEmployeeAttendances();
        foreach (EmployeeAttendanceSummary summary in summaries)
        {
          EmployeeAttendances.Add(summary);
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
  }
}

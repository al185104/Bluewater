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

public partial class TimesheetsViewModel : BaseViewModel
{
  private readonly ITimesheetApiService timesheetApiService;
  private readonly IReferenceDataService referenceDataService;
  private bool hasInitialized;
  private bool suppressSelectedChargingChanged;

  public TimesheetsViewModel(
    ITimesheetApiService timesheetApiService,
    IReferenceDataService referenceDataService,
    IActivityTraceService activityTraceService,
    IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
    this.timesheetApiService = timesheetApiService;
    this.referenceDataService = referenceDataService;

    StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-7));
    EndDate = DateOnly.FromDateTime(DateTime.Today);
  }

  public ObservableCollection<ChargingSummary> Chargings { get; } = new();
  public ObservableCollection<EmployeeTimesheetSummary> Timesheets { get; } = new();
  public IReadOnlyList<TenantDto> TenantOptions { get; } = Array.AsReadOnly(Enum.GetValues<TenantDto>());

  [ObservableProperty]
  public partial ChargingSummary? SelectedCharging { get; set; }

  [ObservableProperty]
  public partial TenantDto SelectedTenant { get; set; } = TenantDto.Maribago;

  [ObservableProperty]
  public partial DateOnly StartDate { get; set; }

  [ObservableProperty]
  public partial DateOnly EndDate { get; set; }

  public override async Task InitializeAsync()
  {
    if (hasInitialized)
    {
      await LoadTimesheetsAsync().ConfigureAwait(false);
      return;
    }

    hasInitialized = true;
    await TraceCommandAsync(nameof(InitializeAsync)).ConfigureAwait(false);

    MainThread.BeginInvokeOnMainThread(() => { LoadChargings();  });

    if (SelectedCharging is not null)
    {
      await LoadTimesheetsAsync().ConfigureAwait(false);
    }
  }

  [RelayCommand]
  private async Task RefreshAsync()
  {
    await TraceCommandAsync(nameof(RefreshAsync)).ConfigureAwait(false);
    await LoadTimesheetsAsync().ConfigureAwait(false);
  }

  [RelayCommand]
  private async Task ApplyDateRangeAsync()
  {
    await TraceCommandAsync(nameof(ApplyDateRangeAsync)).ConfigureAwait(false);
    await LoadTimesheetsAsync().ConfigureAwait(false);
  }

  partial void OnSelectedChargingChanged(ChargingSummary? value)
  {
    if (!hasInitialized || suppressSelectedChargingChanged)
    {
      return;
    }

    _ = LoadTimesheetsAsync();
  }

  partial void OnSelectedTenantChanged(TenantDto value)
  {
    if (!hasInitialized)
    {
      return;
    }

    _ = LoadTimesheetsAsync();
  }

  private async Task LoadTimesheetsAsync()
  {
    if (SelectedCharging is null)
    {
      await MainThread.InvokeOnMainThreadAsync(ClearTimesheets);
      return;
    }

    try
    {
      IsBusy = true;

      IReadOnlyList<EmployeeTimesheetSummary> summaries = await timesheetApiService
        .GetTimesheetSummariesAsync(
          SelectedCharging.Name,
          StartDate,
          EndDate,
          SelectedTenant)
        .ConfigureAwait(false);

      await MainThread.InvokeOnMainThreadAsync(() =>
      {
        Timesheets.Clear();
        foreach (EmployeeTimesheetSummary summary in summaries)
        {
          UpdateTimesheetRowIndexes(summary);
          Timesheets.Add(summary);
        }
      });
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Loading timesheets");
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
  }

  private void ClearTimesheets()
  {
    Timesheets.Clear();
  }

  private static void UpdateTimesheetRowIndexes(EmployeeTimesheetSummary summary)
  {
    for (int i = 0; i < summary.Timesheets.Count; i++)
    {
      summary.Timesheets[i].RowIndex = i;
    }
  }
}

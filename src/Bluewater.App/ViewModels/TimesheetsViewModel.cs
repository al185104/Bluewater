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

public partial class TimesheetsViewModel : BaseViewModel
{
  private const string DefaultDetailsPrimaryActionText = "Save Changes";

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

    SetCurrentPayslipPeriod();
  }

  public ObservableCollection<ChargingSummary> Chargings { get; } = new();
  public ObservableCollection<EmployeeTimesheetSummary> Timesheets { get; } = new();
  public ObservableCollection<EditableTimesheetEntry> EditableTimesheets { get; } = new();
  public IReadOnlyList<TenantDto> TenantOptions { get; } = Array.AsReadOnly(Enum.GetValues<TenantDto>());

  [ObservableProperty]
  public partial ChargingSummary? SelectedCharging { get; set; }

  [ObservableProperty]
  public partial TenantDto SelectedTenant { get; set; } = TenantDto.Maribago;

  [ObservableProperty]
  public partial DateOnly StartDate { get; set; }

  [ObservableProperty]
  public partial DateOnly EndDate { get; set; }

  public string PeriodRangeDisplay => $"{StartDate:MMMM dd} - {EndDate:MMMM dd}";

  [ObservableProperty]
  public partial bool IsDetailsOpen { get; set; }

  [ObservableProperty]
  public partial string DetailsTitle { get; set; } = string.Empty;

  [ObservableProperty]
  public partial string DetailsPrimaryActionText { get; set; } = DefaultDetailsPrimaryActionText;

  [ObservableProperty]
  public partial EmployeeTimesheetSummary? SelectedEmployeeTimesheet { get; set; }

  [ObservableProperty]
  public partial EditableTimesheetEntry? SelectedEditableTimesheet { get; set; }

  public bool CanSaveTimesheets => !IsBusy && EditableTimesheets.Any(entry => entry.HasChanges);

  partial void OnIsDetailsOpenChanged(bool value)
  {
    if (value)
    {
      UpdateCanSaveTimesheets();
      return;
    }

    MainThread.BeginInvokeOnMainThread(() =>
    {
      ClearEditableTimesheets();
      DetailsTitle = string.Empty;
      DetailsPrimaryActionText = DefaultDetailsPrimaryActionText;
      SelectedEmployeeTimesheet = null;
      SelectedEditableTimesheet = null;
    });
  }

  public override void IsBusyChanged(bool isBusy)
  {
    base.IsBusyChanged(isBusy);
    UpdateCanSaveTimesheets();
    RaiseNavigationState();
  }

  public override async Task InitializeAsync()
  {
    if (hasInitialized)
    {
      await LoadTimesheetsAsync().ConfigureAwait(false);
      return;
    }

    hasInitialized = true;
    await TraceCommandAsync(nameof(InitializeAsync)).ConfigureAwait(false);

    MainThread.BeginInvokeOnMainThread(LoadChargings);

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

  [RelayCommand(CanExecute = nameof(CanChangePeriod))]
  private async Task PreviousPeriodAsync()
  {
    SetPreviousPayslipPeriod();
    await TraceCommandAsync(nameof(PreviousPeriodAsync)).ConfigureAwait(false);
    await LoadTimesheetsAsync().ConfigureAwait(false);
  }

  [RelayCommand(CanExecute = nameof(CanChangePeriod))]
  private async Task NextPeriodAsync()
  {
    SetNextPayslipPeriod();
    await TraceCommandAsync(nameof(NextPeriodAsync)).ConfigureAwait(false);
    await LoadTimesheetsAsync().ConfigureAwait(false);
  }

  private bool CanChangePeriod() => !IsBusy;

  [RelayCommand]
  private async Task EditTimesheetAsync(EmployeeTimesheetSummary? summary)
  {
    if (summary is null)
    {
      return;
    }

    MainThread.BeginInvokeOnMainThread(() =>
    {
      SelectedEmployeeTimesheet = summary;
      DetailsTitle = $"Edit Timesheet - {summary.Name}";
      DetailsPrimaryActionText = DefaultDetailsPrimaryActionText;
      LoadEditableTimesheets(summary);
      IsDetailsOpen = true;
    });

    await TraceCommandAsync(nameof(EditTimesheetAsync), summary.EmployeeId).ConfigureAwait(false);
  }

  [RelayCommand]
  private async Task SaveTimesheetsAsync()
  {
    if (IsBusy || EditableTimesheets.Count == 0)
    {
      return;
    }

    List<EditableTimesheetEntry> entriesToUpdate = EditableTimesheets
      .Where(entry => entry.HasChanges)
      .ToList();

    if (entriesToUpdate.Count == 0)
    {
      return;
    }

    bool anyUpdated = false;

    try
    {
      IsBusy = true;

      foreach (EditableTimesheetEntry entry in entriesToUpdate)
      {
        try
        {
          AttendanceTimesheetSummary? updated = await timesheetApiService
            .UpdateTimesheetAsync(entry.ToUpdateRequest())
            .ConfigureAwait(false);

          if (updated is null)
          {
            continue;
          }

          await MainThread.InvokeOnMainThreadAsync(() =>
          {
            entry.ApplySummary(updated);
            UpdateSummaryEntry(updated);
          }).ConfigureAwait(false);

          anyUpdated = true;
        }
        catch (Exception ex)
        {
          ExceptionHandlingService.Handle(ex, "Updating timesheet");
        }
      }
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Updating timesheets");
    }
    finally
    {
      IsBusy = false;
      MainThread.BeginInvokeOnMainThread(UpdateCanSaveTimesheets);
    }

    if (anyUpdated)
    {
      await TraceCommandAsync(nameof(SaveTimesheetsAsync), SelectedEmployeeTimesheet?.EmployeeId)
        .ConfigureAwait(false);
    }
  }

  [RelayCommand]
  private void CloseTimesheetDetails()
  {
    IsDetailsOpen = false;
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

        SyncSelectedTimesheetSummary();
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

  private void LoadEditableTimesheets(EmployeeTimesheetSummary summary)
  {
    ClearEditableTimesheets();

    foreach (AttendanceTimesheetSummary timesheet in summary.Timesheets)
    {
      EditableTimesheetEntry entry = EditableTimesheetEntry.FromSummary(timesheet);
      entry.PropertyChanged += OnEditableTimesheetPropertyChanged;
      EditableTimesheets.Add(entry);
    }

    OnPropertyChanged(nameof(EditableTimesheets));
    SelectedEditableTimesheet = EditableTimesheets.FirstOrDefault();
    UpdateCanSaveTimesheets();
  }

  private void ClearEditableTimesheets()
  {
    foreach (EditableTimesheetEntry entry in EditableTimesheets)
    {
      entry.PropertyChanged -= OnEditableTimesheetPropertyChanged;
    }

    EditableTimesheets.Clear();
    SelectedEditableTimesheet = null;
    UpdateCanSaveTimesheets();
  }

  private void OnEditableTimesheetPropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    UpdateCanSaveTimesheets();
  }

  private void UpdateSummaryEntry(AttendanceTimesheetSummary updatedTimesheet)
  {
    EmployeeTimesheetSummary? summary = Timesheets
      .FirstOrDefault(item => item.EmployeeId == updatedTimesheet.EmployeeId);

    if (summary is null)
    {
      return;
    }

    AttendanceTimesheetSummary? existing = summary.Timesheets
      .FirstOrDefault(item => item.Id == updatedTimesheet.Id);

    if (existing is null)
    {
      return;
    }

    existing.TimeIn1 = updatedTimesheet.TimeIn1;
    existing.TimeOut1 = updatedTimesheet.TimeOut1;
    existing.TimeIn2 = updatedTimesheet.TimeIn2;
    existing.TimeOut2 = updatedTimesheet.TimeOut2;
    existing.EntryDate = updatedTimesheet.EntryDate;
    existing.IsEdited = updatedTimesheet.IsEdited;
  }

  private void SyncSelectedTimesheetSummary()
  {
    if (!IsDetailsOpen || SelectedEmployeeTimesheet is null)
    {
      return;
    }

    EmployeeTimesheetSummary? updated = Timesheets
      .FirstOrDefault(item => item.EmployeeId == SelectedEmployeeTimesheet.EmployeeId);

    if (updated is null || ReferenceEquals(updated, SelectedEmployeeTimesheet))
    {
      return;
    }

    SelectedEmployeeTimesheet = updated;
  }

  private static void UpdateTimesheetRowIndexes(EmployeeTimesheetSummary summary)
  {
    for (int i = 0; i < summary.Timesheets.Count; i++)
    {
      summary.Timesheets[i].RowIndex = i;
    }
  }

  private void UpdateCanSaveTimesheets()
  {
    OnPropertyChanged(nameof(CanSaveTimesheets));
  }

  private void SetCurrentPayslipPeriod(DateOnly? referenceDate = null)
  {
    DateOnly date = referenceDate ?? DateOnly.FromDateTime(DateTime.Today);
    (DateOnly startDate, DateOnly endDate) = CalculatePayslipPeriod(date);
    StartDate = startDate;
    EndDate = endDate;
  }

  private void SetPreviousPayslipPeriod()
  {
    SetCurrentPayslipPeriod(StartDate.AddDays(-1));
  }

  private void SetNextPayslipPeriod()
  {
    SetCurrentPayslipPeriod(EndDate.AddDays(1));
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

  partial void OnStartDateChanged(DateOnly value)
  {
    OnPropertyChanged(nameof(PeriodRangeDisplay));
  }

  partial void OnEndDateChanged(DateOnly value)
  {
    OnPropertyChanged(nameof(PeriodRangeDisplay));
  }

  private void RaiseNavigationState()
  {
    void UpdateNavigationCommands()
    {
      PreviousPeriodCommand.NotifyCanExecuteChanged();
      NextPeriodCommand.NotifyCanExecuteChanged();
    }

    if (MainThread.IsMainThread)
    {
      UpdateNavigationCommands();
      return;
    }

    MainThread.BeginInvokeOnMainThread(UpdateNavigationCommands);
  }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bluewater.App.Extensions;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

public partial class PayrollViewModel : BaseViewModel
{
  private const int PageSize = 100;
  private readonly IPayrollApiService payrollApiService;
  private bool hasInitialized;

  public PayrollViewModel(
    IPayrollApiService payrollApiService,
    IActivityTraceService activityTraceService,
    IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
    this.payrollApiService = payrollApiService;
    SetCurrentPayslipPeriod();
    EditablePayroll = CreateNewPayroll();
  }

  public ObservableCollection<PayrollSummary> Payrolls { get; } = new();

  [ObservableProperty]
  public partial DateOnly StartDate { get; set; }

  [ObservableProperty]
  public partial DateOnly EndDate { get; set; }

  public string PeriodRangeDisplay => $"{StartDate:MMMM dd} - {EndDate:MMMM dd}";

  [ObservableProperty]
  public partial string? ChargingFilter { get; set; }

  [ObservableProperty]
  public partial PayrollSummary? SelectedPayroll { get; set; }

  [ObservableProperty]
  public partial PayrollSummary EditablePayroll { get; set; }

  public override async Task InitializeAsync()
  {
    if (hasInitialized)
    {
      return;
    }

    hasInitialized = true;
    await TraceCommandAsync(nameof(InitializeAsync));
    await LoadPayrollsAsync();
  }

  [RelayCommand]
  private async Task RefreshAsync()
  {
    await TraceCommandAsync(nameof(RefreshAsync));
    await LoadPayrollsAsync();
  }

  [RelayCommand(CanExecute = nameof(CanChangePeriod))]
  private async Task PreviousPeriodAsync()
  {
    SetPreviousPayslipPeriod();
    await TraceCommandAsync(nameof(PreviousPeriodAsync));
    await LoadPayrollsAsync();
  }

  [RelayCommand(CanExecute = nameof(CanChangePeriod))]
  private async Task NextPeriodAsync()
  {
    SetNextPayslipPeriod();
    await TraceCommandAsync(nameof(NextPeriodAsync));
    await LoadPayrollsAsync();
  }

  private bool CanChangePeriod() => !IsBusy;

  [RelayCommand]
  private void BeginCreatePayroll()
  {
    EditablePayroll = CreateNewPayroll();
    SelectedPayroll = null;
  }

  [RelayCommand]
  private void BeginEditPayroll(PayrollSummary? payroll)
  {
    if (payroll is null)
    {
      return;
    }

    SelectedPayroll = payroll;
    EditablePayroll = payroll;
  }

  [RelayCommand]
  private async Task SavePayrollAsync()
  {
    bool isNew = EditablePayroll.Id == Guid.Empty;

    try
    {
      IsBusy = true;

      if (isNew)
      {
        Guid? newId = await payrollApiService.CreatePayrollAsync(EditablePayroll);

        if (newId.HasValue)
        {
          EditablePayroll.Id = newId.Value;
          PayrollSummary? created = await payrollApiService.GetPayrollByIdAsync(newId.Value);
          PayrollSummary payroll = created ?? EditablePayroll;
          payroll.RowIndex = Payrolls.Count;
          Payrolls.Add(payroll);
          EditablePayroll = payroll;
        }
        else
        {
          EditablePayroll.RowIndex = Payrolls.Count;
          Payrolls.Add(EditablePayroll);
        }
      }
      else
      {
        PayrollSummary? updated = await payrollApiService.UpdatePayrollAsync(EditablePayroll);
        PayrollSummary result = updated ?? EditablePayroll;
        int index = FindPayrollIndex(result.Id);
        if (index >= 0)
        {
          result.RowIndex = index;
          Payrolls[index] = result;
        }
        else
        {
          result.RowIndex = Payrolls.Count;
          Payrolls.Add(result);
        }

        EditablePayroll = result;
      }

      UpdatePayrollRowIndexes();

      await TraceCommandAsync(nameof(SavePayrollAsync), EditablePayroll.Id);
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, isNew ? "Creating payroll" : "Updating payroll");
    }
    finally
    {
      IsBusy = false;
    }
  }

  [RelayCommand]
  private async Task DeletePayrollAsync(PayrollSummary? payroll)
  {
    if (payroll is null)
    {
      return;
    }

    try
    {
      IsBusy = true;

      bool deleted = await payrollApiService.DeletePayrollAsync(payroll.Id);

      if (deleted)
      {
        Payrolls.Remove(payroll);
        UpdatePayrollRowIndexes();
        await TraceCommandAsync(nameof(DeletePayrollAsync), payroll.Id);
      }
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Deleting payroll");
    }
    finally
    {
      IsBusy = false;
    }
  }

  public override void IsBusyChanged(bool isBusy)
  {
    base.IsBusyChanged(isBusy);
    RaiseNavigationState();
  }

  private async Task LoadPayrollsAsync()
  {
    try
    {
      IsBusy = true;

      var payrolls = new List<PayrollSummary>();
      int skip = 0;

      while (true)
      {
        IReadOnlyList<PayrollSummary> page = await payrollApiService
          .GetPayrollsAsync(StartDate, EndDate, ChargingFilter, skip, PageSize)
          .ConfigureAwait(false);

        if (page.Count == 0)
        {
          break;
        }

        payrolls.AddRange(page);

        if (page.Count < PageSize)
        {
          break;
        }

        skip += PageSize;
      }

      Payrolls.Clear();
      foreach (PayrollSummary payroll in payrolls)
      {
        payroll.RowIndex = Payrolls.Count;
        Payrolls.Add(payroll);
      }
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Loading payrolls");
    }
    finally
    {
      IsBusy = false;
    }
  }

  private static PayrollSummary CreateNewPayroll()
  {
    return new PayrollSummary
    {
      Id = Guid.Empty,
      Date = DateOnly.FromDateTime(DateTime.Today),
      RowIndex = 0
    };
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

  private int FindPayrollIndex(Guid payrollId)
  {
    for (int i = 0; i < Payrolls.Count; i++)
    {
      if (Payrolls[i].Id == payrollId)
      {
        return i;
      }
    }

    return -1;
  }

  private void UpdatePayrollRowIndexes()
  {
    Payrolls.UpdateRowIndexes();
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

    if (Microsoft.Maui.ApplicationModel.MainThread.IsMainThread)
    {
      UpdateNavigationCommands();
      return;
    }

    Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(UpdateNavigationCommands);
  }
}

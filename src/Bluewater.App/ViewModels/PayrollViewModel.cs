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

namespace Bluewater.App.ViewModels;

public partial class PayrollViewModel : BaseViewModel
{
  private readonly IPayrollApiService payrollApiService;
  private bool hasInitialized;

  public PayrollViewModel(
    IPayrollApiService payrollApiService,
    IActivityTraceService activityTraceService,
    IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
    this.payrollApiService = payrollApiService;
    StartDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(-1));
    EndDate = DateOnly.FromDateTime(DateTime.Today);
    EditablePayroll = CreateNewPayroll();
  }

  public ObservableCollection<PayrollSummary> Payrolls { get; } = new();

  [ObservableProperty]
  public partial DateOnly StartDate { get; set; }

  [ObservableProperty]
  public partial DateOnly EndDate { get; set; }

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
          Payrolls.Add(payroll);
          EditablePayroll = payroll;
        }
        else
        {
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
          Payrolls[index] = result;
        }
        else
        {
          Payrolls.Add(result);
        }

        EditablePayroll = result;
      }

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

  private async Task LoadPayrollsAsync()
  {
    try
    {
      IsBusy = true;

      IReadOnlyList<PayrollSummary> payrolls = await payrollApiService
        .GetPayrollsAsync(StartDate, EndDate, chargingName: ChargingFilter)
        .ConfigureAwait(false);

      Payrolls.Clear();
      foreach (PayrollSummary payroll in payrolls)
      {
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
      Date = DateOnly.FromDateTime(DateTime.Today)
    };
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
}

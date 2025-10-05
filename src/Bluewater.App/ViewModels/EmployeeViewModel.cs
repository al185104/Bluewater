using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

public partial class EmployeeViewModel : BaseViewModel
{
  private readonly IEmployeeApiService employeeApiService;
  private bool hasInitialized;

  public EmployeeViewModel(
    IEmployeeApiService employeeApiService,
    IActivityTraceService activityTraceService,
    IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
    this.employeeApiService = employeeApiService;
  }

  public ObservableCollection<EmployeeSummary> Employees { get; } = new();

  [RelayCommand]
  private async Task EditEmployeeAsync(EmployeeSummary? employee)
  {
    if (employee is null)
    {
      return;
    }

    await TraceCommandAsync(nameof(EditEmployeeAsync), employee.Id);
  }

  [RelayCommand]
  private async Task DeleteEmployeeAsync(EmployeeSummary? employee)
  {
    if (employee is null)
    {
      return;
    }

    await TraceCommandAsync(nameof(DeleteEmployeeAsync), employee.Id);
  }

  public override async Task InitializeAsync()
  {
    if (IsBusy || hasInitialized)
    {
      return;
    }

    try
    {
      IsBusy = true;
      Employees.Clear();
      IReadOnlyList<EmployeeSummary> employees = await employeeApiService.GetEmployeesAsync();

      foreach (EmployeeSummary employee in employees
        .OrderBy(e => e.LastName ?? string.Empty)
        .ThenBy(e => e.FirstName ?? string.Empty))
      {
        Employees.Add(employee);
      }

      hasInitialized = true;
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Loading employees");
    }
    finally
    {
      IsBusy = false;
    }
  }
}

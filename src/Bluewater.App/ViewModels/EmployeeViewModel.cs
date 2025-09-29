using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;

namespace Bluewater.App.ViewModels;

public partial class EmployeeViewModel : BaseViewModel
{
  private readonly IEmployeeApiService employeeApiService;
  private bool hasInitialized;

  public EmployeeViewModel(IEmployeeApiService employeeApiService)
  {
    this.employeeApiService = employeeApiService;
  }

  public ObservableCollection<EmployeeSummary> Employees { get; } = new();

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
      Debug.WriteLine($"Failed to load employees: {ex.Message}");
    }
    finally
    {
      IsBusy = false;
    }
  }
}

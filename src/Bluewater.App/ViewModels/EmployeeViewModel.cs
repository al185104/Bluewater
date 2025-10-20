using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

public partial class EmployeeViewModel : BaseViewModel
{
  private readonly IEmployeeApiService employeeApiService;
  private bool hasInitialized;

  [ObservableProperty]
  public partial EditableEmployee? EditableEmployee { get; set; }

  [ObservableProperty]
  public partial bool IsEditorOpen { get; set; }

  [ObservableProperty]
  public partial string EditorTitle { get; set; } = null!;

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

    EditableEmployee = EditableEmployee.FromSummary(employee);
    EditorTitle = $"Edit {EditableEmployee.FullName}";
    IsEditorOpen = true;

    await TraceCommandAsync(nameof(EditEmployeeAsync), employee.Id);
  }

  [RelayCommand]
  private void UpdateEmployee()
  {
    if (EditableEmployee is null)
    {
      return;
    }

    EmployeeSummary? existing = Employees.FirstOrDefault(e => e.Id == EditableEmployee.Id);

    if (existing is null)
    {
      IsEditorOpen = false;
      return;
    }

    int index = Employees.IndexOf(existing);
    EmployeeSummary updated = EditableEmployee.ToSummary(existing.RowIndex);
    Employees[index] = updated;

    IsEditorOpen = false;
    EditorTitle = string.Empty;
    EditableEmployee = null;
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

      int index = 0;

      foreach (EmployeeSummary employee in employees
        .OrderBy(e => e.LastName ?? string.Empty)
        .ThenBy(e => e.FirstName ?? string.Empty))
      {
        employee.RowIndex = index++;
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Bluewater.App.Helpers;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "CommunityToolkit.Mvvm RelayCommand attributes are platform-agnostic in .NET MAUI view models.")]
public partial class EmployeeViewModel : BaseViewModel
{
  private const string DefaultPrimaryActionText = "Save Employee";
  private const int PageSize = 100;
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
  public ObservableCollection<int> PageNumbers { get; } = new();

  [ObservableProperty]
  public partial int CurrentPage { get; set; } = 1;

  [ObservableProperty]
  public partial int TotalCount { get; set; }

  public int TotalPages => TotalCount == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

  public bool HasPagination => TotalPages > 0;

  [ObservableProperty]
  public partial EditableEmployee? EditableEmployee { get; set; }

  [ObservableProperty]
  public partial bool IsEditorOpen { get; set; }

  [ObservableProperty]
  public partial string EditorTitle { get; set; } = string.Empty;

  [ObservableProperty]
  public partial string EditorPrimaryActionText { get; set; } = DefaultPrimaryActionText;

  [ObservableProperty]
  public partial string ImportStatusMessage { get; set; } = string.Empty;

  [ObservableProperty]
  public partial bool HasImportStatusMessage { get; set; }

  partial void OnImportStatusMessageChanged(string value)
  {
    HasImportStatusMessage = !string.IsNullOrWhiteSpace(value);
  }

  [RelayCommand]
  private async Task EditEmployeeAsync(EmployeeSummary? employee)
  {
    if (employee is null)
    {
      return;
    }

    EditableEmployee = EditableEmployee.FromSummary(employee);
    EditorTitle = $"Edit {EditableEmployee.FullName}";
    EditorPrimaryActionText = "Update Employee";
    IsEditorOpen = true;

    await TraceCommandAsync(nameof(EditEmployeeAsync), employee.Id);
  }

  [RelayCommand]
  private async Task UpdateEmployeeAsync()
  {
    if (EditableEmployee is null)
    {
      return;
    }

    EmployeeSummary? existing = Employees.FirstOrDefault(e => e.Id == EditableEmployee.Id);

    if (existing is null)
    {
      CloseEditor();
      return;
    }

    bool updateSucceeded = false;

    try
    {
      IsBusy = true;

      UpdateEmployeeRequestDto request = EditableEmployee.ToUpdateRequest(existing);
      EmployeeSummary? updated = await employeeApiService.UpdateEmployeeAsync(request, existing);

      EmployeeSummary result = updated ?? EditableEmployee.ToSummary(existing.RowIndex);
      int index = Employees.IndexOf(existing);
      Employees[index] = result;
      updateSucceeded = true;

      await TraceCommandAsync(nameof(UpdateEmployeeAsync), result.Id);
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Updating employee");
    }
    finally
    {
      IsBusy = false;
    }

    if (updateSucceeded)
    {
      CloseEditor();
    }
  }

  [RelayCommand]
  private void CloseEditor()
  {
    IsEditorOpen = false;
    EditorTitle = string.Empty;
    EditorPrimaryActionText = DefaultPrimaryActionText;
    EditableEmployee = null;
  }

  [RelayCommand]
  private async Task DeleteEmployeeAsync(EmployeeSummary? employee)
  {
    if (employee is null || IsBusy)
    {
      return;
    }

    bool confirmed = await MainThread.InvokeOnMainThreadAsync(async () =>
    {
      Page? mainPage = null;
      if (Application.Current?.Windows?.Count > 0)
      {
        mainPage = Application.Current.Windows[0].Page;
      }

      if (mainPage is null)
      {
        return false;
      }

      string employeeName = string.IsNullOrWhiteSpace(employee.FullName)
        ? "this employee"
        : employee.FullName;

      return await mainPage.DisplayAlert(
        "Delete Employee",
        $"Are you sure you want to delete {employeeName}?",
        "Delete",
        "Cancel");
    }).ConfigureAwait(false);

    if (!confirmed)
    {
      return;
    }

    bool deleted = false;

    try
    {
      IsBusy = true;
      deleted = await employeeApiService.DeleteEmployeeAsync(employee.Id).ConfigureAwait(false);
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Deleting employee");
    }
    finally
    {
      IsBusy = false;
    }

    if (!deleted)
    {
      return;
    }

    await LoadEmployeesAsync(forceRefresh: true).ConfigureAwait(false);
    await TraceCommandAsync(nameof(DeleteEmployeeAsync), employee.Id).ConfigureAwait(false);
  }

  [RelayCommand]
  private async Task ImportEmployeesAsync()
  {
    if (IsBusy)
    {
      return;
    }

    ImportStatusMessage = string.Empty;

    bool importedAny = false;
    int successCount = 0;
    int total = 0;
    Exception? lastError = null;

    try
    {
      PickOptions options = new()
      {
        PickerTitle = "Select employee CSV file",
        FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
          [DevicePlatform.iOS] = new[] { "public.comma-separated-values-text", "public.text" },
          [DevicePlatform.Android] = new[] { "text/csv", "text/comma-separated-values" },
          [DevicePlatform.WinUI] = new[] { ".csv" },
          [DevicePlatform.MacCatalyst] = new[] { "public.comma-separated-values-text", "public.text" }
        })
      };

      FileResult? file = await FilePicker.Default.PickAsync(options);

      if (file is null)
      {
        return;
      }

      await using Stream stream = await file.OpenReadAsync();
      IReadOnlyList<CreateEmployeeRequestDto> employees = await EmployeeCsvImporter.ParseAsync(stream);
      total = employees.Count;

      if (total == 0)
      {
        ImportStatusMessage = "No employees were found in the selected file.";
        return;
      }

      IsBusy = true;

      foreach (CreateEmployeeRequestDto employee in employees)
      {
        try
        {
          if (await employeeApiService.CreateEmployeeAsync(employee))
          {
            successCount++;
          }
        }
        catch (Exception ex)
        {
          lastError = ex;
        }
      }

      importedAny = successCount > 0;

      ImportStatusMessage = successCount switch
      {
        0 => "No employees were imported.",
        1 when total == 1 => "Imported 1 employee successfully.",
        _ when successCount == total => $"Imported {successCount} employees successfully.",
        _ => $"Imported {successCount} of {total} employees successfully."
      };

      await TraceCommandAsync(nameof(ImportEmployeesAsync), new { count = successCount, total, file = file.FileName });

      if (lastError is not null && successCount != total)
      {
        ExceptionHandlingService.Handle(lastError, "Importing employees");
      }
    }
    catch (OperationCanceledException)
    {
      // The user cancelled the picker.
    }
    catch (FormatException ex)
    {
      ImportStatusMessage = ex.Message;
      ExceptionHandlingService.Handle(ex, "Importing employees");
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Importing employees");
    }
    finally
    {
      if (IsBusy)
      {
        IsBusy = false;
      }
    }

    if (importedAny)
    {
      await LoadEmployeesAsync(forceRefresh: true);
    }
  }

  [RelayCommand]
  private async Task GoToPageAsync(int page)
  {
    if (IsBusy || page < 1 || page == CurrentPage)
    {
      return;
    }

    if (TotalPages > 0 && page > TotalPages)
    {
      return;
    }

    CurrentPage = page;
    await LoadEmployeesAsync(forceRefresh: true).ConfigureAwait(false);
  }

  public override Task InitializeAsync()
  {
    return LoadEmployeesAsync();
  }

  private async Task LoadEmployeesAsync(bool forceRefresh = false)
  {
    if (IsBusy)
    {
      return;
    }

    if (!forceRefresh && hasInitialized)
    {
      return;
    }

    try
    {
      IsBusy = true;

      int skip = (CurrentPage - 1) * PageSize;
      PagedResult<EmployeeSummary> page = await employeeApiService.GetEmployeesAsync(skip, PageSize);

      UpdatePagination(page.TotalCount);
      Employees.Clear();

      int index = 0;

      foreach (EmployeeSummary employee in page.Items
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

  private void UpdatePagination(int totalCount)
  {
    TotalCount = totalCount;
    OnPropertyChanged(nameof(TotalPages));
    OnPropertyChanged(nameof(HasPagination));

    PageNumbers.Clear();
    for (int page = 1; page <= TotalPages; page++)
    {
      PageNumbers.Add(page);
    }

    if (TotalPages == 0)
    {
      CurrentPage = 1;
    }
    else if (CurrentPage > TotalPages)
    {
      CurrentPage = TotalPages;
    }
  }
}

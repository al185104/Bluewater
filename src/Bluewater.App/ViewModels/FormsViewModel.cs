using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using Bluewater.App.Extensions;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

public partial class FormsViewModel : BaseViewModel
{
  private readonly IDeductionApiService deductionApiService;
  private readonly IEmployeeApiService employeeApiService;
  private readonly List<DeductionSummary> allDeductions = [];
  private bool hasInitialized;

  public FormsViewModel(
    IDeductionApiService deductionApiService,
    IEmployeeApiService employeeApiService,
    IActivityTraceService activityTraceService,
    IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
    this.deductionApiService = deductionApiService;
    this.employeeApiService = employeeApiService;

    EditableDeduction = CreateNewDeduction();
    LoadDeductionTypes();
  }

  public ObservableCollection<DeductionSummary> Deductions { get; } = new();
  public ObservableCollection<EmployeeSummary> Employees { get; } = new();
  public ObservableCollection<DeductionTypeOption> DeductionTypes { get; } = new();

  [ObservableProperty]
  public partial DeductionSummary? SelectedDeduction { get; set; }

  [ObservableProperty]
  public partial DeductionSummary EditableDeduction { get; set; }

  [ObservableProperty]
  public partial TenantDto TenantFilter { get; set; } = TenantDto.Maribago;

  [ObservableProperty]
  public partial string SearchText { get; set; } = string.Empty;

  [ObservableProperty]
  public partial EmployeeSummary? SelectedEmployee { get; set; }

  [ObservableProperty]
  public partial DeductionTypeOption? SelectedDeductionType { get; set; }

  public override async Task InitializeAsync()
  {
    if (hasInitialized)
    {
      return;
    }

    hasInitialized = true;
    await TraceCommandAsync(nameof(InitializeAsync));
    await LoadDeductionsAsync();
  }

  [RelayCommand]
  private async Task RefreshAsync()
  {
    await TraceCommandAsync(nameof(RefreshAsync));
    await LoadDeductionsAsync();
  }

  [RelayCommand]
  private void BeginCreateDeduction()
  {
    EditableDeduction = CreateNewDeduction();
    SelectedDeduction = null;
    SelectedDeductionType = null;

    if (SelectedEmployee is not null)
    {
      EditableDeduction.EmpId = SelectedEmployee.Id;
      EditableDeduction.Name = SelectedEmployee.FullName;
    }
  }

  [RelayCommand]
  private async Task SaveDeductionAsync()
  {
    if (SelectedEmployee is null)
    {
      return;
    }

    if (SelectedDeductionType is null || SelectedDeductionType.Value == DeductionTypeDto.NotSet)
    {
      return;
    }

    try
    {
      IsBusy = true;

      EditableDeduction.EmpId = SelectedEmployee.Id;
      EditableDeduction.Name = SelectedEmployee.FullName;
      EditableDeduction.Type = SelectedDeductionType.Value;
      EditableDeduction.Status = ApplicationStatusDto.Pending;

      DeductionSummary? saved = await deductionApiService.CreateDeductionAsync(EditableDeduction);
      DeductionSummary result = saved ?? CloneDeduction(EditableDeduction);

      if (result.Id == Guid.Empty)
      {
        result.Id = Guid.NewGuid();
      }

      int existingIndex = allDeductions.FindIndex(item => item.Id == result.Id);
      if (existingIndex >= 0)
      {
        allDeductions[existingIndex] = result;
      }
      else
      {
        allDeductions.Add(result);
      }

      ApplyDeductionFilter();
      await TraceCommandAsync(nameof(SaveDeductionAsync), result.Id);

      EditableDeduction = CreateNewDeduction();
      EditableDeduction.EmpId = SelectedEmployee.Id;
      EditableDeduction.Name = SelectedEmployee.FullName;
      SelectedDeductionType = null;
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Creating deduction");
    }
    finally
    {
      IsBusy = false;
    }
  }

  partial void OnSearchTextChanged(string value)
  {
    _ = LoadSearchedEmployeeAsync(value);
    ApplyDeductionFilter();
  }

  partial void OnSelectedEmployeeChanged(EmployeeSummary? value)
  {
    if (value is not null)
    {
      EditableDeduction.EmpId = value.Id;
      EditableDeduction.Name = value.FullName;
    }

    ApplyDeductionFilter();
  }

  partial void OnSelectedDeductionTypeChanged(DeductionTypeOption? value)
  {
    if (value is null)
    {
      return;
    }

    EditableDeduction.Type = value.Value;
  }

  private async Task LoadDeductionsAsync()
  {
    try
    {
      IsBusy = true;

      IReadOnlyList<DeductionSummary> deductions = await deductionApiService
        .GetDeductionsAsync(tenant: TenantFilter)
        .ConfigureAwait(false);

      allDeductions.Clear();
      allDeductions.AddRange(deductions);
      ApplyDeductionFilter();
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Loading deductions");
    }
    finally
    {
      IsBusy = false;
    }
  }

  private async Task LoadSearchedEmployeeAsync(string searchText)
  {
    if (string.IsNullOrWhiteSpace(searchText))
    {
      Employees.Clear();
      SelectedEmployee = null;
      return;
    }

    try
    {
      const int pageSize = 100;
      int skip = 0;
      string search = searchText.Trim();
      EmployeeSummary? matchedEmployee = null;

      while (matchedEmployee is null)
      {
        PagedResult<EmployeeSummary> page = await employeeApiService.GetEmployeesAsync(skip: skip, take: pageSize);
        if (page.Items.Count == 0)
        {
          break;
        }

        matchedEmployee = page.Items.FirstOrDefault(employee =>
          employee.FullName.Contains(search, StringComparison.OrdinalIgnoreCase)
          || (employee.Position?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
          || (employee.Department?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
          || (employee.Section?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));

        skip += page.Items.Count;
        if (page.Items.Count < pageSize)
        {
          break;
        }
      }

      Employees.Clear();
      if (matchedEmployee is not null)
      {
        Employees.Add(matchedEmployee);
      }

      SelectedEmployee = matchedEmployee;
      ApplyDeductionFilter();
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Searching employee");
    }
  }

  private void ApplyDeductionFilter()
  {
    IEnumerable<DeductionSummary> filteredDeductions = allDeductions;
    if (SelectedEmployee is not null)
    {
      filteredDeductions = filteredDeductions.Where(deduction => deduction.EmpId == SelectedEmployee.Id);
    }
    else if (!string.IsNullOrWhiteSpace(SearchText))
    {
      string search = SearchText.Trim();
      filteredDeductions = filteredDeductions.Where(deduction =>
        deduction.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
    }

    Deductions.Clear();
    foreach (DeductionSummary deduction in filteredDeductions.OrderByDescending(item => item.StartDate))
    {
      Deductions.Add(deduction);
    }

    Deductions.UpdateRowIndexes();
  }

  private void LoadDeductionTypes()
  {
    DeductionTypes.Clear();

    foreach (DeductionTypeDto deductionType in Enum.GetValues<DeductionTypeDto>())
    {
      if (deductionType == DeductionTypeDto.NotSet)
      {
        continue;
      }

      DeductionTypes.Add(new DeductionTypeOption
      {
        Value = deductionType,
        Description = GetEnumDescription(deductionType)
      });
    }
  }

  private static string GetEnumDescription(DeductionTypeDto value)
  {
    MemberInfo? member = typeof(DeductionTypeDto).GetMember(value.ToString()).FirstOrDefault();
    return member is not null
      && Attribute.GetCustomAttribute(member, typeof(DescriptionAttribute)) is DescriptionAttribute description
        ? description.Description
        : value.ToString();
  }

  private static DeductionSummary CreateNewDeduction()
  {
    return new DeductionSummary
    {
      Id = Guid.Empty,
      StartDate = DateTime.Today,
      EndDate = DateTime.Today,
      Status = ApplicationStatusDto.Pending,
      RowIndex = 0
    };
  }

  private static DeductionSummary CloneDeduction(DeductionSummary deduction)
  {
    return new DeductionSummary
    {
      Id = deduction.Id,
      EmpId = deduction.EmpId,
      Name = deduction.Name,
      Type = deduction.Type,
      TotalAmount = deduction.TotalAmount,
      MonthlyAmortization = deduction.MonthlyAmortization,
      RemainingBalance = deduction.RemainingBalance,
      NoOfMonths = deduction.NoOfMonths,
      StartDate = deduction.StartDate,
      EndDate = deduction.EndDate,
      Remarks = deduction.Remarks,
      Status = deduction.Status,
      RowIndex = deduction.RowIndex
    };
  }
}

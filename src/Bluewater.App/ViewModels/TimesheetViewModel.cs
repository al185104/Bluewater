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

public partial class TimesheetViewModel : BaseViewModel
{
  private readonly ITimesheetApiService timesheetApiService;
  private readonly IEmployeeApiService employeeApiService;
  private readonly Dictionary<Guid, TenantDto> employeeTenants = new();
  private bool hasInitialized;

  public TimesheetViewModel(
    ITimesheetApiService timesheetApiService,
    IEmployeeApiService employeeApiService,
    IActivityTraceService activityTraceService,
    IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
    this.timesheetApiService = timesheetApiService;
    this.employeeApiService = employeeApiService;
    StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-7));
    EndDate = DateOnly.FromDateTime(DateTime.Today);
    EditableTimesheet = CreateNewTimesheet();
  }

  public ObservableCollection<AttendanceTimesheetSummary> Timesheets { get; } = new();

  [ObservableProperty]
  public partial Guid? EmployeeFilter { get; set; }

  [ObservableProperty]
  public partial DateOnly StartDate { get; set; }

  [ObservableProperty]
  public partial DateOnly EndDate { get; set; }

  [ObservableProperty]
  public partial AttendanceTimesheetSummary? SelectedTimesheet { get; set; }

  [ObservableProperty]
  public partial AttendanceTimesheetSummary EditableTimesheet { get; set; }

  public override async Task InitializeAsync()
  {
    if (hasInitialized)
    {
      return;
    }

    hasInitialized = true;
    await TraceCommandAsync(nameof(InitializeAsync));
    await EnsureDefaultEmployeeFilterAsync().ConfigureAwait(false);
    await LoadTimesheetsAsync();
  }

  [RelayCommand]
  private async Task RefreshAsync()
  {
    await TraceCommandAsync(nameof(RefreshAsync));
    await LoadTimesheetsAsync();
  }

  [RelayCommand]
  private void BeginCreateTimesheet()
  {
    EditableTimesheet = CreateNewTimesheet();
    if (EmployeeFilter.HasValue)
    {
      EditableTimesheet.EmployeeId = EmployeeFilter.Value;
    }
    SelectedTimesheet = null;
  }

  [RelayCommand]
  private void BeginEditTimesheet(AttendanceTimesheetSummary? timesheet)
  {
    if (timesheet is null)
    {
      return;
    }

    SelectedTimesheet = timesheet;
    EditableTimesheet = CloneTimesheet(timesheet);
  }

  [RelayCommand]
  private async Task SaveTimesheetAsync()
  {
    if (EditableTimesheet.EmployeeId == Guid.Empty)
    {
      return;
    }

    bool isNew = Timesheets.All(item => item.Id != EditableTimesheet.Id);
    AttendanceTimesheetSummary item = CloneTimesheet(EditableTimesheet);

    if (isNew)
    {
      item.Id = Guid.NewGuid();
      Timesheets.Add(item);
    }
    else
    {
      int index = FindTimesheetIndex(item.Id);
      if (index >= 0)
      {
        Timesheets[index] = item;
      }
      else
      {
        Timesheets.Add(item);
      }
    }

    EditableTimesheet = item;
    await TraceCommandAsync(nameof(SaveTimesheetAsync), item.Id);
  }

  [RelayCommand]
  private async Task DeleteTimesheetAsync(AttendanceTimesheetSummary? timesheet)
  {
    if (timesheet is null)
    {
      return;
    }

    if (Timesheets.Remove(timesheet))
    {
      await TraceCommandAsync(nameof(DeleteTimesheetAsync), timesheet.Id);
    }
  }

  private async Task LoadTimesheetsAsync()
  {
    if (!EmployeeFilter.HasValue || EmployeeFilter.Value == Guid.Empty)
    {
      Timesheets.Clear();
      return;
    }

    try
    {
      IsBusy = true;

      TenantDto tenant = GetEmployeeTenant(EmployeeFilter.Value);

      IReadOnlyList<AttendanceTimesheetSummary> timesheets = await timesheetApiService
        .GetTimesheetsAsync(EmployeeFilter.Value, StartDate, EndDate, tenant)
        .ConfigureAwait(false);

      Timesheets.Clear();
      foreach (AttendanceTimesheetSummary timesheet in timesheets)
      {
        Timesheets.Add(CloneTimesheet(timesheet));
      }
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

  private async Task EnsureDefaultEmployeeFilterAsync()
  {
    if (EmployeeFilter.HasValue && EmployeeFilter.Value != Guid.Empty)
    {
      return;
    }

    try
    {
      IReadOnlyList<EmployeeSummary> employees = await employeeApiService
        .GetEmployeesAsync(take: 1)
        .ConfigureAwait(false);

      Guid? defaultEmployeeId = employees.FirstOrDefault()?.Id;

      if (defaultEmployeeId.HasValue && defaultEmployeeId.Value != Guid.Empty)
      {
        EmployeeSummary? defaultEmployee = employees.FirstOrDefault(summary => summary?.Id == defaultEmployeeId.Value);
        if (defaultEmployee is not null)
        {
          employeeTenants[defaultEmployee.Id] = ConvertTenant(defaultEmployee.Tenant);
        }
        EmployeeFilter = defaultEmployeeId.Value;
      }
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Loading default employee");
    }
  }

  private TenantDto GetEmployeeTenant(Guid employeeId)
  {
    if (employeeTenants.TryGetValue(employeeId, out TenantDto tenant))
    {
      return tenant;
    }

    return TenantDto.Maribago;
  }

  private static TenantDto ConvertTenant(Bluewater.Core.EmployeeAggregate.Enum.Tenant tenant)
  {
    return Enum.TryParse<TenantDto>(tenant.ToString(), out TenantDto tenantDto)
      ? tenantDto
      : TenantDto.Maribago;
  }

  private static AttendanceTimesheetSummary CreateNewTimesheet()
  {
    return new AttendanceTimesheetSummary
    {
      Id = Guid.NewGuid(),
      EntryDate = DateOnly.FromDateTime(DateTime.Today)
    };
  }

  private static AttendanceTimesheetSummary CloneTimesheet(AttendanceTimesheetSummary timesheet)
  {
    return new AttendanceTimesheetSummary
    {
      Id = timesheet.Id,
      EmployeeId = timesheet.EmployeeId,
      TimeIn1 = timesheet.TimeIn1,
      TimeOut1 = timesheet.TimeOut1,
      TimeIn2 = timesheet.TimeIn2,
      TimeOut2 = timesheet.TimeOut2,
      EntryDate = timesheet.EntryDate,
      IsEdited = timesheet.IsEdited
    };
  }

  private int FindTimesheetIndex(Guid timesheetId)
  {
    for (int i = 0; i < Timesheets.Count; i++)
    {
      if (Timesheets[i].Id == timesheetId)
      {
        return i;
      }
    }

    return -1;
  }
}

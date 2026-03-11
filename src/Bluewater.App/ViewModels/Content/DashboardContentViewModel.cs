using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bluewater.App.Helpers;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using Bluewater.Core.EmployeeAggregate.Enum;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels.Content;

public partial class DashboardContentViewModel : BaseViewModel
{
  private const int BatchSize = 250;
  private readonly IEmployeeApiService employeeApiService;
  private readonly IPayrollApiService payrollApiService;
  private readonly ILeaveApiService leaveApiService;
  private readonly IHolidayApiService holidayApiService;
  private readonly IDepartmentApiService departmentApiService;
  private bool hasInitialized;

  public DashboardContentViewModel(
    IEmployeeApiService employeeApiService,
    IPayrollApiService payrollApiService,
    ILeaveApiService leaveApiService,
    IHolidayApiService holidayApiService,
    IDepartmentApiService departmentApiService,
    IActivityTraceService activityTraceService,
    IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
    this.employeeApiService = employeeApiService;
    this.payrollApiService = payrollApiService;
    this.leaveApiService = leaveApiService;
    this.holidayApiService = holidayApiService;
    this.departmentApiService = departmentApiService;

    SelectedTenant = TenantPreferences.GetSelectedTenant();
    SetCurrentPayslipPeriod();
  }

  public ObservableCollection<DepartmentSummary> DepartmentOptions { get; } = new();

  public ObservableCollection<DepartmentPayrollCostSummary> PayrollCostByDepartment { get; } = new();

  public ObservableCollection<ChargingPayrollGenerationSummary> PayrollGeneratedStatusByCharging { get; } = new();

  public ObservableCollection<HolidaySummary> UpcomingHolidays { get; } = new();

  [ObservableProperty]
  public partial TenantDto SelectedTenant { get; set; }

  [ObservableProperty]
  public partial DepartmentSummary? SelectedDepartment { get; set; }

  [ObservableProperty]
  public partial DateOnly PayrollPeriodStart { get; set; }

  [ObservableProperty]
  public partial DateOnly PayrollPeriodEnd { get; set; }

  public string PayrollPeriodDisplay => $"{PayrollPeriodStart:MMM dd, yyyy} - {PayrollPeriodEnd:MMM dd, yyyy}";

  [ObservableProperty]
  public partial DateTime Today { get; set; } = DateTime.Today;

  [ObservableProperty]
  public partial int TotalActiveEmployees { get; set; }

  [ObservableProperty]
  public partial int NewHiresInPeriod { get; set; }

  [ObservableProperty]
  public partial int EmployeesTerminatedInPeriod { get; set; }

  [ObservableProperty]
  public partial decimal TotalWorkHoursLogged { get; set; }

  [ObservableProperty]
  public partial decimal TotalOvertimeHours { get; set; }

  [ObservableProperty]
  public partial int TotalAbsences { get; set; }

  [ObservableProperty]
  public partial decimal TotalLateInstances { get; set; }

  [ObservableProperty]
  public partial decimal TotalUndertime { get; set; }

  [ObservableProperty]
  public partial decimal TotalLeaveDaysTaken { get; set; }

  [ObservableProperty]
  public partial decimal SickLeaveDays { get; set; }

  [ObservableProperty]
  public partial decimal VacationLeaveDays { get; set; }

  [ObservableProperty]
  public partial decimal OtherLeaveTypesDays { get; set; }

  public override async Task InitializeAsync()
  {
    if (hasInitialized)
    {
      return;
    }

    hasInitialized = true;
    await TraceCommandAsync(nameof(InitializeAsync));
    await LoadDepartmentOptionsAsync().ConfigureAwait(false);
    await LoadDashboardAsync().ConfigureAwait(false);
  }

  [RelayCommand]
  private async Task RefreshAsync()
  {
    await TraceCommandAsync(nameof(RefreshAsync));
    await LoadDashboardAsync().ConfigureAwait(false);
  }

  [RelayCommand(CanExecute = nameof(CanChangePeriod))]
  private async Task PreviousPayrollPeriodAsync()
  {
    SetCurrentPayslipPeriod(PayrollPeriodStart.AddDays(-1));
    await TraceCommandAsync(nameof(PreviousPayrollPeriodAsync));
    await LoadDashboardAsync().ConfigureAwait(false);
  }

  [RelayCommand(CanExecute = nameof(CanChangePeriod))]
  private async Task NextPayrollPeriodAsync()
  {
    SetCurrentPayslipPeriod(PayrollPeriodEnd.AddDays(1));
    await TraceCommandAsync(nameof(NextPayrollPeriodAsync));
    await LoadDashboardAsync().ConfigureAwait(false);
  }

  private bool CanChangePeriod() => !IsBusy;

  private async Task LoadDepartmentOptionsAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      IReadOnlyList<DepartmentSummary> departments = await departmentApiService
        .GetDepartmentsAsync(cancellationToken: cancellationToken)
        .ConfigureAwait(false);

      MainThread.BeginInvokeOnMainThread(() =>
      {
        DepartmentOptions.Clear();
        DepartmentOptions.Add(new DepartmentSummary
        {
          Id = Guid.Empty,
          Name = "All Departments",
          Description = string.Empty,
          DivisionId = Guid.Empty
        });

        foreach (DepartmentSummary department in departments.OrderBy(d => d.Name))
        {
          DepartmentOptions.Add(department);
        }

        SelectedDepartment ??= DepartmentOptions.FirstOrDefault();
      });
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Loading dashboard department options");
    }
  }

  private async Task LoadDashboardAsync(CancellationToken cancellationToken = default)
  {
    if (IsBusy)
    {
      return;
    }

    try
    {
      IsBusy = true;
      Today = DateTime.Today;

      DateOnly todayDate = DateOnly.FromDateTime(Today);
      string? selectedDepartmentName = NormalizeDepartmentName(SelectedDepartment?.Name);

      Task<IReadOnlyList<EmployeeSummary>> employeesTask = LoadAllEmployeesAsync(cancellationToken);
      Task<IReadOnlyList<PayrollSummary>> payrollTask = LoadAllPayrollsInPeriodAsync(cancellationToken);
      Task<IReadOnlyList<LeaveSummary>> leaveTask = leaveApiService.GetLeavesAsync(tenant: SelectedTenant, cancellationToken: cancellationToken);
      Task<IReadOnlyList<HolidaySummary>> holidayTask = holidayApiService.GetHolidaysAsync(cancellationToken: cancellationToken);

      await Task.WhenAll(employeesTask, payrollTask, leaveTask, holidayTask).ConfigureAwait(false);

      IReadOnlyList<EmployeeSummary> scopedEmployees = FilterByDepartment(employeesTask.Result, selectedDepartmentName);
      IReadOnlyList<PayrollSummary> scopedPayrolls = FilterByDepartment(payrollTask.Result, selectedDepartmentName);
      IReadOnlyList<LeaveSummary> scopedLeaves = FilterLeavesByEmployees(leaveTask.Result, scopedEmployees);

      ComputeEmployeeHeadcountMetrics(scopedEmployees, todayDate);
      ComputePayrollMetrics(scopedPayrolls);
      ComputeLeaveMetrics(scopedLeaves);
      ComputeDepartmentAndChargingBreakdowns(scopedPayrolls);
      ComputeUpcomingHolidays(holidayTask.Result);
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Loading dashboard information");
    }
    finally
    {
      IsBusy = false;
      RaisePeriodNavigationState();
    }
  }

  private async Task<IReadOnlyList<EmployeeSummary>> LoadAllEmployeesAsync(CancellationToken cancellationToken)
  {
    List<EmployeeSummary> employees = [];
    int skip = 0;

    while (true)
    {
      PagedResult<EmployeeSummary> page = await employeeApiService
        .GetEmployeesAsync(skip, BatchSize, cancellationToken)
        .ConfigureAwait(false);

      if (page.Items.Count == 0)
      {
        break;
      }

      employees.AddRange(page.Items);
      skip += page.Items.Count;

      if (employees.Count >= page.TotalCount)
      {
        break;
      }
    }

    return employees;
  }

  private async Task<IReadOnlyList<PayrollSummary>> LoadAllPayrollsInPeriodAsync(CancellationToken cancellationToken)
  {
    List<PayrollSummary> payrolls = [];
    int skip = 0;

    while (true)
    {
      PagedResult<PayrollSummary> page = await payrollApiService
        .GetPayrollsAsync(PayrollPeriodStart, PayrollPeriodEnd, skip: skip, take: BatchSize, tenant: SelectedTenant, cancellationToken: cancellationToken)
        .ConfigureAwait(false);

      if (page.Items.Count == 0)
      {
        break;
      }

      payrolls.AddRange(page.Items);
      skip += page.Items.Count;

      if (payrolls.Count >= page.TotalCount)
      {
        break;
      }
    }

    return payrolls;
  }

  private IReadOnlyList<EmployeeSummary> FilterByDepartment(IReadOnlyList<EmployeeSummary> source, string? department)
  {
    if (string.IsNullOrWhiteSpace(department))
    {
      return source;
    }

    return source
      .Where(employee => string.Equals(employee.Department, department, StringComparison.OrdinalIgnoreCase))
      .ToList();
  }

  private IReadOnlyList<PayrollSummary> FilterByDepartment(IReadOnlyList<PayrollSummary> source, string? department)
  {
    if (string.IsNullOrWhiteSpace(department))
    {
      return source;
    }

    return source
      .Where(payroll => string.Equals(payroll.Department, department, StringComparison.OrdinalIgnoreCase))
      .ToList();
  }

  private static IReadOnlyList<LeaveSummary> FilterLeavesByEmployees(IReadOnlyList<LeaveSummary> leaves, IReadOnlyList<EmployeeSummary> scopedEmployees)
  {
    HashSet<Guid> employeeIds = scopedEmployees
      .Select(employee => employee.Id)
      .ToHashSet();

    return leaves
      .Where(leave => leave.EmployeeId.HasValue && employeeIds.Contains(leave.EmployeeId.Value))
      .ToList();
  }

  private void ComputeEmployeeHeadcountMetrics(IReadOnlyList<EmployeeSummary> employees, DateOnly todayDate)
  {
    TotalActiveEmployees = employees.Count(employee => employee.Status == Status.Active);

    NewHiresInPeriod = employees.Count(employee =>
    {
      DateTime? dateHired = employee.EmploymentInfo.DateHired;
      return dateHired.HasValue && IsWithinPeriod(DateOnly.FromDateTime(dateHired.Value));
    });

    EmployeesTerminatedInPeriod = employees.Count(employee =>
    {
      DateTime? dateTerminated = employee.EmploymentInfo.DateTerminated;
      return dateTerminated.HasValue && IsWithinPeriod(DateOnly.FromDateTime(dateTerminated.Value));
    });

    _ = todayDate;
  }

  private void ComputePayrollMetrics(IReadOnlyList<PayrollSummary> payrolls)
  {
    TotalWorkHoursLogged = payrolls.Sum(payroll => payroll.LaborHrs);

    TotalOvertimeHours = payrolls.Sum(payroll =>
      payroll.OvertimeHrs
      + payroll.OvertimeRestDayHrs
      + payroll.OvertimeRegularHolidayHrs
      + payroll.OvertimeSpecialHolidayHrs
      + payroll.NightDiffOvertimeHrs);

    TotalAbsences = payrolls.Sum(payroll => payroll.Absences);
    TotalLateInstances = payrolls.Sum(payroll => payroll.Lates);
    TotalUndertime = payrolls.Sum(payroll => payroll.Undertime);
  }

  private void ComputeLeaveMetrics(IReadOnlyList<LeaveSummary> leaves)
  {
    IReadOnlyList<LeaveSummary> approvedLeaves = leaves
      .Where(leave => leave.Status == ApplicationStatusDto.Approved)
      .ToList();

    TotalLeaveDaysTaken = approvedLeaves.Sum(CalculateLeaveDays);
    SickLeaveDays = approvedLeaves
      .Where(leave => IsMatchingLeaveType(leave.LeaveCreditName, "sick"))
      .Sum(CalculateLeaveDays);

    VacationLeaveDays = approvedLeaves
      .Where(leave => IsMatchingLeaveType(leave.LeaveCreditName, "vacation"))
      .Sum(CalculateLeaveDays);

    OtherLeaveTypesDays = TotalLeaveDaysTaken - SickLeaveDays - VacationLeaveDays;
  }

  private void ComputeDepartmentAndChargingBreakdowns(IReadOnlyList<PayrollSummary> payrolls)
  {
    MainThread.BeginInvokeOnMainThread(() =>
    {
      PayrollCostByDepartment.Clear();
      foreach (var item in payrolls
                 .GroupBy(payroll => payroll.Department ?? "Unassigned")
                 .OrderBy(group => group.Key)
                 .Select(group => new DepartmentPayrollCostSummary
                 {
                   Department = group.Key,
                   TotalPayrollCost = group.Sum(payroll => payroll.NetAmount),
                   EmployeeCount = group.Select(payroll => payroll.EmployeeId).Where(id => id.HasValue).Distinct().Count()
                 }))
      {
        PayrollCostByDepartment.Add(item);
      }

      PayrollGeneratedStatusByCharging.Clear();
      foreach (var item in payrolls
                 .GroupBy(payroll => payroll.Charging ?? "Unassigned")
                 .OrderBy(group => group.Key)
                 .Select(group => new ChargingPayrollGenerationSummary
                 {
                   Charging = group.Key,
                   GeneratedCount = group.Count(payroll => payroll.IsSaved),
                   PendingCount = group.Count(payroll => !payroll.IsSaved)
                 }))
      {
        PayrollGeneratedStatusByCharging.Add(item);
      }
    });
  }

  private void ComputeUpcomingHolidays(IReadOnlyList<HolidaySummary> holidays)
  {
    DateTime periodStart = PayrollPeriodStart.ToDateTime(TimeOnly.MinValue);
    DateTime periodEnd = PayrollPeriodEnd.ToDateTime(TimeOnly.MaxValue);

    IReadOnlyList<HolidaySummary> scoped = holidays
      .Where(holiday => holiday.Date >= periodStart && holiday.Date <= periodEnd)
      .OrderBy(holiday => holiday.Date)
      .ToList();

    MainThread.BeginInvokeOnMainThread(() =>
    {
      UpcomingHolidays.Clear();
      foreach (HolidaySummary holiday in scoped)
      {
        UpcomingHolidays.Add(holiday);
      }
    });
  }

  private bool IsWithinPeriod(DateOnly date)
  {
    return date >= PayrollPeriodStart && date <= PayrollPeriodEnd;
  }

  private static decimal CalculateLeaveDays(LeaveSummary leave)
  {
    if (!leave.StartDate.HasValue || !leave.EndDate.HasValue)
    {
      return 0m;
    }

    DateTime start = leave.StartDate.Value.Date;
    DateTime end = leave.EndDate.Value.Date;

    if (end < start)
    {
      (start, end) = (end, start);
    }

    decimal daySpan = (decimal)(end - start).TotalDays + 1m;
    return leave.IsHalfDay ? 0.5m : daySpan;
  }

  private static bool IsMatchingLeaveType(string leaveTypeName, string token)
  {
    return leaveTypeName.Contains(token, StringComparison.OrdinalIgnoreCase);
  }

  private static string? NormalizeDepartmentName(string? departmentName)
  {
    if (string.IsNullOrWhiteSpace(departmentName) || string.Equals(departmentName, "All Departments", StringComparison.OrdinalIgnoreCase))
    {
      return null;
    }

    return departmentName;
  }

  private void SetCurrentPayslipPeriod(DateOnly? referenceDate = null)
  {
    DateOnly date = referenceDate ?? DateOnly.FromDateTime(DateTime.Today);
    (DateOnly startDate, DateOnly endDate) = CalculatePayslipPeriod(date);
    PayrollPeriodStart = startDate;
    PayrollPeriodEnd = endDate;
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

  public override void IsBusyChanged(bool isBusy)
  {
    base.IsBusyChanged(isBusy);
    RaisePeriodNavigationState();
  }

  partial void OnPayrollPeriodStartChanged(DateOnly value)
  {
    OnPropertyChanged(nameof(PayrollPeriodDisplay));
  }

  partial void OnPayrollPeriodEndChanged(DateOnly value)
  {
    OnPropertyChanged(nameof(PayrollPeriodDisplay));
  }

  partial void OnSelectedDepartmentChanged(DepartmentSummary? value)
  {
    if (!hasInitialized)
    {
      return;
    }

    _ = LoadDashboardAsync();
  }

  partial void OnSelectedTenantChanged(TenantDto value)
  {
    if (!hasInitialized)
    {
      return;
    }

    TenantPreferences.SetSelectedTenant(value);
    _ = LoadDashboardAsync();
  }

  private void RaisePeriodNavigationState()
  {
    void UpdateCommands()
    {
      PreviousPayrollPeriodCommand.NotifyCanExecuteChanged();
      NextPayrollPeriodCommand.NotifyCanExecuteChanged();
    }

    if (MainThread.IsMainThread)
    {
      UpdateCommands();
      return;
    }

    MainThread.BeginInvokeOnMainThread(UpdateCommands);
  }
}

public class DepartmentPayrollCostSummary
{
  public string Department { get; set; } = string.Empty;

  public decimal TotalPayrollCost { get; set; }

  public int EmployeeCount { get; set; }
}

public class ChargingPayrollGenerationSummary
{
  public string Charging { get; set; } = string.Empty;

  public int GeneratedCount { get; set; }

  public int PendingCount { get; set; }
}

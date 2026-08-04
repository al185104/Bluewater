using System.Collections.ObjectModel;
using Bluewater.App.Extensions;
using Bluewater.App.Helpers;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.Services;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
  private readonly IUserApiService userApiService;
  private readonly IEmployeeApiService employeeApiService;
  private readonly ILeaveApiService leaveApiService;
  private readonly IDeductionApiService deductionApiService;
  private readonly IReferenceDataService referenceDataService;
  private bool hasInitialized;

  public ProfileViewModel(
    IUserApiService userApiService,
    IEmployeeApiService employeeApiService,
    ILeaveApiService leaveApiService,
    IDeductionApiService deductionApiService,
    IReferenceDataService referenceDataService,
    IActivityTraceService activityTraceService,
    IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
    this.userApiService = userApiService;
    this.employeeApiService = employeeApiService;
    this.leaveApiService = leaveApiService;
    this.deductionApiService = deductionApiService;
    this.referenceDataService = referenceDataService;
  }

  public ObservableCollection<ProfileLeaveBalance> LeaveBalances { get; } = new();
  public ObservableCollection<ProfileFiledForm> FiledForms { get; } = new();

  [ObservableProperty]
  public partial string Username { get; set; } = string.Empty;

  [ObservableProperty]
  public partial EmployeeSummary? CurrentEmployee { get; set; }

  [ObservableProperty]
  public partial string EmptyMessage { get; set; } = "Profile details will appear after the signed-in user is linked to an employee record.";

  public string DisplayName => CurrentEmployee?.FullName ?? Username;
  public string EmailDisplay => CurrentEmployee?.EmailDisplay ?? string.Empty;
  public string PositionDisplay => CurrentEmployee?.PositionDisplay ?? string.Empty;
  public string DepartmentDisplay => CurrentEmployee?.DepartmentDisplay ?? string.Empty;
  public string SectionDisplay => CurrentEmployee?.SectionDisplay ?? string.Empty;
  public string TypeLevelDisplay => CurrentEmployee?.TypeLevelDisplay ?? string.Empty;
  public string TenantDisplay => CurrentEmployee?.Tenant.ToString() ?? string.Empty;
  public string StatusDisplay => CurrentEmployee?.Status.ToString() ?? string.Empty;
  public string MobileDisplay => CurrentEmployee?.ContactInfo.MobileNumber ?? string.Empty;
  public string DateHiredDisplay => CurrentEmployee?.EmploymentInfo.DateHired?.ToString("MMM dd, yyyy") ?? string.Empty;
  public int LeaveBalanceCount => LeaveBalances.Count;
  public int FiledFormCount => FiledForms.Count;

  public override async Task InitializeAsync()
  {
    if (hasInitialized)
    {
      return;
    }

    hasInitialized = true;
    await LoadProfileAsync();
  }

  [RelayCommand]
  private async Task RefreshAsync()
  {
    await LoadProfileAsync();
  }

  private async Task LoadProfileAsync()
  {
    try
    {
      IsBusy = true;
      await TraceCommandAsync(nameof(LoadProfileAsync)).ConfigureAwait(false);

      Username = LoginSession.CurrentUsername;
      await referenceDataService.InitializeAsync().ConfigureAwait(false);

      UserRecordDto? user = await FindCurrentUserAsync().ConfigureAwait(false);
      EmployeeSummary? employee = FindEmployee(user);

      IReadOnlyList<LeaveSummary> employeeLeaves = Array.Empty<LeaveSummary>();
      IReadOnlyList<DeductionSummary> employeeDeductions = Array.Empty<DeductionSummary>();

      if (employee is not null)
      {
        TenantDto tenant = TenantPreferences.GetSelectedTenant();
        Guid? chargingId = employee.ChargingId is { } id && id != Guid.Empty ? id : null;

        IReadOnlyList<LeaveSummary> leaves = await leaveApiService
          .GetLeavesAsync(tenant: tenant, chargingId: chargingId)
          .ConfigureAwait(false);
        IReadOnlyList<DeductionSummary> deductions = await deductionApiService
          .GetDeductionsAsync(tenant: tenant, chargingId: chargingId)
          .ConfigureAwait(false);

        employeeLeaves = leaves
          .Where(leave => leave.EmployeeId == employee.Id)
          .ToList();
        employeeDeductions = deductions
          .Where(deduction => deduction.EmpId == employee.Id)
          .ToList();
      }

      await MainThread.InvokeOnMainThreadAsync(() =>
      {
        CurrentEmployee = employee;
        ReplaceLeaveBalances(BuildLeaveBalances(employeeLeaves));
        ReplaceFiledForms(BuildFiledForms(employeeLeaves, employeeDeductions));
        EmptyMessage = string.IsNullOrWhiteSpace(Username)
          ? "No signed-in user was found for this session."
          : "The signed-in user is not linked to an employee record.";

        OnPropertyChanged(nameof(DisplayName));
        OnPropertyChanged(nameof(EmailDisplay));
        OnPropertyChanged(nameof(PositionDisplay));
        OnPropertyChanged(nameof(DepartmentDisplay));
        OnPropertyChanged(nameof(SectionDisplay));
        OnPropertyChanged(nameof(TypeLevelDisplay));
        OnPropertyChanged(nameof(TenantDisplay));
        OnPropertyChanged(nameof(StatusDisplay));
        OnPropertyChanged(nameof(MobileDisplay));
        OnPropertyChanged(nameof(DateHiredDisplay));
        OnPropertyChanged(nameof(LeaveBalanceCount));
        OnPropertyChanged(nameof(FiledFormCount));
      });
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Loading profile");
    }
    finally
    {
      IsBusy = false;
    }
  }

  private async Task<UserRecordDto?> FindCurrentUserAsync()
  {
    Guid userId = LoginSession.CurrentUserId;
    if (userId == Guid.Empty)
    {
      return null;
    }

    return await userApiService.GetUserByIdAsync(userId).ConfigureAwait(false);
  }

  private static EmployeeSummary? FindEmployee(UserRecordDto? user)
  {
    return user?.Employee is null ? null : EmployeeApiService.MapToSummary(user.Employee);
  }

  private IReadOnlyList<ProfileLeaveBalance> BuildLeaveBalances(IReadOnlyList<LeaveSummary> employeeLeaves)
  {
    return referenceDataService.LeaveCredits
      .Select(leaveCredit =>
      {
        decimal used = employeeLeaves
          .Where(leave => leave.LeaveCreditId == leaveCredit.Id && leave.Status == ApplicationStatusDto.Approved)
          .Sum(GetLeaveDays);

        decimal remaining = Math.Max(leaveCredit.DefaultCredits - used, 0m);
        return new ProfileLeaveBalance(leaveCredit.Description, leaveCredit.DefaultCredits, used, remaining);
      })
      .OrderBy(balance => balance.LeaveCreditName, StringComparer.OrdinalIgnoreCase)
      .ToList();
  }

  private static IReadOnlyList<ProfileFiledForm> BuildFiledForms(
    IReadOnlyList<LeaveSummary> employeeLeaves,
    IReadOnlyList<DeductionSummary> employeeDeductions)
  {
    List<ProfileFiledForm> forms = [];

    forms.AddRange(employeeLeaves.Select(leave => new ProfileFiledForm(
      "Leave",
      leave.LeaveCreditName,
      leave.Status.ToString(),
      FormatDateRange(leave.StartDate, leave.EndDate),
      leave.StartDate ?? DateTime.MinValue)));

    forms.AddRange(employeeDeductions.Select(deduction => new ProfileFiledForm(
      "Deduction",
      deduction.Type?.ToString() ?? "Deduction",
      deduction.Status?.ToString() ?? string.Empty,
      FormatDateRange(deduction.StartDate, deduction.EndDate),
      deduction.StartDate ?? DateTime.MinValue)));

    return forms
      .OrderByDescending(form => form.SortDate)
      .ToList();
  }

  private static decimal GetLeaveDays(LeaveSummary leave)
  {
    if (leave.IsHalfDay)
    {
      return 0.5m;
    }

    if (!leave.StartDate.HasValue || !leave.EndDate.HasValue)
    {
      return 0m;
    }

    DateTime start = leave.StartDate.Value.Date;
    DateTime end = leave.EndDate.Value.Date;
    if (end < start)
    {
      return 0m;
    }

    return (decimal)(end - start).TotalDays + 1m;
  }

  private static string FormatDateRange(DateTime? startDate, DateTime? endDate)
  {
    if (!startDate.HasValue && !endDate.HasValue)
    {
      return string.Empty;
    }

    if (!endDate.HasValue || startDate?.Date == endDate.Value.Date)
    {
      return startDate?.ToString("MMM dd, yyyy") ?? string.Empty;
    }

    return $"{startDate:MMM dd, yyyy} - {endDate:MMM dd, yyyy}";
  }

  private void ReplaceLeaveBalances(IReadOnlyList<ProfileLeaveBalance> balances)
  {
    LeaveBalances.Clear();
    foreach (ProfileLeaveBalance balance in balances)
    {
      LeaveBalances.Add(balance);
    }

    LeaveBalances.UpdateRowIndexes();
  }

  private void ReplaceFiledForms(IReadOnlyList<ProfileFiledForm> forms)
  {
    FiledForms.Clear();
    foreach (ProfileFiledForm form in forms)
    {
      FiledForms.Add(form);
    }

    FiledForms.UpdateRowIndexes();
  }
}

public record ProfileLeaveBalance(
  string LeaveCreditName,
  decimal Entitled,
  decimal Used,
  decimal Remaining) : IRowIndexed
{
  public int RowIndex { get; set; }
}

public record ProfileFiledForm(
  string FormType,
  string Description,
  string Status,
  string DateRange,
  DateTime SortDate) : IRowIndexed
{
  public int RowIndex { get; set; }
}

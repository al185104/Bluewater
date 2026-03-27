using System.Collections.ObjectModel;
using Bluewater.App.Extensions;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

public partial class LeaveViewModel : BaseViewModel
{
  private readonly ILeaveApiService leaveApiService;
  private readonly IEmployeeApiService employeeApiService;
  private readonly IReferenceDataService referenceDataService;
  private readonly List<LeaveSummary> allLeaves = [];
  private readonly Dictionary<Guid, HashSet<Guid>> employeeIdsByCharging = [];
  private bool hasInitialized;
  private bool suppressSelectedChargingChanged;
  private bool hasLoadedEmployeeChargingMap;

  public LeaveViewModel(
    ILeaveApiService leaveApiService,
    IEmployeeApiService employeeApiService,
    IReferenceDataService referenceDataService,
    IActivityTraceService activityTraceService,
    IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
    this.leaveApiService = leaveApiService;
    this.employeeApiService = employeeApiService;
    this.referenceDataService = referenceDataService;
    EditableLeave = CreateNewLeave();
  }

  public ObservableCollection<LeaveSummary> Leaves { get; } = new();
  public ObservableCollection<EmployeeSummary> Employees { get; } = new();
  public ObservableCollection<LeaveCreditSummary> LeaveCredits { get; } = new();
  public ObservableCollection<ChargingSummary> Chargings { get; } = new();

  [ObservableProperty]
  public partial LeaveSummary? SelectedLeave { get; set; }

  [ObservableProperty]
  public partial LeaveSummary EditableLeave { get; set; }

  [ObservableProperty]
  public partial TenantDto TenantFilter { get; set; } = TenantDto.Maribago;

  [ObservableProperty]
  public partial string SearchText { get; set; } = string.Empty;

  [ObservableProperty]
  public partial EmployeeSummary? SelectedEmployee { get; set; }

  [ObservableProperty]
  public partial ChargingSummary? SelectedCharging { get; set; }

  [ObservableProperty]
  public partial LeaveCreditSummary? SelectedLeaveCredit { get; set; }

  public override async Task InitializeAsync()
  {
    if (hasInitialized)
    {
      return;
    }

    hasInitialized = true;
    await TraceCommandAsync(nameof(InitializeAsync));
    await LoadReferenceDataAsync();
    await LoadChargingsAsync();
    await LoadLeavesAsync();
  }

  [RelayCommand]
  private async Task RefreshAsync()
  {
    try
    {
      await TraceCommandAsync(nameof(RefreshAsync));
      await LoadReferenceDataAsync();
      await LoadChargingsAsync();
      await LoadLeavesAsync();
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Refreshing leave records");
    }
  }

  [RelayCommand]
  private void BeginCreateLeave()
  {
    _ = TraceCommandAsync(nameof(BeginCreateLeave), new { SelectedEmployeeId = SelectedEmployee?.Id });
    EditableLeave = CreateNewLeave();
    SelectedLeave = null;
    SelectedLeaveCredit = null;

    if (SelectedEmployee is not null)
    {
      EditableLeave.EmployeeId = SelectedEmployee.Id;
      EditableLeave.EmployeeName = SelectedEmployee.FullName;
    }
  }

  [RelayCommand]
  private async Task SaveLeaveAsync()
  {
    if (EditableLeave.EmployeeId is null || EditableLeave.EmployeeId.Value == Guid.Empty)
    {
      return;
    }

    if (SelectedLeaveCredit is null)
    {
      return;
    }

    EditableLeave.LeaveCreditId = SelectedLeaveCredit.Id;
    EditableLeave.LeaveCreditName = SelectedLeaveCredit.Description;
    EditableLeave.Status = ApplicationStatusDto.Pending;

    bool isNew = EditableLeave.Id == Guid.Empty;

    try
    {
      IsBusy = true;

      LeaveSummary? saved = isNew
        ? await leaveApiService.CreateLeaveAsync(EditableLeave)
        : await leaveApiService.UpdateLeaveAsync(EditableLeave);

      LeaveSummary result = saved ?? EditableLeave;

      if (string.IsNullOrEmpty(result.EmployeeName))
      {
        result.EmployeeName = SelectedEmployee!.FullName;
      }

      int existingIndex = allLeaves.FindIndex(item => item.Id == result.Id);
      if (existingIndex >= 0)
      {
        allLeaves[existingIndex] = result;
      }
      else
      {
        allLeaves.Add(result);
      }

      await ApplyLeaveFilterAsync();
      EditableLeave = CloneLeave(result);
      SelectedLeave = result;
      await TraceCommandAsync(nameof(SaveLeaveAsync), result.Id);
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, isNew ? "Creating leave" : "Updating leave");
    }
    finally
    {
      IsBusy = false;
    }
  }

  [RelayCommand]
  private Task AcceptLeaveAsync(LeaveSummary? leave)
  {
    _ = TraceCommandAsync(nameof(AcceptLeaveAsync), leave?.Id);
    return UpdateLeaveStatusAsync(leave, ApplicationStatusDto.Approved);
  }

  [RelayCommand]
  private Task RejectLeaveAsync(LeaveSummary? leave)
  {
    _ = TraceCommandAsync(nameof(RejectLeaveAsync), leave?.Id);
    return UpdateLeaveStatusAsync(leave, ApplicationStatusDto.Rejected);
  }

  [RelayCommand]
  private async Task DeleteLeaveAsync(LeaveSummary? leave)
  {
    if (leave is null)
    {
      return;
    }

    try
    {
      IsBusy = true;

      bool deleted = await leaveApiService.DeleteLeaveAsync(leave.Id);

      if (deleted)
      {
        allLeaves.RemoveAll(item => item.Id == leave.Id);
        await ApplyLeaveFilterAsync();
        await TraceCommandAsync(nameof(DeleteLeaveAsync), leave.Id);
      }
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Deleting leave");
    }
    finally
    {
      IsBusy = false;
    }
  }

  partial void OnSearchTextChanged(string value)
  {
    _ = LoadSearchedEmployeeAsync(value);
    _ = ApplyLeaveFilterAsync();
  }

  partial void OnSelectedEmployeeChanged(EmployeeSummary? value)
  {
    if (value is not null)
    {
      TrySelectChargingForEmployee(value);
      EditableLeave.EmployeeId = value.Id;
      EditableLeave.EmployeeName = value.FullName;
    }

    _ = ApplyLeaveFilterAsync();
  }

  partial void OnSelectedChargingChanged(ChargingSummary? value)
  {
    if (suppressSelectedChargingChanged)
    {
      return;
    }

    if (SelectedEmployee is not null && SelectedEmployee.ChargingId != value?.Id)
    {
      SelectedEmployee = null;
    }

    if (hasInitialized)
    {
      _ = LoadLeavesAsync();
      return;
    }

    _ = ApplyLeaveFilterAsync();
  }

  partial void OnSelectedLeaveCreditChanged(LeaveCreditSummary? value)
  {
    if (value is null)
    {
      return;
    }

    EditableLeave.LeaveCreditId = value.Id;
    EditableLeave.LeaveCreditName = value.Description;
  }

  private async Task LoadLeavesAsync()
  {
    try
    {
      IsBusy = true;

      IReadOnlyList<LeaveSummary> leaves = await leaveApiService
        .GetLeavesAsync(tenant: TenantFilter, chargingId: SelectedCharging?.Id);

      allLeaves.Clear();
      allLeaves.AddRange(leaves);
      await EnsureEmployeeChargingMapAsync();
      await ApplyLeaveFilterAsync();
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Loading leaves");
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
      await MainThread.InvokeOnMainThreadAsync(() =>
      {
        Employees.Clear();
        SelectedEmployee = null;
      });
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

      await MainThread.InvokeOnMainThreadAsync(() =>
      {
        Employees.Clear();
        if (matchedEmployee is not null)
        {
          Employees.Add(matchedEmployee);
          RegisterEmployeeCharging(matchedEmployee);
        }

        SelectedEmployee = matchedEmployee;
      });
      await ApplyLeaveFilterAsync();
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Searching employee");
    }
  }

  private async Task UpdateLeaveStatusAsync(LeaveSummary? leave, ApplicationStatusDto status)
  {
    if (leave is null)
    {
      return;
    }

    try
    {
      IsBusy = true;
      LeaveSummary request = CloneLeave(leave);
      request.Status = status;

      LeaveSummary? updated = await leaveApiService.UpdateLeaveAsync(request);
      if (updated is null)
      {
        return;
      }

      int existingIndex = allLeaves.FindIndex(item => item.Id == updated.Id);
      if (existingIndex >= 0)
      {
        allLeaves[existingIndex] = updated;
      }

      await ApplyLeaveFilterAsync();
      await TraceCommandAsync(nameof(UpdateLeaveStatusAsync), updated.Id);
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Updating leave status");
    }
    finally
    {
      IsBusy = false;
    }
  }

  private async Task LoadReferenceDataAsync()
  {
    try
    {
      await referenceDataService.InitializeAsync();
      IReadOnlyList<LeaveCreditSummary> leaveCredits = referenceDataService.LeaveCredits.ToList();
      await MainThread.InvokeOnMainThreadAsync(() => ReplaceLeaveCredits(leaveCredits));
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Loading leave credits");
    }
  }

  private async Task ApplyLeaveFilterAsync()
  {
    IEnumerable<LeaveSummary> filteredLeaves = allLeaves;
    if (SelectedCharging is not null)
    {
      HashSet<Guid> employeeIds = employeeIdsByCharging.TryGetValue(SelectedCharging.Id, out HashSet<Guid>? ids)
        ? ids
        : [];
      filteredLeaves = filteredLeaves.Where(leave =>
        !leave.EmployeeId.HasValue || employeeIds.Contains(leave.EmployeeId.Value));
    }

    if (SelectedEmployee is not null)
    {
      filteredLeaves = filteredLeaves.Where(leave => leave.EmployeeId == SelectedEmployee.Id);
    }
    else if (!string.IsNullOrWhiteSpace(SearchText))
    {
      string search = SearchText.Trim();
      filteredLeaves = filteredLeaves.Where(leave =>
        leave.EmployeeName.Contains(search, StringComparison.OrdinalIgnoreCase));
    }

    List<LeaveSummary> orderedLeaves = filteredLeaves
      .OrderByDescending(item => item.StartDate)
      .ToList();

    await MainThread.InvokeOnMainThreadAsync(() => ReplaceLeaves(orderedLeaves));
  }

  private async Task LoadChargingsAsync()
  {
    try
    {
      await referenceDataService.InitializeAsync();
      Guid? previousId = SelectedCharging?.Id;
      suppressSelectedChargingChanged = true;
      IReadOnlyList<ChargingSummary> chargings = referenceDataService.Chargings
        .OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
        .ToList();
      await MainThread.InvokeOnMainThreadAsync(() => ReplaceChargings(chargings, previousId));
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Loading chargings");
    }
    finally
    {
      suppressSelectedChargingChanged = false;
    }
  }

  private void TrySelectChargingForEmployee(EmployeeSummary employee)
  {
    RegisterEmployeeCharging(employee);

    if (!employee.ChargingId.HasValue || employee.ChargingId.Value == Guid.Empty)
    {
      return;
    }

    ChargingSummary? targetCharging = Chargings.FirstOrDefault(charging => charging.Id == employee.ChargingId.Value);
    if (targetCharging is null || SelectedCharging?.Id == targetCharging.Id)
    {
      return;
    }

    suppressSelectedChargingChanged = true;
    SelectedCharging = targetCharging;
    suppressSelectedChargingChanged = false;
  }

  private async Task EnsureEmployeeChargingMapAsync()
  {
    if (hasLoadedEmployeeChargingMap)
    {
      return;
    }

    const int pageSize = 200;
    int skip = 0;
    while (true)
    {
      PagedResult<EmployeeSummary> page = await employeeApiService.GetEmployeesAsync(skip: skip, take: pageSize);
      if (page.Items.Count == 0)
      {
        break;
      }

      foreach (EmployeeSummary employee in page.Items)
      {
        RegisterEmployeeCharging(employee);
      }

      skip += page.Items.Count;
      if (page.Items.Count < pageSize)
      {
        break;
      }
    }

    hasLoadedEmployeeChargingMap = true;
  }

  private void RegisterEmployeeCharging(EmployeeSummary employee)
  {
    if (!employee.ChargingId.HasValue || employee.ChargingId.Value == Guid.Empty)
    {
      return;
    }

    if (!employeeIdsByCharging.TryGetValue(employee.ChargingId.Value, out HashSet<Guid>? employeeIds))
    {
      employeeIds = [];
      employeeIdsByCharging[employee.ChargingId.Value] = employeeIds;
    }

    employeeIds.Add(employee.Id);
  }

  private static LeaveSummary CreateNewLeave()
  {
    return new LeaveSummary
    {
      Id = Guid.Empty,
      StartDate = DateTime.Today,
      EndDate = DateTime.Today,
      Status = ApplicationStatusDto.Pending,
      RowIndex = 0
    };
  }

  private static LeaveSummary CloneLeave(LeaveSummary leave)
  {
    return new LeaveSummary
    {
      Id = leave.Id,
      StartDate = leave.StartDate,
      EndDate = leave.EndDate,
      IsHalfDay = leave.IsHalfDay,
      Status = leave.Status,
      EmployeeId = leave.EmployeeId,
      LeaveCreditId = leave.LeaveCreditId,
      EmployeeName = leave.EmployeeName,
      LeaveCreditName = leave.LeaveCreditName,
      RowIndex = leave.RowIndex
    };
  }

  private void ReplaceLeaveCredits(IReadOnlyList<LeaveCreditSummary> leaveCredits)
  {
    LeaveCredits.Clear();
    foreach (LeaveCreditSummary leaveCredit in leaveCredits)
    {
      LeaveCredits.Add(leaveCredit);
    }
  }

  private void ReplaceLeaves(IReadOnlyList<LeaveSummary> leaves)
  {
    Leaves.Clear();
    foreach (LeaveSummary leave in leaves)
    {
      Leaves.Add(leave);
    }

    Leaves.UpdateRowIndexes();
  }

  private void ReplaceChargings(IReadOnlyList<ChargingSummary> chargings, Guid? previousId)
  {
    Chargings.Clear();
    foreach (ChargingSummary charging in chargings)
    {
      Chargings.Add(charging);
    }

    if (previousId.HasValue)
    {
      SelectedCharging = Chargings.FirstOrDefault(item => item.Id == previousId.Value);
    }

    if (SelectedCharging is null && Chargings.Count > 0)
    {
      SelectedCharging = Chargings[0];
    }
  }
}

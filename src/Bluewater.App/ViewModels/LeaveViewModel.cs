using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Bluewater.App.Exceptions;
using Bluewater.App.Extensions;
using Bluewater.App.Helpers;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.Services;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

public partial class LeaveViewModel : BaseViewModel
{
  private readonly ILeaveApiService leaveApiService;
  private readonly IEmployeeApiService employeeApiService;
  private readonly IUserApiService userApiService;
  private readonly IReferenceDataService referenceDataService;
  private readonly List<LeaveSummary> allLeaves = [];
  private readonly Dictionary<Guid, HashSet<Guid>> employeeIdsByCharging = [];
  private bool hasInitialized;
  private bool suppressSelectedChargingChanged;
  private bool suppressSearchTextChanged;
  private bool hasLoadedEmployeeChargingMap;

  public LeaveViewModel(
    ILeaveApiService leaveApiService,
    IEmployeeApiService employeeApiService,
    IUserApiService userApiService,
    IReferenceDataService referenceDataService,
    IActivityTraceService activityTraceService,
    IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
    this.leaveApiService = leaveApiService;
    this.employeeApiService = employeeApiService;
    this.userApiService = userApiService;
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

  public bool IsSelfServiceMode => !LoginSession.IsManagerOrAbove;
  public bool CanSelectEmployeeFilters => !IsSelfServiceMode;
  public bool CanReviewApplications => LoginSession.IsManagerOrAbove;

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
    await ApplySelfServiceScopeAsync();
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
      await ApplySelfServiceScopeAsync();
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
    if (leave is null || !leave.CanModify)
    {
      return;
    }

    try
    {
      bool confirmed = await Shell.Current.DisplayAlertAsync(
        "Delete leave",
        $"Delete leave record for {leave.EmployeeName} from {leave.StartDate:MMM dd, yyyy} to {leave.EndDate:MMM dd, yyyy}?",
        "Delete",
        "Cancel");

      if (!confirmed)
      {
        return;
      }

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

  [RelayCommand]
  private async Task DownloadLeavesAsync()
  {
    if (IsBusy)
    {
      return;
    }

    if (Leaves.Count == 0)
    {
      await Shell.Current.DisplayAlertAsync("Download", "No leave records to download.", "Okay");
      return;
    }

    try
    {
      IsBusy = true;
      Dictionary<Guid, EmployeeSummary> employeesById = await LoadEmployeesByIdAsync(TenantFilter).ConfigureAwait(false);

      StringBuilder csv = new();
      csv.AppendLine(string.Join(",", new[]
      {
        "EmployeeId",
        "Employee",
        "Barcode",
        "LeaveCode",
        "StartDate",
        "EndDate",
        "IsHalfDay",
        "Status"
      }));

      foreach (LeaveSummary leave in Leaves.OrderBy(item => item.EmployeeName).ThenBy(item => item.StartDate))
      {
        EmployeeSummary? employee = leave.EmployeeId.HasValue && employeesById.TryGetValue(leave.EmployeeId.Value, out EmployeeSummary? found)
          ? found
          : null;

        csv.AppendLine(string.Join(",", new[]
        {
          EscapeCsv(leave.EmployeeId?.ToString()),
          EscapeCsv(leave.EmployeeName),
          EscapeCsv(employee?.User.Username),
          EscapeCsv(leave.LeaveCreditName),
          EscapeCsv(leave.StartDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
          EscapeCsv(leave.EndDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
          EscapeCsv(leave.IsHalfDay ? "Yes" : "No"),
          EscapeCsv(leave.Status.ToString())
        }));
      }

      string fileName = $"leaves_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
      string downloadsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
      Directory.CreateDirectory(downloadsDirectory);
      string filePath = Path.Combine(downloadsDirectory, fileName);
      await File.WriteAllTextAsync(filePath, csv.ToString(), Encoding.UTF8).ConfigureAwait(false);

      await MainThread.InvokeOnMainThreadAsync(() =>
        Shell.Current.DisplayAlertAsync("Download", $"Leave records downloaded to {filePath}", "Okay"));

      await TraceCommandAsync(nameof(DownloadLeavesAsync), new
      {
        FileName = fileName,
        RecordCount = Leaves.Count,
        Tenant = TenantFilter.ToString(),
        Charging = SelectedCharging?.Name
      }).ConfigureAwait(false);
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Downloading leave records");
    }
    finally
    {
      IsBusy = false;
    }
  }

  [RelayCommand]
  private async Task UploadLeavesAsync()
  {
    if (IsBusy)
    {
      return;
    }

    try
    {
      PickOptions options = new()
      {
        PickerTitle = "Select leave CSV file",
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
      IReadOnlyList<LeaveCsvRow> rows = await ParseLeaveCsvAsync(stream).ConfigureAwait(false);

      if (rows.Count == 0)
      {
        await MainThread.InvokeOnMainThreadAsync(() =>
          Shell.Current.DisplayAlertAsync("Upload", "No leave rows were found in the selected file.", "Okay"));
        return;
      }

      IsBusy = true;

      Dictionary<Guid, EmployeeSummary> employeesById = await LoadEmployeesByIdAsync(TenantFilter).ConfigureAwait(false);
      Dictionary<string, EmployeeSummary> employeesByBarcode = employeesById.Values
        .Where(employee => !string.IsNullOrWhiteSpace(employee.User.Username))
        .GroupBy(employee => employee.User.Username.Trim(), StringComparer.OrdinalIgnoreCase)
        .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

      Dictionary<string, LeaveCreditSummary> leaveCreditsByCode = LeaveCredits
        .Where(credit => !string.IsNullOrWhiteSpace(credit.Code))
        .GroupBy(credit => credit.Code.Trim(), StringComparer.OrdinalIgnoreCase)
        .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

      int createdCount = 0;
      int skippedCount = 0;
      int failedCount = 0;

      foreach (LeaveCsvRow row in rows)
      {
        if (!TryResolveEmployee(row, employeesById, employeesByBarcode, out EmployeeSummary? employee))
        {
          skippedCount++;
          continue;
        }

        if (employee is null)
        {
          skippedCount++;
          continue;
        }

        if (!leaveCreditsByCode.TryGetValue(row.LeaveCode.Trim(), out LeaveCreditSummary? leaveCredit))
        {
          skippedCount++;
          continue;
        }

        if (leaveCredit is null)
        {
          skippedCount++;
          continue;
        }

        EmployeeSummary resolvedEmployee = employee;
        LeaveCreditSummary resolvedLeaveCredit = leaveCredit;

        if (HasConflictingImportedLeave(resolvedEmployee.Id, resolvedLeaveCredit.Id, row.StartDate, row.EndDate))
        {
          skippedCount++;
          continue;
        }

        LeaveSummary request = new()
        {
          EmployeeId = resolvedEmployee.Id,
          EmployeeName = resolvedEmployee.FullName,
          LeaveCreditId = resolvedLeaveCredit.Id,
          LeaveCreditName = resolvedLeaveCredit.Code,
          StartDate = row.StartDate,
          EndDate = row.EndDate,
          IsHalfDay = row.IsHalfDay,
          Status = ApplicationStatusDto.Pending
        };

        LeaveSummary? created;
        try
        {
          created = await leaveApiService.CreateLeaveAsync(request).ConfigureAwait(false);
        }
        catch (PresentationException)
        {
          skippedCount++;
          continue;
        }

        if (created is null)
        {
          failedCount++;
          continue;
        }

        created.Status = ApplicationStatusDto.Pending;
        if (string.IsNullOrWhiteSpace(created.EmployeeName))
        {
          created.EmployeeName = resolvedEmployee.FullName;
        }

        if (string.IsNullOrWhiteSpace(created.LeaveCreditName))
        {
          created.LeaveCreditName = resolvedLeaveCredit.Code;
        }

        allLeaves.Add(created);
        createdCount++;
      }

      await ApplyLeaveFilterAsync().ConfigureAwait(false);

      await MainThread.InvokeOnMainThreadAsync(() =>
        Shell.Current.DisplayAlertAsync(
          "Upload",
          $"Imported {createdCount} leave record(s) as Pending. Skipped {skippedCount}. Failed {failedCount}.",
          "Okay"));

      await TraceCommandAsync(nameof(UploadLeavesAsync), new
      {
        File = file.FileName,
        Created = createdCount,
        Skipped = skippedCount,
        Failed = failedCount
      }).ConfigureAwait(false);
    }
    catch (FormatException ex)
    {
      ExceptionHandlingService.Handle(ex, "Uploading leave records");
      await MainThread.InvokeOnMainThreadAsync(() => Shell.Current.DisplayAlertAsync("Upload error", ex.Message, "Okay"));
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Uploading leave records");
    }
    finally
    {
      IsBusy = false;
    }
  }
  partial void OnSearchTextChanged(string value)
  {
    if (suppressSearchTextChanged)
    {
      return;
    }

    if (IsSelfServiceMode)
    {
      SearchText = SelectedEmployee?.FullName ?? SearchText;
      return;
    }

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

      MainThread.BeginInvokeOnMainThread(async() => { 
        allLeaves.Clear();
        allLeaves.AddRange(leaves);
        await EnsureEmployeeChargingMapAsync();
        await ApplyLeaveFilterAsync();
      });
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

  private async Task ApplySelfServiceScopeAsync()
  {
    if (!IsSelfServiceMode)
    {
      return;
    }

    EmployeeSummary? employee = await FindLoggedInEmployeeAsync();
    if (employee is null)
    {
      return;
    }

    await MainThread.InvokeOnMainThreadAsync(() =>
    {
      suppressSearchTextChanged = true;
      try
      {
        Employees.Clear();
        Employees.Add(employee);
        RegisterEmployeeCharging(employee);
        SelectedEmployee = employee;
        SearchText = employee.FullName;
        TrySelectChargingForEmployee(employee);
      }
      finally
      {
        suppressSearchTextChanged = false;
      }
    });
  }

  private async Task<EmployeeSummary?> FindLoggedInEmployeeAsync()
  {
    Guid userId = LoginSession.CurrentUserId;
    if (userId == Guid.Empty)
    {
      return null;
    }

    UserRecordDto? user = await userApiService.GetUserByIdAsync(userId);
    return user?.Employee is null ? null : EmployeeApiService.MapToSummary(user.Employee);
  }

  private async Task UpdateLeaveStatusAsync(LeaveSummary? leave, ApplicationStatusDto status)
  {
    if (leave is null || !leave.CanReview)
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

    MainThread.BeginInvokeOnMainThread(() => {
      Leaves.Clear();
      foreach (LeaveSummary leave in orderedLeaves)
      {
        Leaves.Add(leave);
      }

      Leaves.UpdateRowIndexes();
    });
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

  private async Task<Dictionary<Guid, EmployeeSummary>> LoadEmployeesByIdAsync(TenantDto tenant)
  {
    const int pageSize = 200;
    int skip = 0;
    Dictionary<Guid, EmployeeSummary> employeesById = [];

    while (true)
    {
      PagedResult<EmployeeSummary> page = await employeeApiService
        .GetEmployeesAsync(skip: skip, take: pageSize, tenant: tenant)
        .ConfigureAwait(false);

      if (page.Items.Count == 0)
      {
        break;
      }

      foreach (EmployeeSummary employee in page.Items)
      {
        employeesById[employee.Id] = employee;
        RegisterEmployeeCharging(employee);
      }

      skip += page.Items.Count;
      if (page.Items.Count < pageSize || employeesById.Count >= page.TotalCount)
      {
        break;
      }
    }

    hasLoadedEmployeeChargingMap = true;
    return employeesById;
  }

  private static bool TryResolveEmployee(
    LeaveCsvRow row,
    IReadOnlyDictionary<Guid, EmployeeSummary> employeesById,
    IReadOnlyDictionary<string, EmployeeSummary> employeesByBarcode,
    out EmployeeSummary? employee)
  {
    employee = null;

    if (row.EmployeeId is Guid employeeId && employeesById.TryGetValue(employeeId, out employee))
    {
      return true;
    }

    if (!string.IsNullOrWhiteSpace(row.Barcode) && employeesByBarcode.TryGetValue(row.Barcode.Trim(), out employee))
    {
      return true;
    }

    return false;
  }

  private bool HasConflictingImportedLeave(Guid employeeId, Guid leaveCreditId, DateTime startDate, DateTime endDate)
  {
    DateTime rangeStart = startDate.Date;
    DateTime rangeEnd = endDate.Date;

    return allLeaves.Any(leave =>
      leave.EmployeeId == employeeId &&
      leave.LeaveCreditId == leaveCreditId &&
      leave.Status != ApplicationStatusDto.Rejected &&
      leave.StartDate.HasValue &&
      leave.EndDate.HasValue &&
      leave.StartDate.Value.Date <= rangeEnd &&
      leave.EndDate.Value.Date >= rangeStart);
  }

  private static async Task<IReadOnlyList<LeaveCsvRow>> ParseLeaveCsvAsync(Stream stream)
  {
    using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
    string? headerLine = await reader.ReadLineAsync().ConfigureAwait(false);
    if (string.IsNullOrWhiteSpace(headerLine))
    {
      return [];
    }

    string[] headers = SplitCsvLine(headerLine);
    Dictionary<string, int> headerLookup = CreateHeaderLookup(headers);

    if (!TryGetHeaderIndex(headerLookup, "EmployeeId", out _) &&
        !TryGetHeaderIndex(headerLookup, "Barcode", out _))
    {
      throw new FormatException("The CSV file must include either EmployeeId or Barcode.");
    }

    if (!TryGetHeaderIndex(headerLookup, "LeaveCode", out _))
    {
      throw new FormatException("The CSV file must include a LeaveCode column, such as VL or SL.");
    }

    if (!TryGetHeaderIndex(headerLookup, "StartDate", out _) ||
        !TryGetHeaderIndex(headerLookup, "EndDate", out _))
    {
      throw new FormatException("The CSV file must include StartDate and EndDate columns.");
    }

    var rows = new List<LeaveCsvRow>();
    string? line;
    int lineNumber = 1;
    while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) is not null)
    {
      lineNumber++;
      if (string.IsNullOrWhiteSpace(line))
      {
        continue;
      }

      string[] values = SplitCsvLine(line);
      string employeeIdText = GetValue(values, headerLookup, "EmployeeId");
      string barcode = GetValue(values, headerLookup, "Barcode");
      string leaveCode = GetValue(values, headerLookup, "LeaveCode");
      string startDateText = GetValue(values, headerLookup, "StartDate");
      string endDateText = GetValue(values, headerLookup, "EndDate");
      string halfDayText = GetValue(values, headerLookup, "IsHalfDay");

      if (string.IsNullOrWhiteSpace(employeeIdText) && string.IsNullOrWhiteSpace(barcode))
      {
        continue;
      }

      if (string.IsNullOrWhiteSpace(leaveCode))
      {
        throw new FormatException($"Line {lineNumber}: LeaveCode is required.");
      }

      Guid? employeeId = null;
      if (!string.IsNullOrWhiteSpace(employeeIdText))
      {
        if (!Guid.TryParse(employeeIdText, out Guid parsedEmployeeId))
        {
          throw new FormatException($"Line {lineNumber}: EmployeeId is not a valid GUID.");
        }

        employeeId = parsedEmployeeId;
      }

      rows.Add(new LeaveCsvRow(
        employeeId,
        barcode,
        leaveCode,
        ParseRequiredDate(startDateText, lineNumber, "StartDate"),
        ParseRequiredDate(endDateText, lineNumber, "EndDate"),
        ParseBoolean(halfDayText)));
    }

    return rows;
  }

  private static Dictionary<string, int> CreateHeaderLookup(string[] headers)
  {
    var lookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    for (int i = 0; i < headers.Length; i++)
    {
      string header = headers[i].Trim();
      if (!string.IsNullOrWhiteSpace(header) && !lookup.ContainsKey(header))
      {
        lookup[header] = i;
      }
    }

    return lookup;
  }

  private static bool TryGetHeaderIndex(IReadOnlyDictionary<string, int> headers, string name, out int index)
  {
    if (headers.TryGetValue(name, out index))
    {
      return true;
    }

    string[] aliases = name switch
    {
      "Barcode" => ["EmployeeBarcode", "Employee Barcode", "Username"],
      "LeaveCode" => ["Leave Code", "Code", "LeaveCredit", "Leave Credit", "LeaveCreditCode", "Leave Credit Code"],
      "StartDate" => ["Start Date", "From"],
      "EndDate" => ["End Date", "To"],
      "IsHalfDay" => ["HalfDay", "Half Day", "Is Half Day"],
      "EmployeeId" => ["Employee Id"],
      _ => []
    };

    foreach (string alias in aliases)
    {
      if (headers.TryGetValue(alias, out index))
      {
        return true;
      }
    }

    index = -1;
    return false;
  }

  private static string GetValue(string[] values, IReadOnlyDictionary<string, int> headers, string name)
  {
    return TryGetHeaderIndex(headers, name, out int index) && index >= 0 && index < values.Length
      ? values[index].Trim()
      : string.Empty;
  }

  private static DateTime ParseRequiredDate(string value, int lineNumber, string columnName)
  {
    string[] formats = ["yyyy-MM-dd", "M/d/yyyy", "MM/dd/yyyy", "M-d-yyyy", "MM-dd-yyyy", "MMM d yyyy", "MMM d, yyyy"];
    if (DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out DateTime exact) ||
        DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out exact) ||
        DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out exact))
    {
      return exact.Date;
    }

    throw new FormatException($"Line {lineNumber}: {columnName} is not a valid date.");
  }

  private static bool ParseBoolean(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      return false;
    }

    if (bool.TryParse(value, out bool result))
    {
      return result;
    }

    string normalized = value.Trim();
    return string.Equals(normalized, "yes", StringComparison.OrdinalIgnoreCase) ||
           string.Equals(normalized, "y", StringComparison.OrdinalIgnoreCase) ||
           string.Equals(normalized, "1", StringComparison.OrdinalIgnoreCase) ||
           string.Equals(normalized, "half", StringComparison.OrdinalIgnoreCase);
  }

  private static string[] SplitCsvLine(string line)
  {
    var values = new List<string>();
    var current = new StringBuilder();
    bool inQuotes = false;

    for (int i = 0; i < line.Length; i++)
    {
      char c = line[i];
      if (c == '"')
      {
        if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
        {
          current.Append('"');
          i++;
        }
        else
        {
          inQuotes = !inQuotes;
        }
      }
      else if (c == ',' && !inQuotes)
      {
        values.Add(current.ToString());
        current.Clear();
      }
      else
      {
        current.Append(c);
      }
    }

    values.Add(current.ToString());
    return values.ToArray();
  }

  private static string EscapeCsv(string? value)
  {
    if (string.IsNullOrEmpty(value))
    {
      return string.Empty;
    }

    string escaped = value.Replace("\"", "\"\"", StringComparison.Ordinal);
    return escaped.IndexOfAny([',', '"', '\n', '\r']) >= 0
      ? $"\"{escaped}\""
      : escaped;
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
      HasPayrollCreated = leave.HasPayrollCreated,
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

public sealed record LeaveCsvRow(Guid? EmployeeId, string Barcode, string LeaveCode, DateTime StartDate, DateTime EndDate, bool IsHalfDay);

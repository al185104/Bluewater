using System.Collections.ObjectModel;
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
		private readonly ITimesheetApiService timesheetApiService;
		private readonly ILeaveApiService leaveApiService;
		private readonly IHolidayApiService holidayApiService;
		private readonly IReferenceDataService referenceDataService;
		private readonly SemaphoreSlim dashboardLoadSemaphore = new(1, 1);
		private readonly Dictionary<DashboardPayrollCacheKey, IReadOnlyList<PayrollSummary>> payrollCache = [];
		private readonly Dictionary<DashboardTimesheetCacheKey, IReadOnlyList<EmployeeTimesheetSummary>> timesheetCache = [];
		private CancellationTokenSource? dashboardLoadCancellationTokenSource;
		private IReadOnlyList<EmployeeSummary> cachedEmployees = Array.Empty<EmployeeSummary>();
		private TenantDto? cachedEmployeesTenant;
		private bool hasInitialized;
		private bool suppressSelectionChanged;

		public DashboardContentViewModel(
			IEmployeeApiService employeeApiService,
			IPayrollApiService payrollApiService,
			ITimesheetApiService timesheetApiService,
			ILeaveApiService leaveApiService,
			IHolidayApiService holidayApiService,
			IReferenceDataService referenceDataService,
			IActivityTraceService activityTraceService,
			IExceptionHandlingService exceptionHandlingService)
			: base(activityTraceService, exceptionHandlingService)
		{
				this.employeeApiService = employeeApiService;
				this.payrollApiService = payrollApiService;
				this.timesheetApiService = timesheetApiService;
				this.leaveApiService = leaveApiService;
				this.holidayApiService = holidayApiService;
				this.referenceDataService = referenceDataService;

				SelectedTenant = TenantPreferences.GetSelectedTenant();
				SetCurrentPayslipPeriod();
		}

		public ObservableCollection<DepartmentSummary> DepartmentOptions { get; } = new();

		public ObservableCollection<ChargingSummary> ChargingOptions { get; } = new();

		public ObservableCollection<DepartmentPayrollCostSummary> PayrollCostByDepartment { get; } = new();

		public ObservableCollection<ChargingPayrollGenerationSummary> PayrollGeneratedStatusByCharging { get; } = new();

		public ObservableCollection<HolidaySummary> UpcomingHolidays { get; } = new();

		[ObservableProperty]
		public partial TenantDto SelectedTenant { get; set; }

		[ObservableProperty]
		public partial DepartmentSummary? SelectedDepartment { get; set; }

		[ObservableProperty]
		public partial ChargingSummary? SelectedCharging { get; set; }

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

				await TraceCommandAsync(nameof(InitializeAsync));
				await EnsureFilterOptionsAsync().ConfigureAwait(false);
				hasInitialized = true;
				await LoadDashboardAsync().ConfigureAwait(false);
		}

		[RelayCommand]
		private async Task RefreshAsync()
		{
				try
				{
						await TraceCommandAsync(nameof(RefreshAsync));
						ResetCachedData();
						await LoadDashboardAsync().ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Refreshing dashboard");
				}
		}

		[RelayCommand(CanExecute = nameof(CanChangePeriod))]
		private async Task PreviousPayrollPeriodAsync()
		{
				try
				{
						SetCurrentPayslipPeriod(PayrollPeriodStart.AddDays(-1));
						ClearPayrollCache();
						await TraceCommandAsync(nameof(PreviousPayrollPeriodAsync));
						await LoadDashboardAsync().ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Loading previous dashboard period");
				}
		}

		[RelayCommand(CanExecute = nameof(CanChangePeriod))]
		private async Task NextPayrollPeriodAsync()
		{
				try
				{
						SetCurrentPayslipPeriod(PayrollPeriodEnd.AddDays(1));
						ClearPayrollCache();
						await TraceCommandAsync(nameof(NextPayrollPeriodAsync));
						await LoadDashboardAsync().ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Loading next dashboard period");
				}
		}

		private bool CanChangePeriod() => !IsBusy;

		private async Task EnsureFilterOptionsAsync(CancellationToken cancellationToken = default)
		{
				try
				{
						await referenceDataService.InitializeAsync(cancellationToken).ConfigureAwait(false);

						await MainThread.InvokeOnMainThreadAsync(() =>
						{
								suppressSelectionChanged = true;
								try
								{
										Guid? selectedDepartmentId = SelectedDepartment?.Id;
										Guid? selectedChargingId = SelectedCharging?.Id;
										HashSet<Guid> validDepartmentIds = referenceDataService.Chargings
											.Where(charging => charging.DepartmentId.HasValue)
											.Select(charging => charging.DepartmentId!.Value)
											.ToHashSet();

										DepartmentOptions.Clear();
										foreach (DepartmentSummary department in referenceDataService.Departments
											.Where(department => validDepartmentIds.Contains(department.Id)))
										{
												DepartmentOptions.Add(department);
										}

										SelectedDepartment = selectedDepartmentId.HasValue
												? DepartmentOptions.FirstOrDefault(option => option.Id == selectedDepartmentId.Value) ?? DepartmentOptions.FirstOrDefault()
												: DepartmentOptions.FirstOrDefault();
										RefreshChargingOptions(selectedChargingId);
								}
								finally
								{
										suppressSelectionChanged = false;
								}
						});
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Loading dashboard filter options");
				}
		}

		private async Task LoadDashboardAsync(CancellationToken cancellationToken = default)
		{
				CancelPendingDashboardLoad();
				using CancellationTokenSource linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
				dashboardLoadCancellationTokenSource = linkedCancellationTokenSource;
				bool lockAcquired = false;

				try
				{
						await dashboardLoadSemaphore.WaitAsync(linkedCancellationTokenSource.Token).ConfigureAwait(false);
						lockAcquired = true;
						await MainThread.InvokeOnMainThreadAsync(() =>
						{
								IsBusy = true;
								Today = DateTime.Today;
						});

						DateOnly todayDate = DateOnly.FromDateTime(Today);
						string? selectedDepartmentName = NormalizeDepartmentName(SelectedDepartment?.Name);
						string? selectedChargingName = NormalizeChargingName(SelectedCharging?.Name);

						Task<IReadOnlyList<EmployeeSummary>> employeesTask = LoadEmployeesForTenantAsync(SelectedTenant, linkedCancellationTokenSource.Token);
					Task<IReadOnlyList<PayrollSummary>> payrollTask = LoadPayrollsInPeriodAsync(selectedChargingName, linkedCancellationTokenSource.Token);
					Task<IReadOnlyList<EmployeeTimesheetSummary>> timesheetTask = LoadTimesheetsInPeriodAsync(selectedChargingName, linkedCancellationTokenSource.Token);
					Task<IReadOnlyList<LeaveSummary>> leaveTask = leaveApiService.GetLeavesAsync(tenant: SelectedTenant, cancellationToken: linkedCancellationTokenSource.Token);
					Task<IReadOnlyList<HolidaySummary>> holidayTask = holidayApiService.GetHolidaysAsync(cancellationToken: linkedCancellationTokenSource.Token);

					await Task.WhenAll(employeesTask, payrollTask, timesheetTask, leaveTask, holidayTask).ConfigureAwait(false);
					linkedCancellationTokenSource.Token.ThrowIfCancellationRequested();

					IReadOnlyList<EmployeeSummary> scopedEmployees = FilterEmployees(employeesTask.Result, selectedDepartmentName, selectedChargingName);
					IReadOnlyList<PayrollSummary> scopedPayrolls = FilterPayrolls(payrollTask.Result, selectedDepartmentName, selectedChargingName);
					IReadOnlyList<EmployeeTimesheetSummary> scopedTimesheets = FilterTimesheets(timesheetTask.Result, selectedDepartmentName, selectedChargingName);
					IReadOnlyList<LeaveSummary> scopedLeaves = FilterLeaves(leaveTask.Result, scopedEmployees);

					DashboardComputationResult result = ComputeDashboard(scopedEmployees, scopedPayrolls, scopedTimesheets, scopedLeaves, holidayTask.Result, todayDate);
					await ApplyDashboardResultAsync(result).ConfigureAwait(false);
				}
				catch (OperationCanceledException)
				{
						// A newer filter selection superseded this load.
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Loading dashboard information");
				}
				finally
				{
						if (ReferenceEquals(dashboardLoadCancellationTokenSource, linkedCancellationTokenSource))
						{
								dashboardLoadCancellationTokenSource = null;
						}

						if (lockAcquired)
						{
								dashboardLoadSemaphore.Release();
						}

						await MainThread.InvokeOnMainThreadAsync(() =>
						{
								IsBusy = false;
								RaisePeriodNavigationState();
						});
				}
		}

		private async Task<IReadOnlyList<EmployeeSummary>> LoadEmployeesForTenantAsync(TenantDto tenant, CancellationToken cancellationToken)
		{
				if (cachedEmployeesTenant == tenant && cachedEmployees.Count > 0)
				{
						return cachedEmployees;
				}

				List<EmployeeSummary> employees = [];
				int skip = 0;

				while (true)
				{
						PagedResult<EmployeeSummary> page = await employeeApiService
							.GetEmployeesAsync(skip, BatchSize, cancellationToken, tenant)
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

				cachedEmployees = employees;
				cachedEmployeesTenant = tenant;
				return cachedEmployees;
		}

		private async Task<IReadOnlyList<PayrollSummary>> LoadPayrollsInPeriodAsync(string? chargingName, CancellationToken cancellationToken)
		{
				DashboardPayrollCacheKey cacheKey = new(SelectedTenant, PayrollPeriodStart, PayrollPeriodEnd, chargingName);
				if (payrollCache.TryGetValue(cacheKey, out IReadOnlyList<PayrollSummary>? cachedPayrolls))
				{
						return cachedPayrolls;
				}

				List<PayrollSummary> payrolls = [];
				int skip = 0;

				while (true)
				{
						PagedResult<PayrollSummary> page = await payrollApiService
							.GetPayrollsAsync(PayrollPeriodStart, PayrollPeriodEnd, chargingName, skip: skip, take: BatchSize, tenant: SelectedTenant, cancellationToken: cancellationToken)
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

				payrollCache[cacheKey] = payrolls;
				return payrolls;
		}


		private async Task<IReadOnlyList<EmployeeTimesheetSummary>> LoadTimesheetsInPeriodAsync(string? chargingName, CancellationToken cancellationToken)
		{
				DashboardTimesheetCacheKey cacheKey = new(SelectedTenant, PayrollPeriodStart, PayrollPeriodEnd, chargingName);
				if (timesheetCache.TryGetValue(cacheKey, out IReadOnlyList<EmployeeTimesheetSummary>? cachedTimesheets))
				{
						return cachedTimesheets;
				}

				List<EmployeeTimesheetSummary> timesheets = [];
				int skip = 0;

				while (true)
				{
						PagedResult<EmployeeTimesheetSummary> page = await timesheetApiService
							.GetTimesheetSummariesAsync(chargingName, PayrollPeriodStart, PayrollPeriodEnd, SelectedTenant, skip: skip, take: BatchSize, cancellationToken: cancellationToken)
							.ConfigureAwait(false);

						if (page.Items.Count == 0)
						{
								break;
						}

						timesheets.AddRange(page.Items);
						skip += page.Items.Count;

						if (timesheets.Count >= page.TotalCount)
						{
								break;
						}
				}

				timesheetCache[cacheKey] = timesheets;
				return timesheets;
		}

		private static IReadOnlyList<EmployeeSummary> FilterEmployees(IReadOnlyList<EmployeeSummary> source, string? department, string? charging)
		{
				if (string.IsNullOrWhiteSpace(department) && string.IsNullOrWhiteSpace(charging))
				{
						return source;
				}

        return source
          .Where(employee => MatchesCharging(employee.Charging, charging))
          .ToList();
  }

		private static IReadOnlyList<PayrollSummary> FilterPayrolls(IReadOnlyList<PayrollSummary> source, string? department, string? charging)
		{
				if (string.IsNullOrWhiteSpace(department) && string.IsNullOrWhiteSpace(charging))
				{
						return source;
				}

        return source
          .Where(payroll => MatchesCharging(payroll.Charging, charging))
          .ToList();
  }

		private static IReadOnlyList<EmployeeTimesheetSummary> FilterTimesheets(IReadOnlyList<EmployeeTimesheetSummary> source, string? department, string? charging)
		{
				if (string.IsNullOrWhiteSpace(department) && string.IsNullOrWhiteSpace(charging))
				{
						return source;
				}

        return source
        .Where(timesheet => MatchesCharging(timesheet.Charging, charging))
        .ToList();
    }

		private IReadOnlyList<LeaveSummary> FilterLeaves(IReadOnlyList<LeaveSummary> leaves, IReadOnlyList<EmployeeSummary> scopedEmployees)
		{
				HashSet<Guid> employeeIds = scopedEmployees
					.Select(employee => employee.Id)
					.ToHashSet();

				return leaves
					.Where(leave => leave.EmployeeId.HasValue
						&& employeeIds.Contains(leave.EmployeeId.Value)
						&& IsWithinSelectedPeriod(leave))
					.ToList();
		}

		private bool IsWithinSelectedPeriod(LeaveSummary leave)
		{
				if (!leave.StartDate.HasValue || !leave.EndDate.HasValue)
				{
						return false;
				}

				DateOnly start = DateOnly.FromDateTime(leave.StartDate.Value);
				DateOnly end = DateOnly.FromDateTime(leave.EndDate.Value);

				if (end < start)
				{
						(start, end) = (end, start);
				}

				return start <= PayrollPeriodEnd && end >= PayrollPeriodStart;
		}

		private static bool MatchesDepartment(string? value, string? selectedDepartment)
		{
				return string.IsNullOrWhiteSpace(selectedDepartment)
					|| string.Equals(value, selectedDepartment, StringComparison.OrdinalIgnoreCase);
		}

		private static bool MatchesCharging(string? value, string? selectedCharging)
		{
				return string.IsNullOrWhiteSpace(selectedCharging)
					|| string.Equals(value, selectedCharging, StringComparison.OrdinalIgnoreCase);
		}

		private DashboardComputationResult ComputeDashboard(
			IReadOnlyList<EmployeeSummary> employees,
			IReadOnlyList<PayrollSummary> payrolls,
			IReadOnlyList<EmployeeTimesheetSummary> timesheets,
			IReadOnlyList<LeaveSummary> leaves,
			IReadOnlyList<HolidaySummary> holidays,
			DateOnly todayDate)
		{
				List<HolidaySummary> upcomingHolidays = BuildUpcomingHolidays(holidays, todayDate);

				return new DashboardComputationResult
				{
						TotalActiveEmployees = employees.Count(employee => employee.Status == Status.Active),
						NewHiresInPeriod = employees.Count(employee =>
						{
								DateTime? dateHired = employee.EmploymentInfo.DateHired;
								return dateHired.HasValue && IsWithinPeriod(DateOnly.FromDateTime(dateHired.Value));
						}),
						EmployeesTerminatedInPeriod = employees.Count(employee =>
						{
								DateTime? dateTerminated = employee.EmploymentInfo.DateTerminated;
								return dateTerminated.HasValue && IsWithinPeriod(DateOnly.FromDateTime(dateTerminated.Value));
						}),
						TotalWorkHoursLogged = timesheets.Sum(timesheet => timesheet.TotalWorkHours),
						TotalOvertimeHours = payrolls.Sum(payroll =>
							payroll.OvertimeHrs
							+ payroll.OvertimeRestDayHrs
							+ payroll.OvertimeRegularHolidayHrs
							+ payroll.OvertimeSpecialHolidayHrs
							+ payroll.NightDiffOvertimeHrs),
						TotalAbsences = payrolls.Sum(payroll => payroll.Absences),
						TotalLateInstances = payrolls.Sum(payroll => payroll.Lates),
						TotalUndertime = payrolls.Sum(payroll => payroll.Undertime),
						LeaveMetrics = ComputeLeaveMetrics(leaves, PayrollPeriodStart, PayrollPeriodEnd),
						PayrollCostByDepartment = BuildDepartmentPayrollCostBreakdown(payrolls),
						PayrollGeneratedStatusByCharging = BuildChargingPayrollGenerationBreakdown(payrolls),
						UpcomingHolidays = upcomingHolidays
				};
		}

		private async Task ApplyDashboardResultAsync(DashboardComputationResult result)
		{
				await MainThread.InvokeOnMainThreadAsync(() =>
				{
						TotalActiveEmployees = result.TotalActiveEmployees;
						NewHiresInPeriod = result.NewHiresInPeriod;
						EmployeesTerminatedInPeriod = result.EmployeesTerminatedInPeriod;
						TotalWorkHoursLogged = result.TotalWorkHoursLogged;
						TotalOvertimeHours = result.TotalOvertimeHours;
						TotalAbsences = result.TotalAbsences;
						TotalLateInstances = result.TotalLateInstances;
						TotalUndertime = result.TotalUndertime;
						TotalLeaveDaysTaken = result.LeaveMetrics.TotalLeaveDaysTaken;
						SickLeaveDays = result.LeaveMetrics.SickLeaveDays;
						VacationLeaveDays = result.LeaveMetrics.VacationLeaveDays;
						OtherLeaveTypesDays = result.LeaveMetrics.OtherLeaveTypesDays;

						PayrollCostByDepartment.Clear();
						foreach (DepartmentPayrollCostSummary item in result.PayrollCostByDepartment)
						{
								PayrollCostByDepartment.Add(item);
						}

						PayrollGeneratedStatusByCharging.Clear();
						foreach (ChargingPayrollGenerationSummary item in result.PayrollGeneratedStatusByCharging)
						{
								PayrollGeneratedStatusByCharging.Add(item);
						}

						UpcomingHolidays.Clear();
						foreach (HolidaySummary holiday in result.UpcomingHolidays)
						{
								UpcomingHolidays.Add(holiday);
						}
				});
		}

		private bool IsWithinPeriod(DateOnly date)
		{
				return date >= PayrollPeriodStart && date <= PayrollPeriodEnd;
		}

		private static DashboardLeaveMetrics ComputeLeaveMetrics(IReadOnlyList<LeaveSummary> leaves, DateOnly periodStart, DateOnly periodEnd)
		{
				IReadOnlyList<LeaveSummary> approvedLeaves = leaves
					.Where(leave => leave.Status == ApplicationStatusDto.Approved)
					.ToList();

				decimal totalLeaveDaysTaken = approvedLeaves.Sum(leave => CalculateLeaveDaysWithinPeriod(leave, periodStart, periodEnd));
				decimal sickLeaveDays = approvedLeaves
					.Where(leave => IsMatchingLeaveType(leave.LeaveCreditName, "sl"))
					.Sum(leave => CalculateLeaveDaysWithinPeriod(leave, periodStart, periodEnd));

				decimal vacationLeaveDays = approvedLeaves
					.Where(leave => IsMatchingLeaveType(leave.LeaveCreditName, "vl"))
					.Sum(leave => CalculateLeaveDaysWithinPeriod(leave, periodStart, periodEnd));

				return new DashboardLeaveMetrics
				{
						TotalLeaveDaysTaken = totalLeaveDaysTaken,
						SickLeaveDays = sickLeaveDays,
						VacationLeaveDays = vacationLeaveDays,
						OtherLeaveTypesDays = totalLeaveDaysTaken - sickLeaveDays - vacationLeaveDays
				};
		}

		private static List<DepartmentPayrollCostSummary> BuildDepartmentPayrollCostBreakdown(IReadOnlyList<PayrollSummary> payrolls)
		{
				return payrolls
					.GroupBy(payroll => payroll.Department ?? "Unassigned")
					.OrderBy(group => group.Key)
					.Select(group => new DepartmentPayrollCostSummary
					{
							Department = group.Key,
							TotalPayrollCost = group.Sum(payroll => payroll.NetAmount),
							EmployeeCount = group.Select(payroll => payroll.EmployeeId).Where(id => id.HasValue).Distinct().Count()
					})
					.ToList();
		}

		private static List<ChargingPayrollGenerationSummary> BuildChargingPayrollGenerationBreakdown(IReadOnlyList<PayrollSummary> payrolls)
		{
				return payrolls
					.GroupBy(payroll => payroll.Charging ?? "Unassigned")
					.OrderBy(group => group.Key)
					.Select(group => new ChargingPayrollGenerationSummary
					{
							Charging = group.Key,
							GeneratedCount = group.Count(payroll => payroll.IsSaved),
							PendingCount = group.Count(payroll => !payroll.IsSaved)
					})
					.ToList();
		}

		private List<HolidaySummary> BuildUpcomingHolidays(IReadOnlyList<HolidaySummary> holidays, DateOnly _todayDate)
		{
				DateTime periodStart = PayrollPeriodStart.ToDateTime(TimeOnly.MinValue);
				DateTime periodEnd = PayrollPeriodEnd.ToDateTime(TimeOnly.MaxValue);

				return holidays
					.Where(holiday => holiday.Date >= periodStart && holiday.Date <= periodEnd)
					.OrderBy(holiday => holiday.Date)
					.ToList();
		}

		private static decimal CalculateLeaveDaysWithinPeriod(LeaveSummary leave, DateOnly periodStart, DateOnly periodEnd)
		{
				if (!leave.StartDate.HasValue || !leave.EndDate.HasValue)
				{
						return 0m;
				}

				DateOnly start = DateOnly.FromDateTime(leave.StartDate.Value.Date);
				DateOnly end = DateOnly.FromDateTime(leave.EndDate.Value.Date);

				if (end < start)
				{
						(start, end) = (end, start);
				}

				DateOnly clampedStart = start > periodStart ? start : periodStart;
				DateOnly clampedEnd = end < periodEnd ? end : periodEnd;
				if (clampedEnd < clampedStart)
				{
						return 0m;
				}

				decimal daySpan = clampedEnd.DayNumber - clampedStart.DayNumber + 1m;
				return leave.IsHalfDay ? 0.5m : daySpan;
		}

		private static bool IsMatchingLeaveType(string leaveTypeName, string token)
		{
				return leaveTypeName.Contains(token, StringComparison.OrdinalIgnoreCase);
		}

		private static string? NormalizeChargingName(string? chargingName) => string.IsNullOrWhiteSpace(chargingName) ? null : chargingName;

		private static string? NormalizeDepartmentName(string? departmentName) => string.IsNullOrWhiteSpace(departmentName) ? null : departmentName;

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
				if (!hasInitialized || suppressSelectionChanged)
				{
						return;
				}

				suppressSelectionChanged = true;
				try
				{
						RefreshChargingOptions(SelectedCharging?.Id);
				}
				finally
				{
						suppressSelectionChanged = false;
				}

				_ = LoadDashboardAsync();
		}

		partial void OnSelectedChargingChanged(ChargingSummary? value)
		{
				if (!hasInitialized || suppressSelectionChanged)
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
				Preferences.Set(nameof(SelectedTenant), value.ToString());
				ResetCachedData();
				_ = LoadDashboardAsync();
		}

		private void CancelPendingDashboardLoad()
		{
				try
				{
						dashboardLoadCancellationTokenSource?.Cancel();
				}
				catch (ObjectDisposedException)
				{
				}
		}

		private void ResetCachedData()
		{
				cachedEmployees = Array.Empty<EmployeeSummary>();
				cachedEmployeesTenant = null;
				ClearPayrollCache();
		}

		private void ClearPayrollCache()
		{
				payrollCache.Clear();
				timesheetCache.Clear();
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

		private void RefreshChargingOptions(Guid? preferredChargingId = null)
		{
				IEnumerable<ChargingSummary> availableChargings = referenceDataService.Chargings;

				if (SelectedDepartment is not null)
				{
						availableChargings = availableChargings.Where(charging => charging.DepartmentId == SelectedDepartment.Id);
				}

				List<ChargingSummary> filteredChargings = availableChargings.ToList();
				if (filteredChargings.Count == 0)
				{
						filteredChargings = referenceDataService.Chargings.ToList();
				}

				ChargingOptions.Clear();
				foreach (ChargingSummary charging in filteredChargings)
				{
						ChargingOptions.Add(charging);
				}

				SelectedCharging = preferredChargingId.HasValue
						? ChargingOptions.FirstOrDefault(option => option.Id == preferredChargingId.Value) ?? ChargingOptions.FirstOrDefault()
						: ChargingOptions.FirstOrDefault();
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

internal sealed record DashboardPayrollCacheKey(
	TenantDto Tenant,
	DateOnly PayrollPeriodStart,
	DateOnly PayrollPeriodEnd,
	string? ChargingName);

internal sealed record DashboardTimesheetCacheKey(
	TenantDto Tenant,
	DateOnly PayrollPeriodStart,
	DateOnly PayrollPeriodEnd,
	string? ChargingName);

internal sealed class DashboardComputationResult
{
		public int TotalActiveEmployees { get; init; }

		public int NewHiresInPeriod { get; init; }

		public int EmployeesTerminatedInPeriod { get; init; }

		public decimal TotalWorkHoursLogged { get; init; }

		public decimal TotalOvertimeHours { get; init; }

		public int TotalAbsences { get; init; }

		public decimal TotalLateInstances { get; init; }

		public decimal TotalUndertime { get; init; }

		public DashboardLeaveMetrics LeaveMetrics { get; init; } = new();

		public IReadOnlyList<DepartmentPayrollCostSummary> PayrollCostByDepartment { get; init; } = Array.Empty<DepartmentPayrollCostSummary>();

		public IReadOnlyList<ChargingPayrollGenerationSummary> PayrollGeneratedStatusByCharging { get; init; } = Array.Empty<ChargingPayrollGenerationSummary>();

		public IReadOnlyList<HolidaySummary> UpcomingHolidays { get; init; } = Array.Empty<HolidaySummary>();
}

internal sealed class DashboardLeaveMetrics
{
		public decimal TotalLeaveDaysTaken { get; init; }

		public decimal SickLeaveDays { get; init; }

		public decimal VacationLeaveDays { get; init; }

		public decimal OtherLeaveTypesDays { get; init; }
}

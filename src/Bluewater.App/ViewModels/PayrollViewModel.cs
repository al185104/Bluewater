using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using Bluewater.App.Extensions;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using Bluewater.App.Views.Modals;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

public partial class PayrollViewModel : BaseViewModel
{
		private const int PageSize = 24;
		private const int DownloadBatchSize = 200;
		private readonly IPayrollApiService payrollApiService;
		private readonly IReferenceDataService referenceDataService;
		private bool hasInitialized;
		private bool isInitializing;
		private bool hasPendingPayrollsInPeriod;
		private int payrollCountInPeriod;

		public PayrollViewModel(
			IPayrollApiService payrollApiService,
			IReferenceDataService referenceDataService,
			IActivityTraceService activityTraceService,
			IExceptionHandlingService exceptionHandlingService)
			: base(activityTraceService, exceptionHandlingService)
		{
				this.payrollApiService = payrollApiService;
				this.referenceDataService = referenceDataService;
				SetCurrentPayslipPeriod();
				EditablePayroll = CreateNewPayroll();
		}

		public ObservableCollection<PayrollSummary> Payrolls { get; } = new();
		public ObservableCollection<int> PageNumbers { get; } = new();
		public ObservableCollection<string> ChargingOptions { get; } = new();

		[ObservableProperty]
		public partial int CurrentPage { get; set; } = 1;

		[ObservableProperty]
		public partial int TotalCount { get; set; }

		public int TotalPages => TotalCount == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

		public bool HasPagination => TotalPages > 0;

		public bool CanSavePayrollPeriod => !IsBusy && hasPendingPayrollsInPeriod;

		public bool CanDownloadPayrollPeriod => !IsBusy && payrollCountInPeriod > 0 && !hasPendingPayrollsInPeriod;

		[ObservableProperty]
		public partial DateOnly StartDate { get; set; }

		[ObservableProperty]
		public partial DateOnly EndDate { get; set; }

		public string PeriodRangeDisplay => $"{StartDate:MMMM dd} - {EndDate:MMMM dd}";

		[ObservableProperty]
		public partial string? ChargingFilter { get; set; }

		[ObservableProperty]
		public partial PayrollSummary? SelectedPayroll { get; set; }

		[ObservableProperty]
		public partial PayrollSummary EditablePayroll { get; set; }

		public override async Task InitializeAsync()
		{
				if (hasInitialized || isInitializing)
				{
						return;
				}

				try
				{
						isInitializing = true;
						await TraceCommandAsync(nameof(InitializeAsync));
						await LoadChargingOptionsAsync();
						await LoadPayrollsAsync();
						hasInitialized = true;
				}
				finally
				{
						isInitializing = false;
				}
		}

		[RelayCommand]
		private async Task RefreshAsync()
		{
				try
				{
						await TraceCommandAsync(nameof(RefreshAsync));
						CurrentPage = 1;
						await LoadPayrollsAsync();
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Refreshing payroll");
				}
		}

		[RelayCommand(CanExecute = nameof(CanChangePeriod))]
		private async Task PreviousPeriodAsync()
		{
				try
				{
						SetPreviousPayslipPeriod();
						CurrentPage = 1;
						await TraceCommandAsync(nameof(PreviousPeriodAsync));
						await LoadPayrollsAsync();
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Loading previous payroll period");
				}
		}

		[RelayCommand(CanExecute = nameof(CanChangePeriod))]
		private async Task NextPeriodAsync()
		{
				try
				{
						SetNextPayslipPeriod();
						CurrentPage = 1;
						await TraceCommandAsync(nameof(NextPeriodAsync));
						await LoadPayrollsAsync();
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Loading next payroll period");
				}
		}

		[RelayCommand]
		private async Task GoToPageAsync(int page)
		{
				try
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
						await TraceCommandAsync(nameof(GoToPageAsync), new { page }).ConfigureAwait(false);
						await LoadPayrollsAsync().ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, $"Navigating to payroll page {page}");
				}
		}

		private bool CanChangePeriod() => !IsBusy;

		private async Task LoadChargingOptionsAsync()
		{
				ChargingOptions.Clear();

				foreach (ChargingSummary charging in referenceDataService.Chargings.OrderBy(c => c.Name))
				{
						if (!string.IsNullOrWhiteSpace(charging.Name))
						{
								ChargingOptions.Add(charging.Name);
						}
				}
				if (ChargingOptions.Count > 1)
						ChargingFilter = ChargingOptions.FirstOrDefault();
		}

		[RelayCommand]
		private void BeginCreatePayroll()
		{
				_ = TraceCommandAsync(nameof(BeginCreatePayroll));
				EditablePayroll = CreateNewPayroll();
				SelectedPayroll = null;
		}

		[RelayCommand]
		private async Task BeginEditPayrollAsync(PayrollSummary? payroll)
		{
				try
				{
						if (payroll is null)
						{
								return;
						}

						await TraceCommandAsync(nameof(BeginEditPayrollAsync), payroll.Id).ConfigureAwait(false);
						SelectedPayroll = payroll;
						EditablePayroll = payroll;

						await Shell.Current.GoToAsync(
								nameof(PayrollDetailsPage),
								new Dictionary<string, object>
								{
										["Payroll"] = payroll
								});
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Opening payroll details");
				}
		}

		[RelayCommand(CanExecute = nameof(CanSavePayrollPeriod))]
		private async Task SavePayrollAsync()
		{
				IReadOnlyList<(PayrollSummary payroll, int index)> pendingPayrolls = GetPendingPayrolls(Payrolls);

				if (pendingPayrolls.Count == 0)
				{
						return;
				}

				try
				{
						IsBusy = true;

						int savedCount = await SavePayrollEntriesAsync(pendingPayrolls).ConfigureAwait(false);

						EditablePayroll = CreateNewPayroll();
						await LoadPayrollsAsync().ConfigureAwait(false);

						await MainThread.InvokeOnMainThreadAsync(() =>
								Snackbar.Make(
										$"Saved {savedCount} payroll record(s) for {PeriodRangeDisplay}. Skipped {pendingPayrolls.Count - savedCount} record(s).",
										duration: TimeSpan.FromSeconds(3))
									.Show());

						await TraceCommandAsync(nameof(SavePayrollAsync), new
						{
								Attempted = pendingPayrolls.Count,
								Saved = savedCount,
								Skipped = pendingPayrolls.Count - savedCount
						}).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Saving payroll period");
				}
				finally
				{
						IsBusy = false;
						UpdatePayrollCommandStates();
				}
		}

		[RelayCommand]
		private async Task SubmitPayrollAsync(PayrollSummary? payroll)
		{
				if (IsBusy || payroll is null || payroll.IsSaved || !payroll.EmployeeId.HasValue || payroll.EmployeeId.Value == Guid.Empty)
				{
						return;
				}

				int index = Payrolls.IndexOf(payroll);
				if (index < 0)
				{
						return;
				}

				SelectedPayroll = payroll;

				try
				{
						IsBusy = true;

						int savedCount = await SavePayrollEntriesAsync([(payroll, index)]).ConfigureAwait(false);

						await RefreshPayrollPeriodCompletionStateAsync().ConfigureAwait(false);
						UpdatePayrollCommandStates();

						if (savedCount > 0)
						{
								PayrollSummary refreshedPayroll = Payrolls[index];
								SelectedPayroll = refreshedPayroll;
								if (ReferenceEquals(EditablePayroll, payroll))
								{
										EditablePayroll = refreshedPayroll;
								}

								await MainThread.InvokeOnMainThreadAsync(() =>
										Snackbar.Make($"Submitted payroll entry for {refreshedPayroll.Name}.", duration: TimeSpan.FromSeconds(3)).Show());
						}

						await TraceCommandAsync(nameof(SubmitPayrollAsync), new
						{
								payroll.Id,
								payroll.EmployeeId,
								Saved = savedCount > 0
						}).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Submitting payroll entry");
				}
				finally
				{
						IsBusy = false;
						UpdatePayrollCommandStates();
				}
		}

		[RelayCommand(CanExecute = nameof(CanDownloadPayrollPeriod))]
		private async Task DownloadPayrollsAsync()
		{
				if (IsBusy)
				{
						return;
				}

				try
				{
            IsBusy = true;
            IReadOnlyList<PayrollSummary> payrollPeriod = await LoadPayrollsForDownloadAsync().ConfigureAwait(false);

						if (payrollPeriod.Count == 0)
						{
								await MainThread.InvokeOnMainThreadAsync(() =>
										Shell.Current.DisplayAlert("Download", "No payroll records to download.", "Okay"));
								return;
						}

						string chargingName = string.IsNullOrWhiteSpace(ChargingFilter) ? "All" : ChargingFilter;
						bool confirmed = await MainThread.InvokeOnMainThreadAsync(() => Shell.Current.DisplayAlert(
								"Download payroll",
								$"Download {chargingName} payroll records for {PeriodRangeDisplay} to your Downloads folder?",
								"Yes",
								"No"));

						if (!confirmed)
						{
								return;
						}

						StringBuilder csv = new();
						csv.AppendLine(string.Join(",", GetPayrollExportHeaders()));

						foreach (PayrollSummary item in payrollPeriod.OrderBy(item => item.Name).ThenBy(item => item.Date))
						{
								csv.AppendLine(string.Join(",", GetPayrollExportRow(item)));
						}

						string fileName = $"payroll_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
						string downloadsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
						Directory.CreateDirectory(downloadsDirectory);
						string filePath = Path.Combine(downloadsDirectory, fileName);
						await File.WriteAllTextAsync(filePath, csv.ToString(), Encoding.UTF8).ConfigureAwait(false);

						await MainThread.InvokeOnMainThreadAsync(() =>
								Shell.Current.DisplayAlert("Download", $"Payroll downloaded to {filePath}", "Okay"));

						await TraceCommandAsync(nameof(DownloadPayrollsAsync), new
						{
								StartDate,
								EndDate,
								Charging = chargingName,
								RecordCount = payrollPeriod.Count,
								FileName = fileName
						}).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Downloading payroll");
				}
				finally
				{
						IsBusy = false;
				}
		}

		[RelayCommand]
		private async Task DeletePayrollAsync(PayrollSummary? payroll)
		{
				if (payroll is null)
				{
						return;
				}

				try
				{
						IsBusy = true;

						bool deleted = await payrollApiService.DeletePayrollAsync(payroll.Id);

						if (deleted)
						{
								Payrolls.Remove(payroll);
								UpdatePayrollRowIndexes();
								await TraceCommandAsync(nameof(DeletePayrollAsync), payroll.Id);
						}
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Deleting payroll");
				}
				finally
				{
						IsBusy = false;
				}
		}

		public override void IsBusyChanged(bool isBusy)
		{
				base.IsBusyChanged(isBusy);
				RaiseNavigationState();
				UpdatePayrollCommandStates();
		}

		private async Task LoadPayrollsAsync()
		{
				try
				{
						IsBusy = true;

						int skip = (CurrentPage - 1) * PageSize;
						PagedResult<PayrollSummary> page = await payrollApiService
							.GetPayrollsAsync(StartDate, EndDate, ChargingFilter, skip, PageSize)
							.ConfigureAwait(false);

						UpdatePagination(page.TotalCount);

						MainThread.BeginInvokeOnMainThread(() =>
						{ 
								Payrolls.Clear();
								foreach (PayrollSummary payroll in page.Items)
								{
										payroll.RowIndex = Payrolls.Count;
										Payrolls.Add(payroll);
								}

								payrollCountInPeriod = page.TotalCount;
						});

						await RefreshPayrollPeriodCompletionStateAsync().ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Loading payrolls");
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

		private static PayrollSummary CreateNewPayroll()
		{
				return new PayrollSummary
				{
						Id = Guid.Empty,
						Date = DateOnly.FromDateTime(DateTime.Today),
						RowIndex = 0
				};
		}

		private void SetCurrentPayslipPeriod(DateOnly? referenceDate = null)
		{
				DateOnly date = referenceDate ?? DateOnly.FromDateTime(DateTime.Today);
				(DateOnly startDate, DateOnly endDate) = CalculatePayslipPeriod(date);
				StartDate = startDate;
				EndDate = endDate;
		}

		private void SetPreviousPayslipPeriod()
		{
				SetCurrentPayslipPeriod(StartDate.AddDays(-1));
		}

		private void SetNextPayslipPeriod()
		{
				SetCurrentPayslipPeriod(EndDate.AddDays(1));
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


		private static IReadOnlyList<(PayrollSummary payroll, int index)> GetPendingPayrolls(IEnumerable<PayrollSummary> payrolls)
		{
				return payrolls
						.Select((payroll, index) => (payroll, index))
						.Where(item => !item.payroll.IsSaved && item.payroll.EmployeeId.HasValue && item.payroll.EmployeeId.Value != Guid.Empty)
						.ToList();
		}

		private async Task<int> SavePayrollEntriesAsync(IReadOnlyList<(PayrollSummary payroll, int index)> payrollEntries)
		{
				int savedCount = 0;

				foreach ((PayrollSummary payroll, int index) payrollEntry in payrollEntries)
				{
						if (payrollEntry.payroll.IsSaved)
						{
								continue;
						}

						Guid? newId = await payrollApiService.CreatePayrollAsync(payrollEntry.payroll).ConfigureAwait(false);
						if (!newId.HasValue)
						{
								continue;
						}

						PayrollSummary result = await payrollApiService.GetPayrollByIdAsync(newId.Value).ConfigureAwait(false) ?? payrollEntry.payroll;
						result.RowIndex = payrollEntry.index;
						await MainThread.InvokeOnMainThreadAsync(() => ReplacePayrollAtIndex(payrollEntry.index, payrollEntry.payroll, result));
						savedCount++;
				}

				await MainThread.InvokeOnMainThreadAsync(UpdatePayrollRowIndexes);
				return savedCount;
		}


		private void ReplacePayrollAtIndex(int index, PayrollSummary previousPayroll, PayrollSummary refreshedPayroll)
		{
				Payrolls[index] = refreshedPayroll;

				if (ReferenceEquals(SelectedPayroll, previousPayroll))
				{
						SelectedPayroll = refreshedPayroll;
				}

				if (ReferenceEquals(EditablePayroll, previousPayroll))
				{
						EditablePayroll = refreshedPayroll;
				}
		}

		private static string EscapeCsv(string? value)
		{
				if (string.IsNullOrEmpty(value))
				{
						return string.Empty;
				}

				if (value.Contains(',') || value.Contains('"') || value.Contains("\n", StringComparison.Ordinal) || value.Contains("\r", StringComparison.Ordinal))
				{
						return $"\"{value.Replace("\"", "\"\"")}\"";
				}

				return value;
		}

		private async Task<IReadOnlyList<PayrollSummary>> LoadPayrollsForDownloadAsync()
		{
				List<PayrollSummary> payrolls = [];
				int skip = 0;

				while (true)
				{
						PagedResult<PayrollSummary> page = await payrollApiService
								.GetPayrollsAsync(StartDate, EndDate, ChargingFilter, skip: skip, take: DownloadBatchSize)
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

		private static IReadOnlyList<string> GetPayrollExportHeaders()
		{
				return
				[
						"Employee Id",
						"Employee",
						"Barcode",
						"Bank Account",
						"Division",
						"Department",
						"Section",
						"Position",
						"Charging",
						"Date",
						"Gross Pay",
						"Net Pay",
						"Basic Pay",
						"SSS",
						"SSS ER",
						"PAGIBIG",
						"PAGIBIG ER",
						"Philhealth",
						"Philhealth ER",
						"Rest Day Amount",
						"Rest Day Hours",
						"Regular Holiday Amount",
						"Regular Holiday Hours",
						"Special Holiday Amount",
						"Special Holiday Hours",
						"Overtime Amount",
						"Overtime Hours",
						"Night Differential Amount",
						"Night Differential Hours",
						"Night Diff Overtime Amount",
						"Night Diff Overtime Hours",
						"Night Diff Regular Holiday Amount",
						"Night Diff Regular Holiday Hours",
						"Night Diff Special Holiday Amount",
						"Night Diff Special Holiday Hours",
						"Overtime Rest Day Amount",
						"Overtime Rest Day Hours",
						"Overtime Regular Holiday Amount",
						"Overtime Regular Holiday Hours",
						"Overtime Special Holiday Amount",
						"Overtime Special Holiday Hours",
						"Union Dues",
						"Absences",
						"Absences Amount",
						"Leaves",
						"Leaves Amount",
						"Lates",
						"Lates Amount",
						"Undertime",
						"Undertime Amount",
						"Overbreak",
						"Overbreak Amount",
						"Service Charge",
						"Cost Of Living Allowance",
						"Monthly Allowance",
						"Salary Underpayment",
						"Refund Absences",
						"Refund Undertime",
						"Refund Overtime",
						"Labor Hours Income",
						"Labor Hours",
						"Tax Deductions",
						"Total Constant Deductions",
						"Total Loan Deductions",
						"Total Deductions",
						"Saved"
				];
		}

		private static IReadOnlyList<string> GetPayrollExportRow(PayrollSummary item)
		{
				return
				[
						EscapeCsv(item.EmployeeId?.ToString()),
						EscapeCsv(item.Name),
						EscapeCsv(item.Barcode),
						EscapeCsv(item.BankAccount),
						EscapeCsv(item.Division),
						EscapeCsv(item.Department),
						EscapeCsv(item.Section),
						EscapeCsv(item.Position),
						EscapeCsv(item.Charging),
						EscapeCsv(item.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
						EscapeCsv(ToDecimalCsv(item.GrossPayAmount)),
						EscapeCsv(ToDecimalCsv(item.NetAmount)),
						EscapeCsv(ToDecimalCsv(item.BasicPayAmount)),
						EscapeCsv(ToDecimalCsv(item.SssAmount)),
						EscapeCsv(ToDecimalCsv(item.SssERAmount)),
						EscapeCsv(ToDecimalCsv(item.PagibigAmount)),
						EscapeCsv(ToDecimalCsv(item.PagibigERAmount)),
						EscapeCsv(ToDecimalCsv(item.PhilhealthAmount)),
						EscapeCsv(ToDecimalCsv(item.PhilhealthERAmount)),
						EscapeCsv(ToDecimalCsv(item.RestDayAmount)),
						EscapeCsv(ToDecimalCsv(item.RestDayHrs)),
						EscapeCsv(ToDecimalCsv(item.RegularHolidayAmount)),
						EscapeCsv(ToDecimalCsv(item.RegularHolidayHrs)),
						EscapeCsv(ToDecimalCsv(item.SpecialHolidayAmount)),
						EscapeCsv(ToDecimalCsv(item.SpecialHolidayHrs)),
						EscapeCsv(ToDecimalCsv(item.OvertimeAmount)),
						EscapeCsv(ToDecimalCsv(item.OvertimeHrs)),
						EscapeCsv(ToDecimalCsv(item.NightDiffAmount)),
						EscapeCsv(ToDecimalCsv(item.NightDiffHrs)),
						EscapeCsv(ToDecimalCsv(item.NightDiffOvertimeAmount)),
						EscapeCsv(ToDecimalCsv(item.NightDiffOvertimeHrs)),
						EscapeCsv(ToDecimalCsv(item.NightDiffRegularHolidayAmount)),
						EscapeCsv(ToDecimalCsv(item.NightDiffRegularHolidayHrs)),
						EscapeCsv(ToDecimalCsv(item.NightDiffSpecialHolidayAmount)),
						EscapeCsv(ToDecimalCsv(item.NightDiffSpecialHolidayHrs)),
						EscapeCsv(ToDecimalCsv(item.OvertimeRestDayAmount)),
						EscapeCsv(ToDecimalCsv(item.OvertimeRestDayHrs)),
						EscapeCsv(ToDecimalCsv(item.OvertimeRegularHolidayAmount)),
						EscapeCsv(ToDecimalCsv(item.OvertimeRegularHolidayHrs)),
						EscapeCsv(ToDecimalCsv(item.OvertimeSpecialHolidayAmount)),
						EscapeCsv(ToDecimalCsv(item.OvertimeSpecialHolidayHrs)),
						EscapeCsv(ToDecimalCsv(item.UnionDues)),
						EscapeCsv(item.Absences.ToString(CultureInfo.InvariantCulture)),
						EscapeCsv(ToDecimalCsv(item.AbsencesAmount)),
						EscapeCsv(ToDecimalCsv(item.Leaves)),
						EscapeCsv(ToDecimalCsv(item.LeavesAmount)),
						EscapeCsv(ToDecimalCsv(item.Lates)),
						EscapeCsv(ToDecimalCsv(item.LatesAmount)),
						EscapeCsv(ToDecimalCsv(item.Undertime)),
						EscapeCsv(ToDecimalCsv(item.UndertimeAmount)),
						EscapeCsv(ToDecimalCsv(item.Overbreak)),
						EscapeCsv(ToDecimalCsv(item.OverbreakAmount)),
						EscapeCsv(ToDecimalCsv(item.SvcCharge)),
						EscapeCsv(ToDecimalCsv(item.CostOfLivingAllowanceAmount)),
						EscapeCsv(ToDecimalCsv(item.MonthlyAllowanceAmount)),
						EscapeCsv(ToDecimalCsv(item.SalaryUnderpaymentAmount)),
						EscapeCsv(ToDecimalCsv(item.RefundAbsencesAmount)),
						EscapeCsv(ToDecimalCsv(item.RefundUndertimeAmount)),
						EscapeCsv(ToDecimalCsv(item.RefundOvertimeAmount)),
						EscapeCsv(ToDecimalCsv(item.LaborHoursIncome)),
						EscapeCsv(ToDecimalCsv(item.LaborHrs)),
						EscapeCsv(ToDecimalCsv(item.TaxDeductions)),
						EscapeCsv(ToDecimalCsv(item.TotalConstantDeductions)),
						EscapeCsv(ToDecimalCsv(item.TotalLoanDeductions)),
						EscapeCsv(ToDecimalCsv(item.TotalDeductions)),
						EscapeCsv(item.IsSaved ? "Yes" : "No")
				];
		}

		private static string ToDecimalCsv(decimal value)
		{
				return value.ToString("0.00", CultureInfo.InvariantCulture);
		}

		private void UpdatePayrollRowIndexes()
		{
				Payrolls.UpdateRowIndexes();
		}

		partial void OnChargingFilterChanged(string? value)
		{
				if (!hasInitialized || isInitializing)
				{
						return;
				}

				CurrentPage = 1;
				_ = LoadPayrollsAsync();
		}

		partial void OnStartDateChanged(DateOnly value)
		{
				OnPropertyChanged(nameof(PeriodRangeDisplay));
		}

		partial void OnEndDateChanged(DateOnly value)
		{
				OnPropertyChanged(nameof(PeriodRangeDisplay));
		}

		private void UpdatePayrollCommandStates()
		{
				OnPropertyChanged(nameof(CanSavePayrollPeriod));
				OnPropertyChanged(nameof(CanDownloadPayrollPeriod));
				//SavePayrollCommand.NotifyCanExecuteChanged();
		}

		private async Task RefreshPayrollPeriodCompletionStateAsync()
		{
				PagedResult<PayrollSummary> payrollPeriodState = await payrollApiService.GetPayrollsAsync(StartDate, EndDate, ChargingFilter)
						.ConfigureAwait(false);

				payrollCountInPeriod = payrollPeriodState.TotalCount;
				hasPendingPayrollsInPeriod = payrollPeriodState.Items.Any(payroll => !payroll.IsSaved);
		}

		private void RaiseNavigationState()
		{
				void UpdateNavigationCommands()
				{
						PreviousPeriodCommand.NotifyCanExecuteChanged();
						NextPeriodCommand.NotifyCanExecuteChanged();
				}

				if (Microsoft.Maui.ApplicationModel.MainThread.IsMainThread)
				{
						UpdateNavigationCommands();
						return;
				}

				Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(UpdateNavigationCommands);
		}
}

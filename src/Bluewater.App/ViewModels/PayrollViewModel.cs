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
		private readonly IPayrollApiService payrollApiService;
		private readonly IReferenceDataService referenceDataService;
		private bool hasInitialized;
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
				if (hasInitialized)
				{
						return;
				}

				hasInitialized = true;
				await TraceCommandAsync(nameof(InitializeAsync));
				await LoadChargingOptionsAsync();
				await LoadPayrollsAsync();
		}

		[RelayCommand]
		private async Task RefreshAsync()
		{
				await TraceCommandAsync(nameof(RefreshAsync));
				CurrentPage = 1;
				await LoadPayrollsAsync();
		}

		[RelayCommand(CanExecute = nameof(CanChangePeriod))]
		private async Task PreviousPeriodAsync()
		{
				SetPreviousPayslipPeriod();
				CurrentPage = 1;
				await TraceCommandAsync(nameof(PreviousPeriodAsync));
				await LoadPayrollsAsync();
		}

		[RelayCommand(CanExecute = nameof(CanChangePeriod))]
		private async Task NextPeriodAsync()
		{
				SetNextPayslipPeriod();
				CurrentPage = 1;
				await TraceCommandAsync(nameof(NextPeriodAsync));
				await LoadPayrollsAsync();
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
				await LoadPayrollsAsync().ConfigureAwait(false);
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
				EditablePayroll = CreateNewPayroll();
				SelectedPayroll = null;
		}

		[RelayCommand]
		private async Task BeginEditPayrollAsync(PayrollSummary? payroll)
		{
				if (payroll is null)
				{
						return;
				}

				SelectedPayroll = payroll;
				EditablePayroll = payroll;

				await Shell.Current.GoToAsync(
						nameof(PayrollDetailsPage),
						new Dictionary<string, object>
						{
								["Payroll"] = payroll
						});
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
						PagedResult<PayrollSummary> payrollPeriod = await payrollApiService
								.GetPayrollsAsync(StartDate, EndDate, ChargingFilter)
								.ConfigureAwait(false);

						if (payrollPeriod.TotalCount == 0 || payrollPeriod.Items.Count == 0)
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

						IsBusy = true;

						StringBuilder csv = new();
						csv.AppendLine(string.Join(",", new[]
						{
								"Employee",
								"Barcode",
								"Department",
								"Section",
								"Position",
								"Charging",
								"Date",
								"Gross Pay",
								"Net Pay",
								"Tax",
								"Deductions",
								"Saved"
						}));

						foreach (PayrollSummary item in payrollPeriod.Items.OrderBy(item => item.Name).ThenBy(item => item.Date))
						{
								csv.AppendLine(string.Join(",", new[]
								{
										EscapeCsv(item.Name),
										EscapeCsv(item.Barcode),
										EscapeCsv(item.Department),
										EscapeCsv(item.Section),
										EscapeCsv(item.Position),
										EscapeCsv(item.Charging),
										EscapeCsv(item.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
										EscapeCsv(item.GrossPayAmount.ToString("0.00", CultureInfo.InvariantCulture)),
										EscapeCsv(item.NetAmount.ToString("0.00", CultureInfo.InvariantCulture)),
										EscapeCsv(item.TaxDeductions.ToString("0.00", CultureInfo.InvariantCulture)),
										EscapeCsv(item.TotalDeductions.ToString("0.00", CultureInfo.InvariantCulture)),
										EscapeCsv(item.IsSaved ? "Yes" : "No")
								}));
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
								RecordCount = payrollPeriod.Items.Count,
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

		private void UpdatePayrollRowIndexes()
		{
				Payrolls.UpdateRowIndexes();
		}

		partial void OnChargingFilterChanged(string? value)
		{
				if (!hasInitialized)
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

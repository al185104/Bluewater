using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using Bluewater.App.Views;
using Bluewater.App.Views.Modals;
using Bluewater.Core.EmployeeAggregate;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

public partial class TimesheetsViewModel : BaseViewModel
{
		private const string DefaultDetailsPrimaryActionText = "Save Changes";
		private const int PageSize = 24;

		private readonly ITimesheetApiService timesheetApiService;
		private readonly IAttendanceApiService attendanceApiService;
		private readonly IReferenceDataService referenceDataService;
		private bool hasInitialized;
		private bool suppressSelectedChargingChanged;

		public TimesheetsViewModel(
			ITimesheetApiService timesheetApiService,
			IAttendanceApiService attendanceApiService,
			IReferenceDataService referenceDataService,
			IActivityTraceService activityTraceService,
			IExceptionHandlingService exceptionHandlingService)
			: base(activityTraceService, exceptionHandlingService)
		{
				this.timesheetApiService = timesheetApiService;
				this.attendanceApiService = attendanceApiService;
				this.referenceDataService = referenceDataService;

				SetCurrentPayslipPeriod();
		}

		public ObservableCollection<ChargingSummary> Chargings { get; } = new();
		public ObservableCollection<EmployeeTimesheetSummary> Timesheets { get; } = new();
		public ObservableCollection<EditableTimesheetEntry> EditableTimesheets { get; } = new();
		public ObservableCollection<int> PageNumbers { get; } = new();

		[ObservableProperty]
		public partial int CurrentPage { get; set; } = 1;

		[ObservableProperty]
		public partial int TotalCount { get; set; }

		public int TotalPages => TotalCount == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

		public bool HasPagination => TotalPages > 0;
		public IReadOnlyList<TenantDto> TenantOptions { get; } = Array.AsReadOnly(Enum.GetValues<TenantDto>());

		[ObservableProperty]
		public partial ChargingSummary? SelectedCharging { get; set; }

		[ObservableProperty]
		public partial TenantDto SelectedTenant { get; set; } = TenantDto.Maribago;

		[ObservableProperty]
		public partial DateOnly StartDate { get; set; }

		[ObservableProperty]
		public partial DateOnly EndDate { get; set; }

		public string PeriodRangeDisplay => $"{StartDate:MMMM dd} - {EndDate:MMMM dd}";

		//[ObservableProperty]
		//public partial bool IsDetailsOpen { get; set; }

		//[ObservableProperty]
		//public partial string DetailsTitle { get; set; } = string.Empty;

		//[ObservableProperty]
		//public partial string DetailsPrimaryActionText { get; set; } = DefaultDetailsPrimaryActionText;

		[ObservableProperty]
		public partial EmployeeTimesheetSummary? SelectedEmployeeTimesheet { get; set; }

		[ObservableProperty]
		public partial EditableTimesheetEntry? SelectedEditableTimesheet { get; set; }

		public bool CanSaveTimesheets => !IsBusy && EditableTimesheets.Any(entry => entry.HasChanges);
		public bool CanSubmit => !IsBusy && Timesheets.Count > 0 && Timesheets.Any(summary => !summary.HasPayrollCreated);

		public override void IsBusyChanged(bool isBusy)
		{
				base.IsBusyChanged(isBusy);
				UpdateCanSaveTimesheets();
				UpdateCanSubmit();
				RaiseNavigationState();
		}

		public override async Task InitializeAsync()
		{
				if (hasInitialized)
				{
						await LoadTimesheetsAsync().ConfigureAwait(false);
						return;
				}

				hasInitialized = true;
				await TraceCommandAsync(nameof(InitializeAsync)).ConfigureAwait(false);

				MainThread.BeginInvokeOnMainThread(async () => {
						LoadChargings();
						if (SelectedCharging is not null)
						{
								await LoadTimesheetsAsync().ConfigureAwait(false);
						}
				});

		}

		[RelayCommand]
		private async Task RefreshAsync()
		{
				try
				{
						await TraceCommandAsync(nameof(RefreshAsync)).ConfigureAwait(false);
						await LoadTimesheetsAsync().ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Refreshing timesheets");
				}
		}

		[RelayCommand(CanExecute = nameof(CanChangePeriod))]
		private async Task PreviousPeriodAsync()
		{
				try
				{
						SetPreviousPayslipPeriod();
						CurrentPage = 1;
						await TraceCommandAsync(nameof(PreviousPeriodAsync)).ConfigureAwait(false);
						await LoadTimesheetsAsync().ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Loading previous timesheet period");
				}
		}

		[RelayCommand(CanExecute = nameof(CanChangePeriod))]
		private async Task NextPeriodAsync()
		{
				try
				{
						SetNextPayslipPeriod();
						CurrentPage = 1;
						await TraceCommandAsync(nameof(NextPeriodAsync)).ConfigureAwait(false);
						await LoadTimesheetsAsync().ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Loading next timesheet period");
				}
		}

		private bool CanChangePeriod() => !IsBusy;

		[RelayCommand]
		private async Task EditTimesheetAsync(EmployeeTimesheetSummary? summary)
		{
				try
				{
						if (summary is null || summary.HasPayrollCreated)
						{
								return;
						}

						MainThread.BeginInvokeOnMainThread(() =>
						{
								SelectedEmployeeTimesheet = summary;
								LoadEditableTimesheets(summary);

						});

						await NavigateAsync(
								nameof(TimesheetDetailsPage),
								new Dictionary<string, object>
								{
										["EditableTimesheets"] = EditableTimesheets,
										["SelectedEditableTimesheet"] = SelectedEditableTimesheet!,
										["SelectedEmployeeTimesheet"] = SelectedEmployeeTimesheet!,
										["IsReadOnlyMode"] = false
								});

						await TraceCommandAsync(nameof(EditTimesheetAsync), summary.EmployeeId).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Opening timesheet details");
				}
		}

		[RelayCommand]
		private async Task ViewTimesheetAsync(EmployeeTimesheetSummary? summary)
		{
				try
				{
						if (summary is null || !summary.HasPayrollCreated)
						{
								return;
						}

						MainThread.BeginInvokeOnMainThread(() =>
						{
								SelectedEmployeeTimesheet = summary;
								LoadEditableTimesheets(summary);
						});

						await NavigateAsync(
								nameof(TimesheetDetailsPage),
								new Dictionary<string, object>
								{
										["EditableTimesheets"] = EditableTimesheets,
										["SelectedEditableTimesheet"] = SelectedEditableTimesheet!,
										["SelectedEmployeeTimesheet"] = SelectedEmployeeTimesheet!,
										["IsReadOnlyMode"] = true
								});

						await TraceCommandAsync(nameof(ViewTimesheetAsync), summary.EmployeeId).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Opening timesheet details in view mode");
				}
		}

		[RelayCommand]
		private async Task SaveTimesheetsAsync()
		{
				if (IsBusy || EditableTimesheets.Count == 0)
				{
						return;
				}

				List<EditableTimesheetEntry> entriesToUpdate = EditableTimesheets
					.Where(entry => entry.HasChanges)
					.ToList();

				if (entriesToUpdate.Count == 0)
				{
						return;
				}

				bool anyUpdated = false;

				try
				{
						IsBusy = true;

						foreach (EditableTimesheetEntry entry in entriesToUpdate)
						{
								try
								{
										AttendanceTimesheetSummary? updated = await timesheetApiService
											.UpdateTimesheetAsync(entry.ToUpdateRequest())
											.ConfigureAwait(false);

										if (updated is null)
										{
												continue;
										}

										await MainThread.InvokeOnMainThreadAsync(() =>
										{
												entry.ApplySummary(updated);
												UpdateSummaryEntry(updated);
										}).ConfigureAwait(false);

										anyUpdated = true;
								}
								catch (Exception ex)
								{
										ExceptionHandlingService.Handle(ex, "Updating timesheet");
								}
						}
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Updating timesheets");
				}
				finally
				{
						IsBusy = false;
						MainThread.BeginInvokeOnMainThread(UpdateCanSaveTimesheets);
				}

				if (anyUpdated)
				{
						await TraceCommandAsync(nameof(SaveTimesheetsAsync), SelectedEmployeeTimesheet?.EmployeeId)
							.ConfigureAwait(false);
				}
		}

		partial void OnSelectedChargingChanged(ChargingSummary? value)
		{
				if (!hasInitialized || suppressSelectedChargingChanged)
				{
						return;
				}

				CurrentPage = 1;
				_ = LoadTimesheetsAsync();
		}

		partial void OnSelectedTenantChanged(TenantDto value)
		{
				if (!hasInitialized)
				{
						return;
				}

				CurrentPage = 1;
				_ = LoadTimesheetsAsync();
		}



		[RelayCommand]
		private async Task DownloadTimesheetsAsync()
		{
				if (IsBusy)
				{
						return;
				}

				if (Timesheets.Count == 0)
				{
						await Shell.Current.DisplayAlert("Download", "No timesheets to download.", "Okay");
						return;
				}

				string chargingName = SelectedCharging?.Name ?? "All";
				bool confirmed = await Shell.Current.DisplayAlert(
						"Download timesheets",
						$"Download {chargingName} timesheets for {PeriodRangeDisplay} to your Downloads folder?",
						"Yes",
						"No");

				if (!confirmed)
				{
						return;
				}

				try
				{
						IsBusy = true;

						StringBuilder csv = new();
						csv.AppendLine(string.Join(",", new[]
						{
								"Employee",
								"Department",
								"Section",
								"Charging",
								"Date",
								"Time In 1",
								"Time Out 1",
								"Time In 2",
								"Time Out 2",
								"Shift",
								"Work Hours",
								"Break Hours",
								"Late Hours",
								"Undertime Hours",
								"Overbreak Hours",
								"Is Absent",
								"Is Leave"
						}));

						foreach (EmployeeTimesheetSummary summary in Timesheets)
						{
								foreach (AttendanceTimesheetSummary timesheet in summary.Timesheets.OrderBy(item => item.EntryDate))
								{
										csv.AppendLine(string.Join(",", new[]
										{
												EscapeCsv(summary.Name),
												EscapeCsv(summary.Department),
												EscapeCsv(summary.Section),
												EscapeCsv(summary.Charging),
												EscapeCsv(timesheet.EntryDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
												EscapeCsv(FormatDateTime(timesheet.TimeIn1)),
												EscapeCsv(FormatDateTime(timesheet.TimeOut1)),
												EscapeCsv(FormatDateTime(timesheet.TimeIn2)),
												EscapeCsv(FormatDateTime(timesheet.TimeOut2)),
												EscapeCsv(timesheet.ShiftName),
												EscapeCsv(CalculateWorkHours(timesheet).ToString("0.##", CultureInfo.InvariantCulture)),
												EscapeCsv(CalculateBreakHours(timesheet).ToString("0.##", CultureInfo.InvariantCulture)),
												EscapeCsv(string.Empty),
												EscapeCsv(string.Empty),
												EscapeCsv(string.Empty),
												EscapeCsv(string.Empty),
												EscapeCsv(string.Empty)
										}));
								}
						}

						string fileName = $"timesheets_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
						string downloadsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
						Directory.CreateDirectory(downloadsDirectory);
						string filePath = Path.Combine(downloadsDirectory, fileName);
						await File.WriteAllTextAsync(filePath, csv.ToString(), Encoding.UTF8).ConfigureAwait(false);

						await MainThread.InvokeOnMainThreadAsync(() =>
								Shell.Current.DisplayAlert("Download", $"Timesheets downloaded to {filePath}", "Okay"));

						await TraceCommandAsync(nameof(DownloadTimesheetsAsync), new
						{
								StartDate,
								EndDate,
								Charging = chargingName,
								Tenant = SelectedTenant.ToString(),
								EmployeeCount = Timesheets.Count,
								FileName = fileName
						}).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Downloading timesheets");
				}
				finally
				{
						IsBusy = false;
				}
		}


		private static decimal CalculateWorkHours(AttendanceTimesheetSummary timesheet)
		{
				if (timesheet.TimeIn1 is null || timesheet.TimeOut2 is null)
				{
						return 0m;
				}

				TimeSpan workDuration = timesheet.TimeOut2.Value - timesheet.TimeIn1.Value;
				if (workDuration <= TimeSpan.Zero)
				{
						return 0m;
				}

				return (decimal)workDuration.TotalHours - CalculateBreakHours(timesheet);
		}

		private static decimal CalculateBreakHours(AttendanceTimesheetSummary timesheet)
		{
				if (timesheet.TimeOut1 is null || timesheet.TimeIn2 is null)
				{
						return 0m;
				}

				TimeSpan breakDuration = timesheet.TimeIn2.Value - timesheet.TimeOut1.Value;
				if (breakDuration <= TimeSpan.Zero)
				{
						return 0m;
				}

				return (decimal)breakDuration.TotalHours;
		}


		private static string FormatDateTime(DateTime? value)
				=> value?.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) ?? string.Empty;

		private static string EscapeCsv(string? value)
		{
				if (string.IsNullOrEmpty(value))
				{
						return string.Empty;
				}

				if (value.Contains('"'))
				{
						value = value.Replace("\"", "\"\"");
				}

				if (value.Contains(',') || value.Contains("\n") || value.Contains("\r"))
				{
						return $"\"{value}\"";
				}

				return value;
		}

		[RelayCommand(CanExecute = nameof(CanSubmit))]
		private async Task SubmitAsync()
		{
				if (!CanSubmit)
				{
						return;
				}

				try
				{
						IsBusy = true;

						foreach (EmployeeTimesheetSummary summary in Timesheets)
						{
								foreach (AttendanceTimesheetSummary timesheet in summary.Timesheets.Where(item => item.EntryDate.HasValue))
								{
										try
										{
												await UpsertAttendanceAsync(summary.EmployeeId, timesheet).ConfigureAwait(false);
										}
										catch (Exception ex)
										{
												ExceptionHandlingService.Handle(ex, $"Submitting attendance for {summary.Name}");
										}
								}
						}

						await Snackbar.Make(
								"Timesheet and attendance have been successfully updated.",
								duration: TimeSpan.FromSeconds(3)
						).Show();

						await TraceCommandAsync(nameof(SubmitAsync), new
						{
								StartDate,
								EndDate,
								TimesheetCount = Timesheets.Count
						}).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Submitting attendance records");
				}
				finally
				{
						IsBusy = false;
				}
		}

		private async Task UpsertAttendanceAsync(Guid employeeId, AttendanceTimesheetSummary timesheet)
		{
				if (!timesheet.EntryDate.HasValue)
				{
						return;
				}

				DateOnly entryDate = timesheet.EntryDate.Value;
				IReadOnlyList<AttendanceSummary> attendances = await attendanceApiService
						.GetAttendancesAsync(employeeId, entryDate, entryDate)
						.ConfigureAwait(false);

				AttendanceSummary attendanceRequest = new()
				{
						EmployeeId = employeeId,
						ShiftId = timesheet.ShiftId,
						TimesheetId = timesheet.Id,
						EntryDate = entryDate,
						IsLocked = false
				};

				AttendanceSummary? existingAttendance = attendances
						.FirstOrDefault(attendance => attendance.EntryDate == entryDate);

				if (existingAttendance is null)
				{
						await attendanceApiService.CreateAttendanceAsync(attendanceRequest).ConfigureAwait(false);
						return;
				}

				attendanceRequest.Id = existingAttendance.Id;
				attendanceRequest.LeaveId = existingAttendance.LeaveId;
				attendanceRequest.IsLocked = existingAttendance.IsLocked;

				await attendanceApiService.UpdateAttendanceAsync(attendanceRequest).ConfigureAwait(false);
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
						await LoadTimesheetsAsync().ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, $"Navigating to timesheet page {page}");
				}
		}

		private async Task LoadTimesheetsAsync()
		{
				if (SelectedCharging is null)
				{
						await MainThread.InvokeOnMainThreadAsync(ClearTimesheets);
						return;
				}

				try
				{
						IsBusy = true;

						int skip = (CurrentPage - 1) * PageSize;
						PagedResult<EmployeeTimesheetSummary> page = await timesheetApiService
							.GetTimesheetSummariesAsync(
								SelectedCharging.Name,
								StartDate,
								EndDate,
								SelectedTenant,
								skip,
								PageSize)
							.ConfigureAwait(false);

						UpdatePagination(page.TotalCount);

						await MainThread.InvokeOnMainThreadAsync(() =>
						{
								Timesheets.Clear();
								foreach (EmployeeTimesheetSummary summary in page.Items)
								{
										UpdateTimesheetRowIndexes(summary);
										Timesheets.Add(summary);
								}

								//SyncSelectedTimesheetSummary();
								UpdateCanSubmit();
						});
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

		private void LoadChargings()
		{
				suppressSelectedChargingChanged = true;

				Guid? previousId = SelectedCharging?.Id;

				Chargings.Clear();
				foreach (ChargingSummary charging in referenceDataService.Chargings
										 .OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase))
				{
						Chargings.Add(charging);
				}

				if (previousId is Guid id)
				{
						ChargingSummary? previous = Chargings.FirstOrDefault(item => item.Id == id);
						if (previous is not null)
						{
								SelectedCharging = previous;
						}
				}

				if (SelectedCharging is null && Chargings.Count > 0)
				{
						SelectedCharging = Chargings[0];
				}

				suppressSelectedChargingChanged = false;
		}

		private void ClearTimesheets()
		{
				Timesheets.Clear();
				UpdateCanSubmit();
				UpdatePagination(0);
		}

		private void LoadEditableTimesheets(EmployeeTimesheetSummary summary)
		{
				ClearEditableTimesheets();

				foreach (AttendanceTimesheetSummary timesheet in summary.Timesheets)
				{
						EditableTimesheetEntry entry = EditableTimesheetEntry.FromSummary(timesheet);
						entry.PropertyChanged += OnEditableTimesheetPropertyChanged;
						EditableTimesheets.Add(entry);
				}

				OnPropertyChanged(nameof(EditableTimesheets));
				SelectedEditableTimesheet = EditableTimesheets.FirstOrDefault();
				UpdateCanSaveTimesheets();
		}

		private void ClearEditableTimesheets()
		{
				foreach (EditableTimesheetEntry entry in EditableTimesheets)
				{
						entry.PropertyChanged -= OnEditableTimesheetPropertyChanged;
				}

				EditableTimesheets.Clear();
				SelectedEditableTimesheet = null;
				UpdateCanSaveTimesheets();
		}

		private void OnEditableTimesheetPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
				UpdateCanSaveTimesheets();
		}

		private void UpdateSummaryEntry(AttendanceTimesheetSummary updatedTimesheet)
		{
				EmployeeTimesheetSummary? summary = Timesheets
					.FirstOrDefault(item => item.EmployeeId == updatedTimesheet.EmployeeId);

				if (summary is null)
				{
						return;
				}

				AttendanceTimesheetSummary? existing = summary.Timesheets
					.FirstOrDefault(item => item.Id == updatedTimesheet.Id);

				if (existing is null)
				{
						return;
				}

				existing.TimeIn1 = updatedTimesheet.TimeIn1;
				existing.TimeOut1 = updatedTimesheet.TimeOut1;
				existing.TimeIn2 = updatedTimesheet.TimeIn2;
				existing.TimeOut2 = updatedTimesheet.TimeOut2;
				existing.EntryDate = updatedTimesheet.EntryDate;
				existing.IsEdited = updatedTimesheet.IsEdited;
		}

		//private void SyncSelectedTimesheetSummary()
		//{
		//		if (!IsDetailsOpen || SelectedEmployeeTimesheet is null)
		//		{
		//				return;
		//		}

		//		EmployeeTimesheetSummary? updated = Timesheets
		//			.FirstOrDefault(item => item.EmployeeId == SelectedEmployeeTimesheet.EmployeeId);

		//		if (updated is null || ReferenceEquals(updated, SelectedEmployeeTimesheet))
		//		{
		//				return;
		//		}

		//		SelectedEmployeeTimesheet = updated;
		//}

		private static void UpdateTimesheetRowIndexes(EmployeeTimesheetSummary summary)
		{
				for (int i = 0; i < summary.Timesheets.Count; i++)
				{
						summary.Timesheets[i].RowIndex = i;
				}
		}

		private void UpdateCanSaveTimesheets()
		{
				OnPropertyChanged(nameof(CanSaveTimesheets));
		}

		private void UpdateCanSubmit()
		{
				OnPropertyChanged(nameof(CanSubmit));
				SubmitCommand.NotifyCanExecuteChanged();
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

		partial void OnStartDateChanged(DateOnly value)
		{
				OnPropertyChanged(nameof(PeriodRangeDisplay));
		}

		partial void OnEndDateChanged(DateOnly value)
		{
				OnPropertyChanged(nameof(PeriodRangeDisplay));
		}

		private void RaiseNavigationState()
		{
				void UpdateNavigationCommands()
				{
						PreviousPeriodCommand.NotifyCanExecuteChanged();
						NextPeriodCommand.NotifyCanExecuteChanged();
				}

				if (MainThread.IsMainThread)
				{
						UpdateNavigationCommands();
						return;
				}

				MainThread.BeginInvokeOnMainThread(UpdateNavigationCommands);
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

		public void Dispose()
		{
				// Unsubscribe PropertyChanged handlers from editable entries
				foreach (var entry in EditableTimesheets)
				{
						entry.PropertyChanged -= OnEditableTimesheetPropertyChanged;
				}

				// Clear collections
				EditableTimesheets.Clear();
				Timesheets.Clear();
				Chargings.Clear();
				PageNumbers.Clear();

				// Reset selections
				SelectedCharging = null;
				SelectedEmployeeTimesheet = null;
				SelectedEditableTimesheet = null;

				// Dispose services if applicable
				if (timesheetApiService is IDisposable d1)
						d1.Dispose();

				if (referenceDataService is IDisposable d2)
						d2.Dispose();
		}
}

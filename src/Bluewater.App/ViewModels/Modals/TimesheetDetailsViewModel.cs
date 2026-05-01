using System.Collections.ObjectModel;
using System.ComponentModel;
using Bluewater.App.Enums;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels.Modals;

public partial class TimesheetDetailsViewModel : BaseViewModel, IQueryAttributable
{
		private readonly ITimesheetApiService timesheetApiService;
		private readonly IShiftApiService shiftApiService;
		private readonly IScheduleApiService scheduleApiService;

		[ObservableProperty]
		public partial EditableTimesheetEntry? SelectedEditableTimesheet { get; set; }

		[ObservableProperty]
		public partial EmployeeTimesheetSummary? SelectedEmployeeTimesheet { get; set; }

		[ObservableProperty]
		public partial ShiftSummary? SelectedShift { get; set; }

		[ObservableProperty]
		public partial bool IsReadOnlyMode { get; set; }

		public ObservableCollection<EditableTimesheetEntry> EditableTimesheets { get; set; } = [];
		public ObservableCollection<ShiftSummary> ShiftOptions { get; } = [];
		public bool CanSaveTimesheets => !IsBusy && !IsReadOnlyMode && EditableTimesheets.Any(entry => entry.HasChanges);
		public bool CanEditTimesheets => !IsReadOnlyMode;

		public TimesheetDetailsViewModel(
				ITimesheetApiService timesheetApiService,
				IShiftApiService shiftApiService,
				IScheduleApiService scheduleApiService,
				IActivityTraceService activityTraceService,
				IExceptionHandlingService exceptionHandlingService)
				: base(activityTraceService, exceptionHandlingService)
		{
				this.timesheetApiService = timesheetApiService;
				this.shiftApiService = shiftApiService;
				this.scheduleApiService = scheduleApiService;
		}

		public override void IsBusyChanged(bool isBusy)
		{
				base.IsBusyChanged(isBusy);
				UpdateCanSaveTimesheets();
		}

		public void ApplyQueryAttributes(IDictionary<string, object> query)
		{
				ClearEditableTimesheets();

				if (query.TryGetValue("SelectedEditableTimesheet", out var value) && value is EditableTimesheetEntry editableTimesheet)
				{
						SelectedEditableTimesheet = editableTimesheet;
				}

				if (query.TryGetValue("EditableTimesheets", out var listObj) &&
						listObj is ObservableCollection<EditableTimesheetEntry> timesheetList)
				{
						foreach (EditableTimesheetEntry timesheet in timesheetList)
						{
								timesheet.PropertyChanged += OnEditableTimesheetPropertyChanged;
								EditableTimesheets.Add(timesheet);
						}
				}

				if (query.TryGetValue("SelectedEmployeeTimesheet", out var empTimesheet) &&
						empTimesheet is EmployeeTimesheetSummary selectedEmpTimesheet)
				{
						SelectedEmployeeTimesheet = selectedEmpTimesheet;
				}

				if (query.TryGetValue("IsReadOnlyMode", out var isReadOnly) && isReadOnly is bool valueIsReadOnly)
				{
						IsReadOnlyMode = valueIsReadOnly;
				}
				else
				{
						IsReadOnlyMode = false;
				}

				UpdateCanSaveTimesheets();
				InitializeCommand.Execute(this);
		}

		public override async Task InitializeAsync()
		{
				try
				{
						IReadOnlyList<ShiftSummary> shifts = await shiftApiService.GetShiftsAsync().ConfigureAwait(false);
						await MainThread.InvokeOnMainThreadAsync(() =>
						{
								ShiftOptions.Clear();
								foreach (ShiftSummary shift in shifts.OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase))
								{
										ShiftOptions.Add(shift);
								}
								SyncSelectedShift();
						}).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Loading shifts");
				}
		}

		[RelayCommand]
		private async Task ClearSelectedShiftAsync() 
		{
				if (IsBusy || IsReadOnlyMode || SelectedEditableTimesheet is null)
				{
						return;
				}

				EditableTimesheetEntry selectedEntry = SelectedEditableTimesheet;

				if (!selectedEntry.ShiftId.HasValue && !selectedEntry.ScheduleId.HasValue)
				{
						SelectedShift = null;
						return;
				}

				try
				{
						IsBusy = true;
						Guid? previousScheduleId = selectedEntry.ScheduleId;

						if (selectedEntry.ScheduleId is Guid scheduleId && scheduleId != Guid.Empty)
						{
								bool deleted = await scheduleApiService.DeleteScheduleAsync(scheduleId).ConfigureAwait(false);
								if (!deleted)
								{
										throw new InvalidOperationException("Failed to delete schedule.");
								}
						}

						selectedEntry.ScheduleId = null;
						selectedEntry.ShiftId = null;
						selectedEntry.ShiftName = null;

						AttendanceTimesheetSummary? updated = await timesheetApiService
								.UpdateTimesheetAsync(selectedEntry.ToUpdateRequest())
								.ConfigureAwait(false);

						if (updated is not null)
						{
								selectedEntry.ApplySummary(updated);
								UpdateSelectedEmployeeTimesheetEntry(updated);
						}

						await MainThread.InvokeOnMainThreadAsync(() =>
						{
								SelectedShift = null;
								UpdateCanSaveTimesheets();
						});

						await TraceCommandAsync(nameof(ClearSelectedShiftAsync), new
						{
								selectedEntry.EmployeeId,
								selectedEntry.EntryDate,
								PreviousScheduleId = previousScheduleId
						}).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Clearing selected shift");
				}
				finally
				{
						IsBusy = false;
						MainThread.BeginInvokeOnMainThread(UpdateCanSaveTimesheets);
				}
		}


		[RelayCommand]
		private async Task DeleteTimesheetAsync(EditableTimesheetEntry? timesheet)
		{
				if (IsBusy || IsReadOnlyMode || timesheet is null || timesheet.Id == Guid.Empty)
				{
						return;
				}

				bool confirmed = await MainThread.InvokeOnMainThreadAsync(() =>
						Shell.Current.DisplayAlert(
								"Delete Timesheet",
								"Are you sure you want to delete this timesheet?",
								"Delete",
								"Cancel"));

				if (!confirmed)
				{
						return;
				}

				try
				{
						IsBusy = true;
						bool deleted = await timesheetApiService.DeleteTimesheetAsync(timesheet.Id).ConfigureAwait(false);
						if (!deleted)
						{
								throw new InvalidOperationException("Failed to delete timesheet.");
						}

						await MainThread.InvokeOnMainThreadAsync(() =>
						{
								EditableTimesheets.Remove(timesheet);
								timesheet.PropertyChanged -= OnEditableTimesheetPropertyChanged;
								if (SelectedEditableTimesheet == timesheet)
								{
										SelectedEditableTimesheet = EditableTimesheets.FirstOrDefault();
								}
								if (SelectedEmployeeTimesheet is not null)
								{
										AttendanceTimesheetSummary? existing = SelectedEmployeeTimesheet.Timesheets
												.FirstOrDefault(t => t.Id == timesheet.Id);
										if (existing is not null)
										{
												SelectedEmployeeTimesheet.Timesheets.Remove(existing);
										}
										SelectedEmployeeTimesheet.IsShowAlert = SelectedEmployeeTimesheet.Timesheets.Any(ShouldShowAlertForTimesheet);
								}
								UpdateCanSaveTimesheets();
						});

						await TraceCommandAsync(nameof(DeleteTimesheetAsync), timesheet.Id).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Deleting timesheet");
				}
				finally
				{
						IsBusy = false;
				}
		}

		[RelayCommand(CanExecute = nameof(CanSaveTimesheets))]
		private async Task SaveTimesheetsAsync()
		{
				if (IsBusy || IsReadOnlyMode || EditableTimesheets.Count == 0)
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

								entry.ApplySummary(updated);
								UpdateSelectedEmployeeTimesheetEntry(updated);
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
            await MainThread.InvokeOnMainThreadAsync(() =>
                Shell.Current.DisplayAlert(
                    "Timesheet Updated",
                    "Timesheet has been successfully updated.",
                    "OK"));
						await NavigateAsync("..",
								new Dictionary<string, object>
								{
										["TargetSection"] = MainSectionEnum.Timesheet
								});

						await TraceCommandAsync(nameof(SaveTimesheetsAsync), SelectedEmployeeTimesheet?.EmployeeId).ConfigureAwait(false);
				}
		}

		partial void OnSelectedEditableTimesheetChanged(EditableTimesheetEntry? value)
		{
				SyncSelectedShift();
		}

		partial void OnIsReadOnlyModeChanged(bool value)
		{
				UpdateCanSaveTimesheets();
		}

		partial void OnSelectedShiftChanged(ShiftSummary? value)
		{
				if (SelectedEditableTimesheet is null)
				{
						return;
				}

				SelectedEditableTimesheet.ShiftId = value?.Id;
				SelectedEditableTimesheet.ShiftName = value?.Name;
		}

		private void SyncSelectedShift()
		{
				if (SelectedEditableTimesheet?.ShiftId is not Guid shiftId)
				{
						SelectedShift = null;
						return;
				}

				SelectedShift = ShiftOptions.FirstOrDefault(s => s.Id == shiftId);
		}

		private void UpdateCanSaveTimesheets()
		{
				OnPropertyChanged(nameof(CanSaveTimesheets));
				OnPropertyChanged(nameof(CanEditTimesheets));
				SaveTimesheetsCommand.NotifyCanExecuteChanged();
		}

		private void OnEditableTimesheetPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
				if (e.PropertyName == nameof(EditableTimesheetEntry.HasChanges))
				{
						UpdateCanSaveTimesheets();
				}
		}

		private void ClearEditableTimesheets()
		{
				foreach (EditableTimesheetEntry entry in EditableTimesheets)
				{
						entry.PropertyChanged -= OnEditableTimesheetPropertyChanged;
				}

				EditableTimesheets.Clear();
		}

		private void UpdateSelectedEmployeeTimesheetEntry(AttendanceTimesheetSummary updatedTimesheet)
		{
				if (SelectedEmployeeTimesheet is null)
				{
						return;
				}

				AttendanceTimesheetSummary? existing = SelectedEmployeeTimesheet.Timesheets
					.FirstOrDefault(timesheet => timesheet.Id == updatedTimesheet.Id);

				if (existing is null)
				{
						existing = SelectedEmployeeTimesheet.Timesheets.FirstOrDefault(timesheet =>
								timesheet.EmployeeId == updatedTimesheet.EmployeeId &&
								timesheet.EntryDate == updatedTimesheet.EntryDate);
				}

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
				existing.ScheduleId = updatedTimesheet.ScheduleId;
				existing.ShiftId = updatedTimesheet.ShiftId;
				existing.ShiftName = updatedTimesheet.ShiftName;
				SelectedEmployeeTimesheet.IsShowAlert = SelectedEmployeeTimesheet.Timesheets.Any(ShouldShowAlertForTimesheet);
		}

		private static bool ShouldShowAlertForTimesheet(AttendanceTimesheetSummary timesheet)
		{
				int mask = 0;

				if (timesheet.TimeOut2.HasValue)
				{
						mask |= 1 << 3;
				}

				if (timesheet.TimeIn2.HasValue)
				{
						mask |= 1 << 2;
				}

				if (timesheet.TimeOut1.HasValue)
				{
						mask |= 1 << 1;
				}

				if (timesheet.TimeIn1.HasValue)
				{
						mask |= 1;
				}

				return mask is not (0 or 3 or 9 or 12 or 15);
		}
}

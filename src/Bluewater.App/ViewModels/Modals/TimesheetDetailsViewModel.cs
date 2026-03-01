using System.Collections.ObjectModel;
using System.ComponentModel;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels.Modals;

public partial class TimesheetDetailsViewModel : BaseViewModel, IQueryAttributable
{
		private readonly ITimesheetApiService timesheetApiService;

		[ObservableProperty]
		public partial EditableTimesheetEntry? SelectedEditableTimesheet { get; set; }

		[ObservableProperty]
		public partial EmployeeTimesheetSummary? SelectedEmployeeTimesheet { get; set; }

		public ObservableCollection<EditableTimesheetEntry> EditableTimesheets { get; set; } = [];
		public bool CanSaveTimesheets => !IsBusy && EditableTimesheets.Any(entry => entry.HasChanges);

		public TimesheetDetailsViewModel(
				ITimesheetApiService timesheetApiService,
				IActivityTraceService activityTraceService,
				IExceptionHandlingService exceptionHandlingService)
				: base(activityTraceService, exceptionHandlingService)
		{
				this.timesheetApiService = timesheetApiService;
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

				UpdateCanSaveTimesheets();
				InitializeCommand.Execute(this);
		}

		[RelayCommand(CanExecute = nameof(CanSaveTimesheets))]
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

										entry.ApplySummary(updated);
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
						await TraceCommandAsync(nameof(SaveTimesheetsAsync), SelectedEmployeeTimesheet?.EmployeeId).ConfigureAwait(false);
				}
		}

		private void UpdateCanSaveTimesheets()
		{
				OnPropertyChanged(nameof(CanSaveTimesheets));
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
}

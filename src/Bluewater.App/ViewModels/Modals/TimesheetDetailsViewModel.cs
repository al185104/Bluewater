using System.Collections.ObjectModel;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bluewater.App.ViewModels.Modals;

public partial class TimesheetDetailsViewModel : BaseViewModel, IQueryAttributable
{
		[ObservableProperty]
		public partial EditableTimesheetEntry? SelectedEditableTimesheet { get; set; }
		[ObservableProperty]
		public partial EmployeeTimesheetSummary? SelectedEmployeeTimesheet { get; set; }
		public ObservableCollection<EditableTimesheetEntry> EditableTimesheets { get; set; } = [];
		public TimesheetDetailsViewModel(
				IActivityTraceService activityTraceService, 
				IExceptionHandlingService exceptionHandlingService) 
				: base(activityTraceService, exceptionHandlingService)
		{
		}

		public void ApplyQueryAttributes(IDictionary<string, object> query)
		{
				if (query.TryGetValue("SelectedEditableTimesheet", out var value) && value is EditableTimesheetEntry editableTimesheet)
				{
						SelectedEditableTimesheet = editableTimesheet;
				}

				if (query.TryGetValue("EditableTimesheets", out var listObj) &&
						listObj is ObservableCollection<EditableTimesheetEntry> timesheetList)
				{
						foreach (var timesheet in timesheetList) {
								EditableTimesheets.Add(timesheet);
						}

				}

				if (query.TryGetValue("SelectedEmployeeTimesheet", out var empTimesheet) &&
						empTimesheet is EmployeeTimesheetSummary selectedEmpTimesheet)
				{
						SelectedEmployeeTimesheet = selectedEmpTimesheet;
				}
				
				InitializeCommand.Execute(this);
		}
}

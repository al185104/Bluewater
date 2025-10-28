using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

public partial class EmployeeViewModel : BaseViewModel
{
		private readonly IEmployeeApiService employeeApiService;
		private bool hasInitialized;
		private const string DefaultPrimaryActionText = "Save Employee";

		[ObservableProperty]
		public partial EditableEmployee? EditableEmployee { get; set; }

		[ObservableProperty]
		public partial bool IsEditorOpen { get; set; }

		[ObservableProperty]
		public partial string EditorTitle { get; set; } = string.Empty;

		[ObservableProperty]
		public partial string EditorPrimaryActionText { get; set; } = DefaultPrimaryActionText;

		public EmployeeViewModel(
			IEmployeeApiService employeeApiService,
			IActivityTraceService activityTraceService,
			IExceptionHandlingService exceptionHandlingService)
			: base(activityTraceService, exceptionHandlingService)
		{
				this.employeeApiService = employeeApiService;
		}

		public ObservableCollection<EmployeeSummary> Employees { get; } = new();

		[RelayCommand]
		private async Task EditEmployeeAsync(EmployeeSummary? employee)
		{
				if (employee is null)
				{
						return;
				}

				EditableEmployee = EditableEmployee.FromSummary(employee);
				EditorTitle = $"Edit {EditableEmployee.FullName}";
				EditorPrimaryActionText = "Update Employee";
				IsEditorOpen = true;

				await TraceCommandAsync(nameof(EditEmployeeAsync), employee.Id);
		}

                [RelayCommand]
                private async Task UpdateEmployeeAsync()
                {
                                if (EditableEmployee is null)
                                {
                                                return;
                                }

                                EmployeeSummary? existing = Employees.FirstOrDefault(e => e.Id == EditableEmployee.Id);

                                if (existing is null)
                                {
                                                CloseEditor();
                                                return;
                                }

                                bool updateSucceeded = false;

                                try
                                {
                                                IsBusy = true;

                                                UpdateEmployeeRequestDto request = EditableEmployee.ToUpdateRequest(existing);
                                                EmployeeSummary? updated = await employeeApiService
                                                                .UpdateEmployeeAsync(request, existing);

                                                EmployeeSummary result = updated ?? EditableEmployee.ToSummary(existing.RowIndex);
                                                int index = Employees.IndexOf(existing);
                                                Employees[index] = result;
                                                updateSucceeded = true;

                                                await TraceCommandAsync(nameof(UpdateEmployeeAsync), result.Id);
                                }
                                catch (Exception ex)
                                {
                                                ExceptionHandlingService.Handle(ex, "Updating employee");
                                }
                                finally
                                {
                                                IsBusy = false;
                                }

                                if (updateSucceeded)
                                {
                                                CloseEditor();
                                }
                }

		[RelayCommand]
		private void CloseEditor()
		{
				IsEditorOpen = false;
				EditorTitle = string.Empty;
				EditorPrimaryActionText = DefaultPrimaryActionText;
				EditableEmployee = null;
		}

		[RelayCommand]
		private async Task DeleteEmployeeAsync(EmployeeSummary? employee)
		{
				if (employee is null)
				{
						return;
				}

				await TraceCommandAsync(nameof(DeleteEmployeeAsync), employee.Id);
		}

		public override async Task InitializeAsync()
		{
				if (IsBusy || hasInitialized)
				{
						return;
				}

				try
				{
						IsBusy = true;
						Employees.Clear();
						IReadOnlyList<EmployeeSummary> employees = await employeeApiService.GetEmployeesAsync();

						int index = 0;

						foreach (EmployeeSummary employee in employees
							.OrderBy(e => e.LastName ?? string.Empty)
							.ThenBy(e => e.FirstName ?? string.Empty))
						{
								employee.RowIndex = index++;
								Employees.Add(employee);
						}

						hasInitialized = true;
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Loading employees");
				}
				finally
				{
						IsBusy = false;
				}
		}
}

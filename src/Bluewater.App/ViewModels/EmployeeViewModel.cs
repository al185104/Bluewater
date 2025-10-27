using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

                public ObservableCollection<EmployeeExternalOption> UserOptions { get; } = new();

                public ObservableCollection<EmployeeExternalOption> PositionOptions { get; } = new();

                public ObservableCollection<EmployeeExternalOption> PayOptions { get; } = new();

                public ObservableCollection<EmployeeExternalOption> TypeOptions { get; } = new();

                public ObservableCollection<EmployeeExternalOption> LevelOptions { get; } = new();

                public ObservableCollection<EmployeeExternalOption> ChargingOptions { get; } = new();

		[RelayCommand]
		private async Task EditEmployeeAsync(EmployeeSummary? employee)
		{
				if (employee is null)
				{
						return;
				}

                                EditableEmployee = EditableEmployee.FromSummary(employee);
                                ApplySelectedExternalOptions(EditableEmployee);
				EditorTitle = $"Edit {EditableEmployee.FullName}";
				EditorPrimaryActionText = "Update Employee";
				IsEditorOpen = true;

				await TraceCommandAsync(nameof(EditEmployeeAsync), employee.Id);
		}

		[RelayCommand]
		private void UpdateEmployee()
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

				int index = Employees.IndexOf(existing);
                                EmployeeSummary updated = EditableEmployee.ToSummary(existing.RowIndex);
                                Employees[index] = updated;
                                UpdateExternalOptions();

				CloseEditor();
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

                private void UpdateExternalOptions()
                {
                                PopulateOptions(UserOptions, Employees.Select(e => (e.UserId, e.FullName)), "User");
                                PopulateOptions(PositionOptions, Employees.Select(e => (e.PositionId, e.Position)), "Position");
                                PopulateOptions(PayOptions, Employees.Select(e => (e.PayId, e.PayId?.ToString())), "Pay");
                                PopulateOptions(TypeOptions, Employees.Select(e => (e.TypeId, e.Type)), "Type");
                                PopulateOptions(LevelOptions, Employees.Select(e => (e.LevelId, e.Level)), "Level");
                                PopulateOptions(ChargingOptions, Employees.Select(e => (e.ChargingId, e.ChargingId?.ToString())), "Charging");

                                if (EditableEmployee is not null)
                                {
                                                ApplySelectedExternalOptions(EditableEmployee);
                                }
                }

                private void ApplySelectedExternalOptions(EditableEmployee employee)
                {
                                employee.SelectedPositionOption = FindOption(PositionOptions, employee.PositionId, employee.Position);
                                employee.SelectedTypeOption = FindOption(TypeOptions, employee.TypeId, employee.Type);
                                employee.SelectedLevelOption = FindOption(LevelOptions, employee.LevelId, employee.Level);
                                employee.SelectedChargingOption = FindOption(ChargingOptions, employee.ChargingId, null);
                }

                private static EmployeeExternalOption? FindOption(IEnumerable<EmployeeExternalOption> options, Guid? id, string? label)
                {
                                if (id.HasValue)
                                {
                                                EmployeeExternalOption? matchById = options.FirstOrDefault(option => option.Id == id);

                                                if (matchById is not null)
                                                {
                                                                return matchById;
                                                }
                                }

                                if (!string.IsNullOrWhiteSpace(label))
                                {
                                                EmployeeExternalOption? matchByLabel = options.FirstOrDefault(option => string.Equals(option.Label, label, StringComparison.OrdinalIgnoreCase));

                                                if (matchByLabel is not null)
                                                {
                                                                return matchByLabel;
                                                }
                                }

                                return options.FirstOrDefault(option => option.IsPlaceholder);
                }

                private static void PopulateOptions(
                                ObservableCollection<EmployeeExternalOption> target,
                                IEnumerable<(Guid? Id, string? Label)> source,
                                string placeholderLabel)
                {
                                List<EmployeeExternalOption> items = new();
                                HashSet<string> seen = new(StringComparer.OrdinalIgnoreCase);

                                foreach ((Guid? Id, string? Label) entry in source)
                                {
                                                if (entry.Id is null && string.IsNullOrWhiteSpace(entry.Label))
                                                {
                                                                continue;
                                                }

                                                string display = !string.IsNullOrWhiteSpace(entry.Label)
                                                        ? entry.Label!.Trim()
                                                        : entry.Id?.ToString() ?? string.Empty;

                                                if (string.IsNullOrWhiteSpace(display))
                                                {
                                                                continue;
                                                }

                                                string key = entry.Id?.ToString() ?? display;

                                                if (!seen.Add(key))
                                                {
                                                                continue;
                                                }

                                                items.Add(new EmployeeExternalOption(entry.Id, display));
                                }

                                items.Sort((left, right) => string.Compare(left.Label, right.Label, StringComparison.OrdinalIgnoreCase));

                                target.Clear();
                                target.Add(EmployeeExternalOption.CreateEmpty($"Select {placeholderLabel}"));

                                foreach (EmployeeExternalOption option in items)
                                {
                                                target.Add(option);
                                }
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

                                                UpdateExternalOptions();

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

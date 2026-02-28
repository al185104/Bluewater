using System.Collections.ObjectModel;
using Bluewater.App.Enums;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels.Modals;

public partial class EmployeeDetailsViewModel : BaseViewModel, IQueryAttributable
{
		private readonly IReferenceDataService _referenceDataService;
		private readonly IEmployeeApiService _employeeApiService;
		private int _rowIndex;

		[ObservableProperty]
		public partial EditableEmployee EditableEmployee { get; set; } = new();

		public ObservableCollection<ChargingSummary> ChargingOptions { get; set; } = new();
		[ObservableProperty]
		public partial ChargingSummary? Charging { get; set; }

		public ObservableCollection<DivisionSummary> DivisionOptions { get; set; } = new();
		[ObservableProperty]
		public partial DivisionSummary? Division { get; set; }

		public ObservableCollection<DepartmentSummary> DepartmentOptions { get; set; } = new();
		[ObservableProperty]
		public partial DepartmentSummary? Department { get; set; }

		public ObservableCollection<SectionSummary> SectionOptions { get; set; } = new();
		[ObservableProperty]
		public partial SectionSummary? Section { get; set; }

		public ObservableCollection<PositionSummary> PositionOptions { get; set; } = new();
		[ObservableProperty]
		public partial PositionSummary? Position { get; set; }

		public ObservableCollection<EmployeeTypeSummary> TypeOptions { get; set; } = new();
		[ObservableProperty]
		public partial EmployeeTypeSummary? EmployeeType { get; set; }
		
		public ObservableCollection<LevelSummary> LevelOptions { get; set; } = new();
		[ObservableProperty]
		public partial LevelSummary? Level { get; set; }

		[ObservableProperty]
		public partial bool ShowPositionValidation { get; set; }

		[ObservableProperty]
		public partial bool ShowChargingValidation { get; set; }

		[ObservableProperty]
		public partial bool ShowEmployeeTypeValidation { get; set; }

		[ObservableProperty]
		public partial bool ShowLevelValidation { get; set; }

		public EmployeeDetailsViewModel(
				IActivityTraceService activityTraceService, 
				IExceptionHandlingService exceptionHandlingService, 
				IReferenceDataService referenceDataService,
				IEmployeeApiService employeeApiService) 
				: base(activityTraceService, exceptionHandlingService)
		{
				_referenceDataService = referenceDataService;
				_employeeApiService = employeeApiService;
		}

		public void ApplyQueryAttributes(IDictionary<string, object> query)
		{
				if (query.TryGetValue("Employee", out var value) && value is EmployeeSummary passedEmployee)
				{
						_rowIndex = passedEmployee.RowIndex;
						EditableEmployee = EditableEmployee.FromSummary(passedEmployee);
						InitializeCommand.Execute(this);
				}
		}

		[RelayCommand]
		public async Task UpdateEmployeeAsync()
		{
				try
				{
						IsBusy = true;
						if (!ValidateRequiredPickers())
								return;

						if(EditableEmployee != null)
						{
								var summary = EditableEmployee.ToSummary(_rowIndex);
								UpdateEmployeeRequestDto request = EditableEmployee.ToUpdateRequest(summary);
								var updated = await _employeeApiService.UpdateEmployeeAsync(request, summary);
								if (updated != null) {
										await Snackbar.Make(
												"Employee has been successfully updated.",
												duration: TimeSpan.FromSeconds(3)
										).Show();
										await Shell.Current.GoToAsync("..",
												new Dictionary<string, object>
												{
														["TargetSection"] = MainSectionEnum.Employees
												});
								}
						}
				}
				finally
				{
						IsBusy = false;
				}
		}

		private bool ValidateRequiredPickers()
		{
				ShowPositionValidation = Position == null;
				ShowChargingValidation = Charging == null;
				ShowEmployeeTypeValidation = EmployeeType == null;
				ShowLevelValidation = Level == null;

				return !(ShowPositionValidation || ShowChargingValidation || ShowEmployeeTypeValidation || ShowLevelValidation);
		}

		[RelayCommand]
		public async Task CancelAsync()
		{
				try
				{
						IsBusy = true;
						await Shell.Current.GoToAsync("..");
				}
				finally
				{
						IsBusy = false;
				}
		}

		public override Task InitializeAsync()
		{
				if (EditableEmployee is null)
						return base.InitializeAsync();

				if (!_referenceDataService.Chargings.Any() ||
						!_referenceDataService.Positions.Any() ||
						!_referenceDataService.EmployeeTypes.Any() ||
						!_referenceDataService.Levels.Any())
				{
						return InitializeWithReferenceDataAsync();
				}

				BindReferenceData();
				return base.InitializeAsync();
		}

		private async Task InitializeWithReferenceDataAsync()
		{
				await _referenceDataService.InitializeAsync();
				BindReferenceData();
				await base.InitializeAsync();
		}

		private void BindReferenceData()
		{
				ChargingOptions.Clear();
				DivisionOptions.Clear();
				DepartmentOptions.Clear();
				SectionOptions.Clear();
				PositionOptions.Clear();
				TypeOptions.Clear();
				LevelOptions.Clear();

				foreach(var charge in _referenceDataService.Chargings)
						ChargingOptions.Add(charge);
				
				foreach(var division in _referenceDataService.Divisions)
						DivisionOptions.Add(division);

				foreach(var department in _referenceDataService.Departments)
						DepartmentOptions.Add(department);

				foreach(var section in _referenceDataService.Sections)
						SectionOptions.Add(section);

				foreach(var position in _referenceDataService.Positions)
						PositionOptions.Add(position);

				foreach (var type in _referenceDataService.EmployeeTypes)
						TypeOptions.Add(type);

				foreach (var level in _referenceDataService.Levels)
						LevelOptions.Add(level);

				EmployeeType = TypeOptions.FirstOrDefault(i => i.Id == EditableEmployee!.TypeId)
						?? TypeOptions.FirstOrDefault(i => i.Name == EditableEmployee.Type);
				EditableEmployee.Type = EmployeeType?.Name;
				EditableEmployee.TypeId ??= EmployeeType?.Id;

				Level = LevelOptions.FirstOrDefault(i => i.Id == EditableEmployee.LevelId)
						?? LevelOptions.FirstOrDefault(i => i.Name == EditableEmployee.Level);
				EditableEmployee.Level = Level?.Name;
				EditableEmployee.LevelId ??= Level?.Id;

				Charging = ChargingOptions.FirstOrDefault(i => i.Id == EditableEmployee.ChargingId);
				EditableEmployee.ChargingId ??= Charging?.Id;

				Position = PositionOptions.FirstOrDefault(i => i.Id == EditableEmployee.PositionId)
						?? PositionOptions.FirstOrDefault(i => i.Name == EditableEmployee.Position);
				EditableEmployee.Position = Position?.Name;
				EditableEmployee.PositionId ??= Position?.Id;

				Section = Position == null
						? null
						: SectionOptions.FirstOrDefault(i => i.Id == Position.SectionId);
				Department = Section == null
						? null
						: DepartmentOptions.FirstOrDefault(i => i.Id == Section.DepartmentId);
				Division = Department == null
						? null
						: DivisionOptions.FirstOrDefault(i => i.Id == Department.DivisionId);

		}


		partial void OnChargingChanged(ChargingSummary? value)
		{
				if (EditableEmployee == null)
						return;

				EditableEmployee.ChargingId = value?.Id;
				ShowChargingValidation = value == null;
		}

		partial void OnEmployeeTypeChanged(EmployeeTypeSummary? value)
		{
				if (EditableEmployee == null)
						return;

				EditableEmployee.Type = value?.Name;
				EditableEmployee.TypeId = value?.Id;
				ShowEmployeeTypeValidation = value == null;
		}

		partial void OnLevelChanged(LevelSummary? value)
		{
				if (EditableEmployee == null)
						return;

				EditableEmployee.Level = value?.Name;
				EditableEmployee.LevelId = value?.Id;
				ShowLevelValidation = value == null;
		}

		partial void OnPositionChanged(PositionSummary? value)
		{
				if (EditableEmployee == null)
						return;

				EditableEmployee.Position = value?.Name;
				EditableEmployee.PositionId = value?.Id;
				ShowPositionValidation = value == null;

				Section = value == null
						? null
						: SectionOptions.FirstOrDefault(i => i.Id == value.SectionId);
				Department = Section == null
						? null
						: DepartmentOptions.FirstOrDefault(i => i.Id == Section.DepartmentId);
				Division = Department == null
						? null
						: DivisionOptions.FirstOrDefault(i => i.Id == Department.DivisionId);
		}
}

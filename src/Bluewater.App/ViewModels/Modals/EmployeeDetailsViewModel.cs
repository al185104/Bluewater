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
		public partial EditableEmployee? EditableEmployee { get; set; }

		public ObservableCollection<ChargingSummary> ChargingOptions { get; set; } = new();
		[ObservableProperty]
		public partial ChargingSummary Charging { get; set; } = new();

		public ObservableCollection<DivisionSummary> DivisionOptions { get; set; } = new();
		[ObservableProperty]
		public partial DivisionSummary Division { get; set; } = new();

		public ObservableCollection<DepartmentSummary> DepartmentOptions { get; set; } = new();
		[ObservableProperty]
		public partial DepartmentSummary Department { get; set; } = new();

		public ObservableCollection<SectionSummary> SectionOptions { get; set; } = new();
		[ObservableProperty]
		public partial SectionSummary Section { get; set; } = new();

		public ObservableCollection<PositionSummary> PositionOptions { get; set; } = new();
		[ObservableProperty]
		public partial PositionSummary Position { get; set; } = new();

		public ObservableCollection<EmployeeTypeSummary> TypeOptions { get; set; } = new();
		[ObservableProperty]
		public partial EmployeeTypeSummary EmployeeType { get; set; } = new();
		
		public ObservableCollection<LevelSummary> LevelOptions { get; set; } = new();
		[ObservableProperty]
		public partial LevelSummary Level { get; set; } = new();

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

				Level = LevelOptions.First();

				if (EditableEmployee!.TypeId == null || EditableEmployee.TypeId == Guid.Empty)
				{
						EmployeeType = TypeOptions.First();
						EditableEmployee.Type = EmployeeType.Name;
						EditableEmployee.TypeId = EmployeeType.Id;
				}
				else
				{
						var type = TypeOptions.FirstOrDefault(i => i.Id == EditableEmployee.TypeId);
						if (type != null)
						{
								EmployeeType = type;
								EditableEmployee.Type = EmployeeType.Name;
								EditableEmployee.TypeId = EmployeeType.Id;
						}
				}

				if(EditableEmployee!.LevelId == null || EditableEmployee.LevelId == Guid.Empty)
				{
						Level = LevelOptions.First();
						EditableEmployee.Level = Level.Name;
						EditableEmployee.LevelId = Level.Id;
				}
				else
				{
						var level = LevelOptions.FirstOrDefault(i => i.Id == EditableEmployee.LevelId);
						if (level != null) 
						{
								Level = level;
								EditableEmployee.Level = Level.Name;
								EditableEmployee.LevelId = Level.Id;
						}
				}

				if (EditableEmployee!.ChargingId == null || EditableEmployee.ChargingId == Guid.Empty)
				{
						Charging = ChargingOptions.First();
						EditableEmployee.ChargingId = Charging.Id;
				}
				else
				{
						var charging = _referenceDataService.Chargings.FirstOrDefault(i => i.Id == EditableEmployee.ChargingId);
						if (charging != null)
						{
								Charging = charging;
								EditableEmployee.ChargingId = charging.Id;
						}
				}

				if(EditableEmployee!.PositionId == null || EditableEmployee.PositionId == Guid.Empty)
				{
						Position = PositionOptions.First();
						EditableEmployee.Position = Position.Name;
						EditableEmployee.PositionId = Position.Id;
				}
				else
				{
						var position = _referenceDataService.Positions.FirstOrDefault(i => i.Id == EditableEmployee.PositionId);
						if (position != null)
						{
								Position = position;
								EditableEmployee.Position = position.Name;
								EditableEmployee.PositionId = position.Id;
						}
				}

				if(Position != null)
				{
						var section = _referenceDataService.Sections.FirstOrDefault(i => i.Id == Position.SectionId);
						if(section != null)
								Section = section;
				}

				if(Section != null)
				{
						var department = _referenceDataService.Departments.FirstOrDefault(i => i.Id == Section.DepartmentId);
						if(department != null)
								Department = department;
				}

				if(Department != null) 
				{ 
						var division = _referenceDataService.Divisions.FirstOrDefault(i => i.Id == Department.DivisionId);
						if(division != null)
								Division = division;
				}

				return base.InitializeAsync();
		}
}

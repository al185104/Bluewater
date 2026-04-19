using System.Collections.ObjectModel;
using Bluewater.App.Enums;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels.Modals;

public partial class EmployeeDetailsViewModel : BaseViewModel, IQueryAttributable
{
		private readonly IReferenceDataService _referenceDataService;
		private readonly IEmployeeApiService _employeeApiService;
		private readonly IUserApiService _userApiService;
		private readonly IPayApiService _payApiService;
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

		[ObservableProperty]
		public partial bool IsCreateMode { get; set; } = true;

		public string SaveButtonText => IsCreateMode ? "Save & Add" : "Save & Update";

		public EmployeeDetailsViewModel(
				IActivityTraceService activityTraceService, 
				IExceptionHandlingService exceptionHandlingService, 
				IReferenceDataService referenceDataService,
				IEmployeeApiService employeeApiService,
				IUserApiService userApiService,
				IPayApiService payApiService) 
				: base(activityTraceService, exceptionHandlingService)
		{
				_referenceDataService = referenceDataService;
				_employeeApiService = employeeApiService;
				_userApiService = userApiService;
				_payApiService = payApiService;
		}

		public void ApplyQueryAttributes(IDictionary<string, object> query)
		{
				if (query.TryGetValue("Employee", out var value) && value is EmployeeSummary passedEmployee)
				{
						IsCreateMode = false;
						_rowIndex = passedEmployee.RowIndex;
						EditableEmployee = EditableEmployee.FromSummary(passedEmployee);
						_ = TraceCommandAsync(nameof(ApplyQueryAttributes), new { EmployeeId = passedEmployee.Id, passedEmployee.RowIndex });
						InitializeCommand.Execute(this);
						return;
				}

				if (query.TryGetValue("IsNewEmployee", out var isNewEmployeeValue) && isNewEmployeeValue is bool isNewEmployee)
				{
						IsCreateMode = isNewEmployee;
				}

				if (IsCreateMode)
				{
						EditableEmployee = new EditableEmployee();
						_ = TraceCommandAsync(nameof(ApplyQueryAttributes), new { IsCreateMode });
						InitializeCommand.Execute(this);
				}
		}

  private void SetupOptions()
  {
    ChargingOptions.Clear();
    DivisionOptions.Clear();
    DepartmentOptions.Clear();
    SectionOptions.Clear();
    PositionOptions.Clear();
    TypeOptions.Clear();
    LevelOptions.Clear();

    foreach (var charge in _referenceDataService.Chargings)
      ChargingOptions.Add(charge);

    foreach (var division in _referenceDataService.Divisions)
      DivisionOptions.Add(division);

    foreach (var department in _referenceDataService.Departments)
      DepartmentOptions.Add(department);

    foreach (var section in _referenceDataService.Sections)
      SectionOptions.Add(section);

    foreach (var position in _referenceDataService.Positions)
      PositionOptions.Add(position);

    foreach (var type in _referenceDataService.EmployeeTypes)
      TypeOptions.Add(type);

    foreach (var level in _referenceDataService.Levels)
      LevelOptions.Add(level);
  }

		[RelayCommand]
		public async Task SaveEmployeeAsync()
		{
				try
				{
						IsBusy = true;
						await TraceCommandAsync(nameof(SaveEmployeeAsync), new { EditableEmployee?.Id, IsCreateMode }).ConfigureAwait(false);
						if (!ValidateRequiredPickers())
								return;

						if(EditableEmployee == null)
						{
								return;
						}

						if (IsCreateMode)
						{
								await CreateEmployeeAsync();
								return;
						}

						var summary = EditableEmployee.ToSummary(_rowIndex);
						UpdateEmployeeRequestDto request = EditableEmployee.ToUpdateRequest(summary);
						var updated = await _employeeApiService.UpdateEmployeeAsync(request, summary);
						if (updated != null) {
                                await Shell.Current.DisplayAlert(
                                        "Employee Updated",
                                        "Employee has been successfully updated.",
                                        "OK");
								await TraceCommandAsync(nameof(SaveEmployeeAsync), new { Action = "Updated", EmployeeId = updated.Id }).ConfigureAwait(false);
								await NavigateAsync("..",
										new Dictionary<string, object>
										{
												["TargetSection"] = MainSectionEnum.Employees
										});
						}
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, IsCreateMode ? "Creating employee" : "Updating employee");
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
						await TraceCommandAsync(nameof(CancelAsync), new { EditableEmployee?.Id }).ConfigureAwait(false);
						await NavigateAsync("..");
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

				SetupOptions();

				if (IsCreateMode)
				{
						InitializeCreateDefaults();
				}
				else
				{
						BindReferenceData();
				}

				return base.InitializeAsync();
		}

		private async Task InitializeWithReferenceDataAsync()
		{
				try
				{
						IsBusy = true;
						await TraceCommandAsync(nameof(InitializeWithReferenceDataAsync)).ConfigureAwait(false);
						await _referenceDataService.InitializeAsync();
						SetupOptions();

						if (IsCreateMode)
						{
								InitializeCreateDefaults();
						}
						else
						{
								BindReferenceData();
						}
						await base.InitializeAsync();
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Initializing employee details");
				}
				finally
				{
						IsBusy = false;
				}
		}

		private void BindReferenceData()
		{
				EmployeeType = TypeOptions.FirstOrDefault(i => i.Id == EditableEmployee!.TypeId)
						?? TypeOptions.FirstOrDefault(i => i.Name == EditableEmployee.Type);
				EditableEmployee.Type = EmployeeType?.Name;
				EditableEmployee.TypeId ??= EmployeeType?.Id;

				Level = LevelOptions.FirstOrDefault(i => i.Id == EditableEmployee.LevelId)
						?? LevelOptions.FirstOrDefault(i => i.Name == EditableEmployee.Level);
				EditableEmployee.Level = Level?.Name;
				EditableEmployee.LevelId ??= Level?.Id;

				Charging = ChargingOptions.FirstOrDefault(i => i.Id == EditableEmployee.ChargingId)
						?? ChargingOptions.FirstOrDefault(i => i.Name == EditableEmployee.Charging);
				EditableEmployee.Charging = Charging?.Name;
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

		private async Task CreateEmployeeAsync()
		{
				var summary = EditableEmployee.ToSummary(_rowIndex);
				var request = EditableEmployee.ToCreateRequest(summary);

				if (!string.IsNullOrWhiteSpace(EditableEmployee.Username) &&
						!string.IsNullOrWhiteSpace(EditableEmployee.PasswordHash))
				{
						var user = await _userApiService.CreateUserAsync(new UserRecordDto
						{
								Username = EditableEmployee.Username,
								PasswordHash = EditableEmployee.PasswordHash,
								Credential = EditableEmployee.Credential,
								IsGlobalSupervisor = EditableEmployee.IsGlobalSupervisor,
								SupervisedGroup = Guid.TryParse(EditableEmployee.SupervisedGroup, out Guid parsedSupervisedGroup)
										? parsedSupervisedGroup
										: null
						});

						request.UserId = user?.Id;
				}

				if (EditableEmployee.BasicPay.HasValue ||
						EditableEmployee.DailyRate.HasValue ||
						EditableEmployee.HourlyRate.HasValue ||
						EditableEmployee.HdmfCon.HasValue ||
						EditableEmployee.HdmfEr.HasValue)
				{
						var pay = await _payApiService.CreatePayAsync(new PayRecordDto
						{
								BasicPay = EditableEmployee.BasicPay ?? 0,
								DailyRate = EditableEmployee.DailyRate ?? 0,
								HourlyRate = EditableEmployee.HourlyRate ?? 0,
								HdmfEmployeeContribution = EditableEmployee.HdmfCon ?? 0,
								HdmfEmployerContribution = EditableEmployee.HdmfEr ?? 0
						});

						request.PayId = pay?.Id;
				}

				var created = await _employeeApiService.CreateEmployeeAsync(request);
				if (!created)
				{
                        await Shell.Current.DisplayAlert(
                                "Employee Create Failed",
                                "Employee create failed. Please check required fields and try again.",
                                "OK");
						return;
				}

                await Shell.Current.DisplayAlert(
                        "Employee Created",
                        "Employee has been successfully created.",
                        "OK");
				await TraceCommandAsync(nameof(SaveEmployeeAsync), new { Action = "Created" }).ConfigureAwait(false);
				await NavigateAsync("..",
						new Dictionary<string, object>
						{
								["TargetSection"] = MainSectionEnum.Employees
						});
		}

		private void InitializeCreateDefaults()
		{
				EmployeeType = TypeOptions.FirstOrDefault();
				EditableEmployee.Type = EmployeeType?.Name;
				EditableEmployee.TypeId = EmployeeType?.Id;

				Level = LevelOptions.FirstOrDefault();
				EditableEmployee.Level = Level?.Name;
				EditableEmployee.LevelId = Level?.Id;

				Charging = ChargingOptions.FirstOrDefault();
				EditableEmployee.Charging = Charging?.Name;
				EditableEmployee.ChargingId = Charging?.Id;

				Position = PositionOptions.FirstOrDefault();
				EditableEmployee.Position = Position?.Name;
				EditableEmployee.PositionId = Position?.Id;
		}

		partial void OnIsCreateModeChanged(bool value)
		{
				OnPropertyChanged(nameof(SaveButtonText));
		}


		partial void OnChargingChanged(ChargingSummary? value)
		{
				if (EditableEmployee == null)
						return;

				EditableEmployee.Charging = value?.Name;
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

using System.Collections.ObjectModel;
using Bluewater.App.Helpers;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using Bluewater.Core.EmployeeAggregate.Enum;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

public partial class SettingViewModel : BaseViewModel
{
		private readonly IDivisionApiService _divisionApiService;
		private readonly IDepartmentApiService _departmentApiService;
		private readonly ISectionApiService _sectionApiService;
		private readonly IChargingApiService _chargingApiService;
		private readonly IPositionApiService _positionApiService;
		private readonly IEmployeeTypeApiService _employeeTypeApiService;
		private readonly ILevelApiService _levelApiService;
		private readonly IEmployeeApiService _employeeApiService;
		private readonly ITimesheetApiService _timesheetApiService;
		private readonly IScheduleApiService _scheduleApiService;
		private readonly IShiftApiService _shiftApiService;
		private readonly IReferenceDataService _referenceService;

		[ObservableProperty]
		public partial EditableSettingItem? EditableSetting { get; set; }

		[ObservableProperty]
		public partial bool IsEditorOpen { get; set; }

		[ObservableProperty]
		public partial string EditorTitle { get; set; }

		[ObservableProperty]
		public partial DateTime StartDate { get; set; } = DateTime.Today.AddDays(-7);

		[ObservableProperty]
		public partial DateTime EndDate { get; set; } = DateTime.Today;

		[ObservableProperty]
		public partial string NewDivisionName { get; set; } = string.Empty;

		[ObservableProperty]
		public partial string NewDivisionDescription { get; set; } = string.Empty;

		[ObservableProperty]
		public partial string NewDepartmentName { get; set; } = string.Empty;

		[ObservableProperty]
		public partial string NewDepartmentDescription { get; set; } = string.Empty;

		[ObservableProperty]
		public partial DivisionSummary? SelectedDivisionForDepartment { get; set; }

		[ObservableProperty]
		public partial string NewSectionName { get; set; } = string.Empty;

		[ObservableProperty]
		public partial string NewSectionDescription { get; set; } = string.Empty;

		[ObservableProperty]
		public partial DepartmentSummary? SelectedDepartmentForSection { get; set; }

		[ObservableProperty]
		public partial string NewPositionName { get; set; } = string.Empty;

		[ObservableProperty]
		public partial string NewPositionDescription { get; set; } = string.Empty;

		[ObservableProperty]
		public partial SectionSummary? SelectedSectionForPosition { get; set; }

		[ObservableProperty]
		public partial string NewChargingName { get; set; } = string.Empty;

		[ObservableProperty]
		public partial string NewChargingDescription { get; set; } = string.Empty;

		[ObservableProperty]
		public partial DepartmentSummary? SelectedDepartmentForCharging { get; set; }

		[ObservableProperty]
		public partial string NewEmployeeTypeName { get; set; } = string.Empty;

		[ObservableProperty]
		public partial string NewEmployeeTypeValue { get; set; } = string.Empty;

		[ObservableProperty]
		public partial string NewEmployeeLevelName { get; set; } = string.Empty;

		[ObservableProperty]
		public partial string NewEmployeeLevelValue { get; set; } = string.Empty;

		[ObservableProperty]
		public partial Tenant SelectedTenant { get; set; } = Tenant.Maribago;

		public ObservableCollection<DivisionSummary> Divisions { get; } = new();
		public ObservableCollection<DepartmentSummary> Departments { get; } = new();
		public ObservableCollection<SectionSummary> Sections { get; } = new();
		public ObservableCollection<ChargingSummary> Chargings { get; } = new();
		public ObservableCollection<PositionSummary> Positions { get; } = new();
		public ObservableCollection<EmployeeTypeSummary> EmployeeTypes { get; } = new();
		public ObservableCollection<LevelSummary> EmployeeLevels { get; } = new();
		public IReadOnlyList<Tenant> TenantOptions { get; } = Enum.GetValues<Tenant>();

		public SettingViewModel(IActivityTraceService activityTraceService, IExceptionHandlingService exceptionHandlingService,
			IDivisionApiService divisionApiService,
			IDepartmentApiService departmentApiService,
			ISectionApiService sectionApiService,
			IChargingApiService chargingApiService,
			IPositionApiService positionApiService,
			IEmployeeTypeApiService employeeTypeApiService,
			ILevelApiService levelApiService,
			IEmployeeApiService employeeApiService,
			ITimesheetApiService timesheetApiService,
			IScheduleApiService scheduleApiService,
			IShiftApiService shiftApiService,
			IReferenceDataService referenceService)
			: base(activityTraceService, exceptionHandlingService)
		{
				_divisionApiService = divisionApiService;
				_departmentApiService = departmentApiService;
				_sectionApiService = sectionApiService;
				_chargingApiService = chargingApiService;
				_positionApiService = positionApiService;
				_employeeTypeApiService = employeeTypeApiService;
				_levelApiService = levelApiService;
				_employeeApiService = employeeApiService;
				_timesheetApiService = timesheetApiService;
				_scheduleApiService = scheduleApiService;
				_shiftApiService = shiftApiService;
				_referenceService = referenceService;

				var tenant = Preferences.Get(nameof(SelectedTenant), Tenant.Maribago.ToString());
				SelectedTenant = Enum.TryParse<Tenant>(tenant, out Tenant parsed) ? parsed : Tenant.Maribago;

				EditorTitle = "Title";

				Divisions = new ObservableCollection<DivisionSummary>(_referenceService.Divisions);
				Departments = new ObservableCollection<DepartmentSummary>(_referenceService.Departments);
				Sections = new ObservableCollection<SectionSummary>(_referenceService.Sections);
				Positions = new ObservableCollection<PositionSummary>(_referenceService.Positions);
				Chargings = new ObservableCollection<ChargingSummary>(_referenceService.Chargings);
				EmployeeTypes = new ObservableCollection<EmployeeTypeSummary>(_referenceService.EmployeeTypes);
				EmployeeLevels = new ObservableCollection<LevelSummary>(_referenceService.Levels);
				SelectedDivisionForDepartment = Divisions.FirstOrDefault();
				SelectedDepartmentForSection = Departments.FirstOrDefault();
				SelectedSectionForPosition = Sections.FirstOrDefault();
				SelectedDepartmentForCharging = Departments.FirstOrDefault();

		}

		partial void OnSelectedTenantChanged(Tenant value)
		{
				Preferences.Set(nameof(SelectedTenant), value.ToString());
		}

		[RelayCommand]
		private async Task EditDivisionAsync(DivisionSummary? division)
		{
				if (division is null)
				{
						return;
				}

				EditableSetting = EditableSettingItem.FromDivision(division);
				EditorTitle = $"Edit Division: {EditableSetting.Name}";
				IsEditorOpen = true;

				await TraceCommandAsync(nameof(EditDivisionAsync), division.Id);
		}

		[RelayCommand]
		private async Task DeleteDivisionAsync(DivisionSummary? division)
		{
				if (division is null ||
						!await Shell.Current.DisplayAlert("Delete", "Are you sure you want to delete this division?", "Yes", "No"))
				{
						return;
				}

				try
				{
						bool deleted = await _divisionApiService.DeleteDivisionAsync(division.Id).ConfigureAwait(false);
						if (!deleted)
						{
								return;
						}

						await MainThread.InvokeOnMainThreadAsync(() => Divisions.Remove(division));
						await TraceCommandAsync(nameof(DeleteDivisionAsync), division.Id).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Deleting division");
				}
		}

		[RelayCommand]
		private async Task EditDepartmentAsync(DepartmentSummary? department)
		{
				if (department is null)
				{
						return;
				}

				EditableSetting = EditableSettingItem.FromDepartment(department);
				EditorTitle = $"Edit Department: {EditableSetting.Name}";
				IsEditorOpen = true;

				await TraceCommandAsync(nameof(EditDepartmentAsync), department.Id);
		}

		[RelayCommand]
		private async Task DeleteDepartmentAsync(DepartmentSummary? department)
		{
				if (department is null || !await Shell.Current.DisplayAlert("Delete", "Are you sure you want to delete this department?", "Yes", "No"))
				{
						return;
				}

				try
				{
						bool deleted = await _departmentApiService.DeleteDepartmentAsync(department.Id).ConfigureAwait(false);
						if (!deleted)
						{
								return;
						}

						await MainThread.InvokeOnMainThreadAsync(() => Departments.Remove(department));
						await TraceCommandAsync(nameof(DeleteDepartmentAsync), department.Id).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Deleting department");
				}
		}

		[RelayCommand]
		private async Task EditSectionAsync(SectionSummary? section)
		{
				if (section is null)
				{
						return;
				}

				EditableSetting = EditableSettingItem.FromSection(section);
				EditorTitle = $"Edit Section: {EditableSetting.Name}";
				IsEditorOpen = true;

				await TraceCommandAsync(nameof(EditSectionAsync), section.Id);
		}

		[RelayCommand]
		private async Task DeleteSectionAsync(SectionSummary? section)
		{
				if (section is null || !await Shell.Current.DisplayAlert("Delete", "Are you sure you want to delete this section?", "Yes", "No"))
				{
						return;
				}

				try
				{
						bool deleted = await _sectionApiService.DeleteSectionAsync(section.Id).ConfigureAwait(false);
						if (!deleted)
						{
								return;
						}

						await MainThread.InvokeOnMainThreadAsync(() => Sections.Remove(section));
						await TraceCommandAsync(nameof(DeleteSectionAsync), section.Id).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Deleting section");
				}
		}

		[RelayCommand]
		private async Task EditChargingAsync(ChargingSummary? charging)
		{
				if (charging is null)
				{
						return;
				}

				EditableSetting = EditableSettingItem.FromCharging(charging);
				EditorTitle = $"Edit Charging: {EditableSetting.Name}";
				IsEditorOpen = true;

				await TraceCommandAsync(nameof(EditChargingAsync), charging.Id);
		}

		[RelayCommand]
		private async Task DeleteChargingAsync(ChargingSummary? charging)
		{
				if (charging is null || !await Shell.Current.DisplayAlert("Delete", "Are you sure you want to delete this charging?", "Yes", "No"))
				{
						return;
				}

				try
				{
						bool deleted = await _chargingApiService.DeleteChargingAsync(charging.Id).ConfigureAwait(false);
						if (!deleted)
						{
								return;
						}

						await MainThread.InvokeOnMainThreadAsync(() => Chargings.Remove(charging));
						await TraceCommandAsync(nameof(DeleteChargingAsync), charging.Id).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Deleting charging");
				}
		}

		[RelayCommand]
		private async Task EditPositionAsync(PositionSummary? position)
		{
				if (position is null)
				{
						return;
				}

				EditableSetting = EditableSettingItem.FromPosition(position);
				EditorTitle = $"Edit Position: {EditableSetting.Name}";
				IsEditorOpen = true;

				await TraceCommandAsync(nameof(EditPositionAsync), position.Id);
		}

		[RelayCommand]
		private async Task UpdateSettingAsync()
		{
				if (EditableSetting is null)
				{
						return;
				}

				switch (EditableSetting.Type)
				{
						case SettingItemType.Division:
								await UpdateDivisionAsync().ConfigureAwait(false);
								break;
						case SettingItemType.Department:
								await UpdateDepartmentAsync().ConfigureAwait(false);
								break;
						case SettingItemType.Section:
								await UpdateSectionAsync().ConfigureAwait(false);
								break;
						case SettingItemType.Charging:
								await UpdateChargingAsync().ConfigureAwait(false);
								break;
						case SettingItemType.Position:
								await UpdatePositionAsync().ConfigureAwait(false);
								break;
						case SettingItemType.EmployeeType:
								await UpdateEmployeeTypeAsync().ConfigureAwait(false);
								break;
						case SettingItemType.EmployeeLevel:
								await UpdateEmployeeLevelAsync().ConfigureAwait(false);
								break;
				}

				IsEditorOpen = false;
				EditorTitle = string.Empty;
				EditableSetting = null;
		}

		private async Task UpdateDivisionAsync()
		{
				if (EditableSetting is null)
				{
						return;
				}

				DivisionSummary? existing = Divisions.FirstOrDefault(item => item.Id == EditableSetting.Id);

				if (existing is null)
				{
						return;
				}

				DivisionSummary? updated = await _divisionApiService.UpdateDivisionAsync(EditableSetting.ToDivision(existing.RowIndex)).ConfigureAwait(false);
				if (updated is null)
				{
						return;
				}

				await MainThread.InvokeOnMainThreadAsync(() =>
				{
						int index = Divisions.IndexOf(existing);
						updated.RowIndex = existing.RowIndex;
						Divisions[index] = updated;
				});
		}

		private async Task UpdateDepartmentAsync()
		{
				if (EditableSetting is null)
				{
						return;
				}

				DepartmentSummary? existing = Departments.FirstOrDefault(item => item.Id == EditableSetting.Id);

				if (existing is null)
				{
						return;
				}

				DepartmentSummary? updated = await _departmentApiService.UpdateDepartmentAsync(EditableSetting.ToDepartment(existing.RowIndex)).ConfigureAwait(false);
				if (updated is null)
				{
						return;
				}

				await MainThread.InvokeOnMainThreadAsync(() =>
				{
						int index = Departments.IndexOf(existing);
						updated.RowIndex = existing.RowIndex;
						updated.DivisionName = Divisions.FirstOrDefault(i => i.Id == updated.DivisionId)?.Name;
						Departments[index] = updated;
				});
		}

		private async Task UpdateSectionAsync()
		{
				if (EditableSetting is null)
				{
						return;
				}

				SectionSummary? existing = Sections.FirstOrDefault(item => item.Id == EditableSetting.Id);

				if (existing is null)
				{
						return;
				}

				SectionSummary? updated = await _sectionApiService.UpdateSectionAsync(EditableSetting.ToSection(existing.RowIndex)).ConfigureAwait(false);
				if (updated is null)
				{
						return;
				}

				await MainThread.InvokeOnMainThreadAsync(() =>
				{
						int index = Sections.IndexOf(existing);
						updated.RowIndex = existing.RowIndex;
						updated.DepartmentName = Departments.FirstOrDefault(i => i.Id == updated.DepartmentId)?.Name;
						Sections[index] = updated;
				});
		}

		private async Task UpdateChargingAsync()
		{
				if (EditableSetting is null)
				{
						return;
				}

				ChargingSummary? existing = Chargings.FirstOrDefault(item => item.Id == EditableSetting.Id);

				if (existing is null)
				{
						return;
				}

				ChargingSummary? updated = await _chargingApiService.UpdateChargingAsync(EditableSetting.ToCharging(existing.RowIndex)).ConfigureAwait(false);
				if (updated is null)
				{
						return;
				}

				await MainThread.InvokeOnMainThreadAsync(() =>
				{
						int index = Chargings.IndexOf(existing);
						updated.RowIndex = existing.RowIndex;
						updated.DepartmentName = updated.DepartmentId.HasValue
								? Departments.FirstOrDefault(i => i.Id == updated.DepartmentId.Value)?.Name
								: null;
						Chargings[index] = updated;
				});
		}

		private async Task UpdatePositionAsync()
		{
				if (EditableSetting is null)
				{
						return;
				}

				PositionSummary? existing = Positions.FirstOrDefault(item => item.Id == EditableSetting.Id);

				if (existing is null)
				{
						return;
				}

				PositionSummary? updated = await _positionApiService.UpdatePositionAsync(EditableSetting.ToPosition(existing.RowIndex)).ConfigureAwait(false);
				if (updated is null)
				{
						return;
				}

				await MainThread.InvokeOnMainThreadAsync(() =>
				{
						int index = Positions.IndexOf(existing);
						updated.RowIndex = existing.RowIndex;
						Positions[index] = updated;
				});
		}

		private async Task UpdateEmployeeTypeAsync()
		{
				if (EditableSetting is null)
				{
						return;
				}

				EmployeeTypeSummary? existing = EmployeeTypes.FirstOrDefault(item => item.Id == EditableSetting.Id);
				if (existing is null)
				{
						return;
				}

				EmployeeTypeSummary? updated = await _employeeTypeApiService.UpdateEmployeeTypeAsync(EditableSetting.ToEmployeeType(existing.IsActive)).ConfigureAwait(false);
				if (updated is null)
				{
						return;
				}

				await MainThread.InvokeOnMainThreadAsync(() =>
				{
						int index = EmployeeTypes.IndexOf(existing);
						EmployeeTypes[index] = updated;
				});
		}

		private async Task UpdateEmployeeLevelAsync()
		{
				if (EditableSetting is null)
				{
						return;
				}

				LevelSummary? existing = EmployeeLevels.FirstOrDefault(item => item.Id == EditableSetting.Id);
				if (existing is null)
				{
						return;
				}

				LevelSummary? updated = await _levelApiService.UpdateLevelAsync(EditableSetting.ToEmployeeLevel(existing.IsActive)).ConfigureAwait(false);
				if (updated is null)
				{
						return;
				}

				await MainThread.InvokeOnMainThreadAsync(() =>
				{
						int index = EmployeeLevels.IndexOf(existing);
						EmployeeLevels[index] = updated;
				});
		}

		[RelayCommand]
		private async Task DeletePositionAsync(PositionSummary? position)
		{
				if (position is null ||
						!await Shell.Current.DisplayAlert("Delete", "Are you sure you want to delete this position?", "Yes", "No"))
				{
						return;
				}

				try
				{
						bool deleted = await _positionApiService.DeletePositionAsync(position.Id).ConfigureAwait(false);
						if (!deleted)
						{
								return;
						}

						await MainThread.InvokeOnMainThreadAsync(() => Positions.Remove(position));
						await TraceCommandAsync(nameof(DeletePositionAsync), position.Id).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Deleting position");
				}
		}

		[RelayCommand]
		private async Task EditEmployeeTypeAsync(EmployeeTypeSummary? employeeType)
		{
				if (employeeType is null)
				{
						return;
				}

				EditableSetting = EditableSettingItem.FromEmployeeType(employeeType);
				EditorTitle = $"Edit Employee Type: {EditableSetting.Name}";
				IsEditorOpen = true;

				await TraceCommandAsync(nameof(EditEmployeeTypeAsync), employeeType.Id);
		}

		[RelayCommand]
		private async Task DeleteEmployeeTypeAsync(EmployeeTypeSummary? employeeType)
		{
				if (employeeType is null || !await Shell.Current.DisplayAlert("Delete", "Are you sure you want to delete this employee type?", "Yes", "No"))
				{
						return;
				}

				bool deleted = await _employeeTypeApiService.DeleteEmployeeTypeAsync(employeeType.Id).ConfigureAwait(false);
				if (!deleted)
				{
						return;
				}

				await MainThread.InvokeOnMainThreadAsync(() => EmployeeTypes.Remove(employeeType));
				await TraceCommandAsync(nameof(DeleteEmployeeTypeAsync), employeeType.Id).ConfigureAwait(false);
		}

		[RelayCommand]
		private async Task EditEmployeeLevelAsync(LevelSummary? level)
		{
				if (level is null)
				{
						return;
				}

				EditableSetting = EditableSettingItem.FromEmployeeLevel(level);
				EditorTitle = $"Edit Employee Level: {EditableSetting.Name}";
				IsEditorOpen = true;

				await TraceCommandAsync(nameof(EditEmployeeLevelAsync), level.Id);
		}

		[RelayCommand]
		private async Task DeleteEmployeeLevelAsync(LevelSummary? level)
		{
				if (level is null || !await Shell.Current.DisplayAlert("Delete", "Are you sure you want to delete this employee level?", "Yes", "No"))
				{
						return;
				}

				bool deleted = await _levelApiService.DeleteLevelAsync(level.Id).ConfigureAwait(false);
				if (!deleted)
				{
						return;
				}

				await MainThread.InvokeOnMainThreadAsync(() => EmployeeLevels.Remove(level));
				await TraceCommandAsync(nameof(DeleteEmployeeLevelAsync), level.Id).ConfigureAwait(false);
		}

		[RelayCommand]
		private async Task AddDivisionAsync()
		{
				if (string.IsNullOrWhiteSpace(NewDivisionName)) return;
				DivisionSummary? created = await _divisionApiService.CreateDivisionAsync(new DivisionSummary
				{
						Name = NewDivisionName.Trim(),
						Description = string.IsNullOrWhiteSpace(NewDivisionDescription) ? null : NewDivisionDescription.Trim()
				}).ConfigureAwait(false);
				if (created is null) return;
				await MainThread.InvokeOnMainThreadAsync(() =>
				{
						created.RowIndex = Divisions.Count;
						Divisions.Add(created);
						NewDivisionName = string.Empty;
						NewDivisionDescription = string.Empty;
				});
		}

		[RelayCommand]
		private async Task AddDepartmentAsync()
		{
				if (string.IsNullOrWhiteSpace(NewDepartmentName) || SelectedDivisionForDepartment is null) return;
				DepartmentSummary? created = await _departmentApiService.CreateDepartmentAsync(new DepartmentSummary
				{
						Name = NewDepartmentName.Trim(),
						Description = string.IsNullOrWhiteSpace(NewDepartmentDescription) ? null : NewDepartmentDescription.Trim(),
						DivisionId = SelectedDivisionForDepartment.Id,
						DivisionName = SelectedDivisionForDepartment.Name
				}).ConfigureAwait(false);
				if (created is null) return;
				await MainThread.InvokeOnMainThreadAsync(() =>
				{
						created.RowIndex = Departments.Count;
						created.DivisionName = SelectedDivisionForDepartment?.Name;
						Departments.Add(created);
						NewDepartmentName = string.Empty;
						NewDepartmentDescription = string.Empty;
				});
		}

		[RelayCommand]
		private async Task AddSectionAsync()
		{
				if (string.IsNullOrWhiteSpace(NewSectionName) || SelectedDepartmentForSection is null) return;
				SectionSummary? created = await _sectionApiService.CreateSectionAsync(new SectionSummary
				{
						Name = NewSectionName.Trim(),
						Description = string.IsNullOrWhiteSpace(NewSectionDescription) ? null : NewSectionDescription.Trim(),
						DepartmentId = SelectedDepartmentForSection.Id,
						DepartmentName = SelectedDepartmentForSection.Name
				}).ConfigureAwait(false);
				if (created is null) return;
				await MainThread.InvokeOnMainThreadAsync(() =>
				{
						created.RowIndex = Sections.Count;
						created.DepartmentName = SelectedDepartmentForSection?.Name;
						Sections.Add(created);
						NewSectionName = string.Empty;
						NewSectionDescription = string.Empty;
				});
		}

		[RelayCommand]
		private async Task AddPositionAsync()
		{
				if (string.IsNullOrWhiteSpace(NewPositionName) || SelectedSectionForPosition is null) return;
				PositionSummary? created = await _positionApiService.CreatePositionAsync(new PositionSummary
				{
						Name = NewPositionName.Trim(),
						Description = string.IsNullOrWhiteSpace(NewPositionDescription) ? null : NewPositionDescription.Trim(),
						SectionId = SelectedSectionForPosition.Id,
						SectionName = SelectedSectionForPosition.Name,
						SectionDescription = SelectedSectionForPosition.Description
				}).ConfigureAwait(false);
				if (created is null) return;
				await MainThread.InvokeOnMainThreadAsync(() =>
				{
						created.RowIndex = Positions.Count;
						Positions.Add(created);
						NewPositionName = string.Empty;
						NewPositionDescription = string.Empty;
				});
		}

		[RelayCommand]
		private async Task AddChargingAsync()
		{
				if (string.IsNullOrWhiteSpace(NewChargingName) || SelectedDepartmentForCharging is null) return;
				ChargingSummary? created = await _chargingApiService.CreateChargingAsync(new ChargingSummary
				{
						Name = NewChargingName.Trim(),
						Description = string.IsNullOrWhiteSpace(NewChargingDescription) ? null : NewChargingDescription.Trim(),
						DepartmentId = SelectedDepartmentForCharging.Id,
						DepartmentName = SelectedDepartmentForCharging.Name
				}).ConfigureAwait(false);
				if (created is null) return;
				await MainThread.InvokeOnMainThreadAsync(() =>
				{
						created.RowIndex = Chargings.Count;
						created.DepartmentName = SelectedDepartmentForCharging?.Name;
						Chargings.Add(created);
						NewChargingName = string.Empty;
						NewChargingDescription = string.Empty;
				});
		}

		[RelayCommand]
		private async Task AddEmployeeTypeAsync()
		{
				if (string.IsNullOrWhiteSpace(NewEmployeeTypeName)) return;
				EmployeeTypeSummary? created = await _employeeTypeApiService.CreateEmployeeTypeAsync(new EmployeeTypeSummary
				{
						Name = NewEmployeeTypeName.Trim(),
						Value = string.IsNullOrWhiteSpace(NewEmployeeTypeValue) ? NewEmployeeTypeName.Trim() : NewEmployeeTypeValue.Trim(),
						IsActive = true
				}).ConfigureAwait(false);
				if (created is null) return;
				await MainThread.InvokeOnMainThreadAsync(() =>
				{
						EmployeeTypes.Add(created);
						NewEmployeeTypeName = string.Empty;
						NewEmployeeTypeValue = string.Empty;
				});
		}

		[RelayCommand]
		private async Task AddEmployeeLevelAsync()
		{
				if (string.IsNullOrWhiteSpace(NewEmployeeLevelName)) return;
				LevelSummary? created = await _levelApiService.CreateLevelAsync(new LevelSummary
				{
						Name = NewEmployeeLevelName.Trim(),
						Value = string.IsNullOrWhiteSpace(NewEmployeeLevelValue) ? NewEmployeeLevelName.Trim() : NewEmployeeLevelValue.Trim(),
						IsActive = true
				}).ConfigureAwait(false);
				if (created is null) return;
				await MainThread.InvokeOnMainThreadAsync(() =>
				{
						EmployeeLevels.Add(created);
						NewEmployeeLevelName = string.Empty;
						NewEmployeeLevelValue = string.Empty;
				});
		}



		[RelayCommand]
		private Task ImportDivisionsAsync() => ImportSettingsAsync(
			"Select divisions CSV file",
			"Importing divisions",
			rows => CreateSettingsAsync(rows, CreateDivisionFromRow, _divisionApiService.CreateDivisionAsync),
			InitializeAsync);

		[RelayCommand]
		private Task ImportDepartmentsAsync() => ImportSettingsAsync(
			"Select departments CSV file",
			"Importing departments",
			rows => CreateSettingsAsync(rows, CreateDepartmentFromRow, _departmentApiService.CreateDepartmentAsync),
			InitializeAsync);

		[RelayCommand]
		private Task ImportSectionsAsync() => ImportSettingsAsync(
			"Select sections CSV file",
			"Importing sections",
			rows => CreateSettingsAsync(rows, CreateSectionFromRow, _sectionApiService.CreateSectionAsync),
			InitializeAsync);

		[RelayCommand]
		private Task ImportPositionsAsync() => ImportSettingsAsync(
			"Select positions CSV file",
			"Importing positions",
			rows => CreateSettingsAsync(rows, CreatePositionFromRow, _positionApiService.CreatePositionAsync),
			InitializeAsync);

		[RelayCommand]
		private Task ImportChargingsAsync() => ImportSettingsAsync(
			"Select chargings CSV file",
			"Importing chargings",
			rows => CreateSettingsAsync(rows, CreateChargingFromRow, _chargingApiService.CreateChargingAsync),
			InitializeAsync);

		[RelayCommand]
		private Task ImportEmployeeTypesAsync() => ImportSettingsAsync(
			"Select employee types CSV file",
			"Importing employee types",
			rows => CreateSettingsAsync(rows, CreateEmployeeTypeFromRow, _employeeTypeApiService.CreateEmployeeTypeAsync),
			InitializeAsync);

		[RelayCommand]
		private Task ImportEmployeeLevelsAsync() => ImportSettingsAsync(
			"Select employee levels CSV file",
			"Importing employee levels",
			rows => CreateSettingsAsync(rows, CreateEmployeeLevelFromRow, _levelApiService.CreateLevelAsync),
			InitializeAsync);

		[RelayCommand]
		private async Task RandomizeDataAsync()
		{
				if (IsBusy)
				{
						return;
				}

				DateTime start = StartDate;
				DateTime end = EndDate;

				if (end < start)
				{
						(start, end) = (end, start);
						StartDate = start;
						EndDate = end;
				}

				DateOnly startDate = DateOnly.FromDateTime(start);
				DateOnly endDate = DateOnly.FromDateTime(end);

				try
				{
						IsBusy = true;

						await TraceCommandAsync(nameof(RandomizeDataAsync), new { startDate, endDate }).ConfigureAwait(false);

						const int employeePageSize = 100;
						var employees = new List<EmployeeSummary>();
						int skip = 0;

						while (true)
						{
								PagedResult<EmployeeSummary> page = await _employeeApiService
									.GetEmployeesAsync(skip, employeePageSize)
									.ConfigureAwait(false);

								if (page.Items.Count == 0)
								{
										break;
								}

								employees.AddRange(page.Items);

								if (employees.Count >= page.TotalCount)
								{
										break;
								}

								skip += employeePageSize;
						}

						if (employees.Count == 0)
						{
								return;
						}

						var random = new Random();

						await GenerateRandomTimesheetsAsync(employees, startDate, endDate, random).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Generating randomized data");
				}
				finally
				{
						IsBusy = false;
				}
		}

		private async Task GenerateRandomTimesheetsAsync(
			IReadOnlyList<EmployeeSummary> employees,
			DateOnly startDate,
			DateOnly endDate,
			Random random)
		{
				foreach (EmployeeSummary employee in employees)
				{
						string username = employee.User?.Username ?? string.Empty;

						if (string.IsNullOrWhiteSpace(username))
						{
								continue;
						}

						for (DateOnly date = startDate; date <= endDate; date = date.AddDays(1))
						{
								if (ShouldSkipWorkday(date, random))
								{
										continue;
								}

								(DateTime timeIn1, DateTime timeOut1, DateTime timeIn2, DateTime timeOut2) = CreateRandomWorkdayTimes(date, random);

								await _timesheetApiService.CreateTimesheetEntryAsync(username, timeIn1, date, TimesheetInputType.TimeIn1).ConfigureAwait(false);
								await _timesheetApiService.CreateTimesheetEntryAsync(username, timeOut1, date, TimesheetInputType.TimeOut1).ConfigureAwait(false);
								await _timesheetApiService.CreateTimesheetEntryAsync(username, timeIn2, date, TimesheetInputType.TimeIn2).ConfigureAwait(false);
								await _timesheetApiService.CreateTimesheetEntryAsync(username, timeOut2, date, TimesheetInputType.TimeOut2).ConfigureAwait(false);
						}
				}
		}

		private async Task GenerateRandomScheduleAsync(
			IReadOnlyList<EmployeeSummary> employees,
			DateOnly startDate,
			DateOnly endDate,
			Random random)
		{
				List<EmployeeSummary> regularEmployees = employees
					.Where(employee => string.Equals(employee.Type, "Regular", StringComparison.OrdinalIgnoreCase))
					.ToList();

				if (regularEmployees.Count == 0)
				{
						return;
				}

				IReadOnlyList<ShiftSummary> shifts = await _shiftApiService
					.GetShiftsAsync()
					.ConfigureAwait(false);

				if (shifts.Count == 0)
				{
						return;
				}

				EmployeeSummary selectedEmployee = regularEmployees[random.Next(regularEmployees.Count)];

				for (DateOnly date = startDate; date <= endDate; date = date.AddDays(1))
				{
						if (ShouldSkipWorkday(date, random))
						{
								continue;
						}

						ShiftSummary shift = shifts[random.Next(shifts.Count)];

						var schedule = new ScheduleSummary
						{
								EmployeeId = selectedEmployee.Id,
								ShiftId = shift.Id,
								ScheduleDate = date,
								IsDefault = false
						};

						await _scheduleApiService.CreateScheduleAsync(schedule).ConfigureAwait(false);
				}
		}

		private (DateTime TimeIn1, DateTime TimeOut1, DateTime TimeIn2, DateTime TimeOut2) CreateRandomWorkdayTimes(
			DateOnly date,
			Random random)
		{
				DateTime dayStart = date.ToDateTime(TimeOnly.MinValue);

				DateTime timeIn1 = dayStart
					.AddHours(7 + random.Next(0, 3))
					.AddMinutes(random.Next(0, 60));

				DateTime timeOut1 = timeIn1
					.AddHours(4)
					.AddMinutes(random.Next(-10, 15));

				DateTime timeIn2 = timeOut1
					.AddMinutes(45 + random.Next(0, 30));

				DateTime timeOut2 = timeIn2
					.AddHours(4)
					.AddMinutes(random.Next(-10, 20));

				return (timeIn1, timeOut1, timeIn2, timeOut2);
		}

		private static bool ShouldSkipWorkday(DateOnly date, Random random)
		{
				double threshold = date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday ? 0.6 : 0.2;
				return random.NextDouble() < threshold;
		}


		private async Task ImportSettingsAsync(
			string pickerTitle,
			string errorContext,
			Func<IReadOnlyList<SettingsCsvRow>, Task<int>> importAction,
			Func<Task> onSuccess)
		{
			if (IsBusy)
			{
				return;
			}

			try
			{
				PickOptions options = new()
				{
					PickerTitle = pickerTitle,
					FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
					{
						[DevicePlatform.iOS] = ["public.comma-separated-values-text", "public.text"],
						[DevicePlatform.Android] = ["text/csv", "text/comma-separated-values"],
						[DevicePlatform.WinUI] = [".csv"],
						[DevicePlatform.MacCatalyst] = ["public.comma-separated-values-text", "public.text"]
					})
				};

				FileResult? file = await FilePicker.Default.PickAsync(options).ConfigureAwait(false);

				if (file is null)
				{
					return;
				}

				await using Stream stream = await file.OpenReadAsync().ConfigureAwait(false);
				IReadOnlyList<SettingsCsvRow> rows = await SettingsCsvImporter.ParseAsync(stream).ConfigureAwait(false);

				if (rows.Count == 0)
				{
					return;
				}

				await RunOnMainThreadAsync(() => IsBusy = true);
				int imported = await importAction(rows).ConfigureAwait(false);
				await RunOnMainThreadAsync(() => IsBusy = false);

				if (imported > 0)
				{
					await RunOnMainThreadAsync(onSuccess);
				}

				await RunOnMainThreadAsync(() =>
					Shell.Current.DisplayAlert(
						"Import result",
						$"Successfully imported {imported} out of {rows.Count} records.",
						"Okay"));
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception ex)
			{
				ExceptionHandlingService.Handle(ex, errorContext);
			}
			finally
			{
				await RunOnMainThreadAsync(() => IsBusy = false);
			}
		}


		private static Task RunOnMainThreadAsync(Action action) =>
			MainThread.IsMainThread
				? ExecuteOnCurrentThread(action)
				: MainThread.InvokeOnMainThreadAsync(action);

		private static Task RunOnMainThreadAsync(Func<Task> action) =>
			MainThread.IsMainThread
				? action()
				: MainThread.InvokeOnMainThreadAsync(action);

		private static Task ExecuteOnCurrentThread(Action action)
		{
			action();
			return Task.CompletedTask;
		}

		private async Task<int> CreateSettingsAsync<TSetting>(
			IReadOnlyList<SettingsCsvRow> rows,
			Func<SettingsCsvRow, TSetting?> map,
			Func<TSetting, CancellationToken, Task<TSetting?>> createAsync)
			where TSetting : class
		{
			int success = 0;

			foreach (SettingsCsvRow row in rows)
			{
				TSetting? setting = map(row);

				if (setting is null)
				{
					continue;
				}

				TSetting? created = await createAsync(setting, CancellationToken.None).ConfigureAwait(false);

				if (created is not null)
				{
					success++;
				}
			}

			return success;
		}

		private DivisionSummary CreateDivisionFromRow(SettingsCsvRow row) => new()
		{
			Name = row.Name,
			Description = row.Description
		};

		private DepartmentSummary? CreateDepartmentFromRow(SettingsCsvRow row)
		{
			Guid? divisionId = ResolveId(row, Divisions.Select(i => (i.Id, i.Name)));

			if (!divisionId.HasValue)
			{
				return null;
			}

			return new DepartmentSummary
			{
				Name = row.Name,
				Description = row.Description,
				DivisionId = divisionId.Value,
				DivisionName = row.Reference
			};
		}

		private SectionSummary? CreateSectionFromRow(SettingsCsvRow row)
		{
			Guid? departmentId = ResolveId(row, Departments.Select(i => (i.Id, i.Name)));

			if (!departmentId.HasValue)
			{
				return null;
			}

			return new SectionSummary
			{
				Name = row.Name,
				Description = row.Description,
				DepartmentId = departmentId.Value,
				DepartmentName = row.Reference
			};
		}

		private PositionSummary? CreatePositionFromRow(SettingsCsvRow row)
		{
			Guid? sectionId = ResolveId(row, Sections.Select(i => (i.Id, i.Name)));

			if (!sectionId.HasValue)
			{
				return null;
			}

			return new PositionSummary
			{
				Name = row.Name,
				Description = row.Description,
				SectionId = sectionId.Value,
				SectionName = Sections.FirstOrDefault(s => s.Id == sectionId.Value)?.Name,
				SectionDescription = Sections.FirstOrDefault(s => s.Id == sectionId.Value)?.Description
			};
		}

		private ChargingSummary CreateChargingFromRow(SettingsCsvRow row) => new()
		{
			Name = row.Name,
			Description = row.Description,
			DepartmentId = ResolveOptionalId(row, Departments.Select(i => (i.Id, i.Name))),
			DepartmentName = row.Reference
		};

		private EmployeeTypeSummary CreateEmployeeTypeFromRow(SettingsCsvRow row) => new()
		{
			Name = row.Name,
			Value = string.IsNullOrWhiteSpace(row.Value) ? row.Name : row.Value,
			IsActive = row.IsActive
		};

		private LevelSummary CreateEmployeeLevelFromRow(SettingsCsvRow row) => new()
		{
			Name = row.Name,
			Value = string.IsNullOrWhiteSpace(row.Value) ? row.Name : row.Value,
			IsActive = row.IsActive
		};

		private static Guid? ResolveId(SettingsCsvRow row, IEnumerable<(Guid Id, string Name)> source) =>
			ResolveOptionalId(row, source);

		private static Guid? ResolveOptionalId(SettingsCsvRow row, IEnumerable<(Guid Id, string Name)> source)
		{
			if (row.ParentId.HasValue && row.ParentId.Value != Guid.Empty)
			{
				return row.ParentId.Value;
			}

			if (string.IsNullOrWhiteSpace(row.ParentName))
			{
				return null;
			}

			(Guid Id, string Name) match = source.FirstOrDefault(item => string.Equals(item.Name, row.ParentName, StringComparison.OrdinalIgnoreCase));
			return match.Id == Guid.Empty ? null : match.Id;
		}

		public override async Task InitializeAsync()
		{
				if (IsBusy)
				{
						return;
				}

				try
				{
						await RunOnMainThreadAsync(() =>
						{
								IsBusy = true;
								Divisions.Clear();
								Departments.Clear();
								Sections.Clear();
								Chargings.Clear();
								Positions.Clear();
								EmployeeTypes.Clear();
								EmployeeLevels.Clear();
						});

						var divisionTask = _divisionApiService.GetDivisionsAsync();
						var departmentTask = _departmentApiService.GetDepartmentsAsync();
						var sectionTask = _sectionApiService.GetSectionsAsync();
						var chargingTask = _chargingApiService.GetChargingsAsync();
						var positionTask = _positionApiService.GetPositionsAsync();
						var employeeTypeTask = _employeeTypeApiService.GetEmployeeTypesAsync();
						var levelTask = _levelApiService.GetLevelsAsync();

						await Task.WhenAll(divisionTask, departmentTask, sectionTask, chargingTask, positionTask, employeeTypeTask, levelTask).ConfigureAwait(false);

						IReadOnlyList<DivisionSummary> divisions = divisionTask.Result.OrderBy(d => d.Name).ToList();
						IReadOnlyList<DepartmentSummary> departments = departmentTask.Result.OrderBy(d => d.Name).ToList();
						IReadOnlyList<SectionSummary> sections = sectionTask.Result.OrderBy(s => s.Name).ToList();
						IReadOnlyList<ChargingSummary> chargings = chargingTask.Result.OrderBy(c => c.Name).ToList();
						IReadOnlyList<PositionSummary> positions = positionTask.Result.OrderBy(p => p.Name).ToList();
						IReadOnlyList<EmployeeTypeSummary> employeeTypes = employeeTypeTask.Result.OrderBy(t => t.Name).ToList();
						IReadOnlyList<LevelSummary> levels = levelTask.Result.OrderBy(l => l.Name).ToList();

						await RunOnMainThreadAsync(() =>
						{
								int index = 0;
								foreach (DivisionSummary division in divisions)
								{
									division.RowIndex = index++;
									Divisions.Add(division);
								}

								index = 0;
								foreach (DepartmentSummary department in departments)
								{
									department.RowIndex = index++;
									department.DivisionName = Divisions.FirstOrDefault(i => i.Id == department.DivisionId)?.Name;
									Departments.Add(department);
								}

								index = 0;
								foreach (SectionSummary section in sections)
								{
									section.RowIndex = index++;
									section.DepartmentName = Departments.FirstOrDefault(i => i.Id == section.DepartmentId)?.Name;
									Sections.Add(section);
								}

								index = 0;
								foreach (ChargingSummary charging in chargings)
								{
									charging.RowIndex = index++;
									charging.DepartmentName = charging.DepartmentId.HasValue
										? Departments.FirstOrDefault(i => i.Id == charging.DepartmentId.Value)?.Name
										: null;
									Chargings.Add(charging);
								}

								index = 0;
								foreach (PositionSummary position in positions)
								{
									position.RowIndex = index++;
									position.SectionName = Sections.FirstOrDefault(i => i.Id == position.SectionId)?.Name;
									position.SectionDescription = Sections.FirstOrDefault(i => i.Id == position.SectionId)?.Description;
									Positions.Add(position);
								}

								foreach (EmployeeTypeSummary employeeType in employeeTypes)
								{
									EmployeeTypes.Add(employeeType);
								}

								foreach (LevelSummary level in levels)
								{
									EmployeeLevels.Add(level);
								}
						});
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Loading settings data");
				}
				finally
				{
						await RunOnMainThreadAsync(() => IsBusy = false);
				}
		}
}

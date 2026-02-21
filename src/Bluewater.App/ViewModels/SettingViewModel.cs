using System.Collections.ObjectModel;
using Bluewater.App.Helpers;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
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

		public ObservableCollection<DivisionSummary> Divisions { get; } = new();
		public ObservableCollection<DepartmentSummary> Departments { get; } = new();
		public ObservableCollection<SectionSummary> Sections { get; } = new();
		public ObservableCollection<ChargingSummary> Chargings { get; } = new();
		public ObservableCollection<PositionSummary> Positions { get; } = new();
		public ObservableCollection<EmployeeTypeSummary> EmployeeTypes { get; } = new();
		public ObservableCollection<LevelSummary> EmployeeLevels { get; } = new();

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
			IShiftApiService shiftApiService)
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

				EditorTitle = "Title";
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
				if (division is null)
				{
						return;
				}

				await TraceCommandAsync(nameof(DeleteDivisionAsync), division.Id);
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
				if (department is null)
				{
						return;
				}

				await TraceCommandAsync(nameof(DeleteDepartmentAsync), department.Id);
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
				if (section is null)
				{
						return;
				}

				await TraceCommandAsync(nameof(DeleteSectionAsync), section.Id);
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
				if (charging is null)
				{
						return;
				}

				await TraceCommandAsync(nameof(DeleteChargingAsync), charging.Id);
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
		private void UpdateSetting()
		{
				if (EditableSetting is null)
				{
						return;
				}

				switch (EditableSetting.Type)
				{
						case SettingItemType.Division:
								UpdateDivision();
								break;
						case SettingItemType.Department:
								UpdateDepartment();
								break;
						case SettingItemType.Section:
								UpdateSection();
								break;
						case SettingItemType.Charging:
								UpdateCharging();
								break;
						case SettingItemType.Position:
								UpdatePosition();
								break;
				}

				IsEditorOpen = false;
				EditorTitle = string.Empty;
				EditableSetting = null;
		}

		private void UpdateDivision()
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

				int index = Divisions.IndexOf(existing);
				Divisions[index] = EditableSetting.ToDivision(existing.RowIndex);
		}

		private void UpdateDepartment()
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

				int index = Departments.IndexOf(existing);
				Departments[index] = EditableSetting.ToDepartment(existing.RowIndex);
		}

		private void UpdateSection()
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

				int index = Sections.IndexOf(existing);
				Sections[index] = EditableSetting.ToSection(existing.RowIndex);
		}

		private void UpdateCharging()
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

				int index = Chargings.IndexOf(existing);
				Chargings[index] = EditableSetting.ToCharging(existing.RowIndex);
		}

		private void UpdatePosition()
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

				int index = Positions.IndexOf(existing);
				Positions[index] = EditableSetting.ToPosition(existing.RowIndex);
		}

		[RelayCommand]
		private async Task DeletePositionAsync(PositionSummary? position)
		{
				if (position is null)
				{
						return;
				}

				await TraceCommandAsync(nameof(DeletePositionAsync), position.Id);
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

				IsBusy = true;
				int imported = await importAction(rows).ConfigureAwait(false);

				if (imported > 0)
				{
					await onSuccess().ConfigureAwait(false);
				}
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
				IsBusy = false;
			}
		}

		private async Task<int> CreateSettingsAsync<TSetting>(
			IReadOnlyList<SettingsCsvRow> rows,
			Func<SettingsCsvRow, TSetting> map,
			Func<TSetting, CancellationToken, Task<TSetting?>> createAsync)
			where TSetting : class
		{
			int success = 0;

			foreach (SettingsCsvRow row in rows)
			{
				TSetting setting = map(row);
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

		private DepartmentSummary CreateDepartmentFromRow(SettingsCsvRow row) => new()
		{
			Name = row.Name,
			Description = row.Description,
			DivisionId = ResolveId(row, Divisions.Select(i => (i.Id, i.Name)), "division")
		};

		private SectionSummary CreateSectionFromRow(SettingsCsvRow row) => new()
		{
			Name = row.Name,
			Description = row.Description,
			DepartmentId = ResolveId(row, Departments.Select(i => (i.Id, i.Name)), "department")
		};

		private PositionSummary CreatePositionFromRow(SettingsCsvRow row) => new()
		{
			Name = row.Name,
			Description = row.Description,
			SectionId = ResolveId(row, Sections.Select(i => (i.Id, i.Name)), "section")
		};

		private ChargingSummary CreateChargingFromRow(SettingsCsvRow row) => new()
		{
			Name = row.Name,
			Description = row.Description,
			DepartmentId = ResolveOptionalId(row, Departments.Select(i => (i.Id, i.Name)))
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

		private static Guid ResolveId(SettingsCsvRow row, IEnumerable<(Guid Id, string Name)> source, string sourceName)
		{
			Guid? id = ResolveOptionalId(row, source);

			if (id.HasValue && id.Value != Guid.Empty)
			{
				return id.Value;
			}

			throw new FormatException($"{sourceName} reference is required for '{row.Name}'.");
		}

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
						IsBusy = true;

						Divisions.Clear();
						Departments.Clear();
						Sections.Clear();
						Chargings.Clear();
						Positions.Clear();
					EmployeeTypes.Clear();
					EmployeeLevels.Clear();

						var divisionTask = _divisionApiService.GetDivisionsAsync();
						var departmentTask = _departmentApiService.GetDepartmentsAsync();
						var sectionTask = _sectionApiService.GetSectionsAsync();
						var chargingTask = _chargingApiService.GetChargingsAsync();
					var positionTask = _positionApiService.GetPositionsAsync();
					var employeeTypeTask = _employeeTypeApiService.GetEmployeeTypesAsync();
					var levelTask = _levelApiService.GetLevelsAsync();

					await Task.WhenAll(divisionTask, departmentTask, sectionTask, chargingTask, positionTask, employeeTypeTask, levelTask);

						int index = 0;
						foreach (var division in divisionTask.Result.OrderBy(d => d.Name))
						{
								division.RowIndex = index++;
								Divisions.Add(division);
						}

						index = 0;
						foreach (var department in departmentTask.Result.OrderBy(d => d.Name))
						{
								department.RowIndex = index++;
								Departments.Add(department);
						}

						index = 0;
						foreach (var section in sectionTask.Result.OrderBy(s => s.Name))
						{
								section.RowIndex = index++;
								Sections.Add(section);
						}

						index = 0;
						foreach (var charging in chargingTask.Result.OrderBy(c => c.Name))
						{
								charging.RowIndex = index++;
								Chargings.Add(charging);
						}

						index = 0;
					foreach (var position in positionTask.Result.OrderBy(p => p.Name))
					{
						position.RowIndex = index++;
						Positions.Add(position);
					}

					foreach (var employeeType in employeeTypeTask.Result.OrderBy(t => t.Name))
					{
						EmployeeTypes.Add(employeeType);
					}

					foreach (var level in levelTask.Result.OrderBy(l => l.Name))
					{
						EmployeeLevels.Add(level);
					}
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Loading settings data");
				}
				finally
				{
						IsBusy = false;
				}
		}
}

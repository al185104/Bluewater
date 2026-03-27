using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.Services;
using Bluewater.App.ViewModels.Base;
using Bluewater.App.Views.Modals;
using Bluewater.Core.EmployeeAggregate.Enum;
using Bluewater.Core.ShiftAggregate;
using Bluewater.Core.UserAggregate.Enum;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels.Content;

public partial class EmployeeContentViewModel : BaseViewModel
{
		private const string AllChargingName = "All Charging";
		private int skip = 0;
		private int take = 50;
		private readonly List<EmployeeSummary> _allEmployees = [];
		private readonly SemaphoreSlim _searchLoadLock = new(1, 1);
		private bool _hasLoadedAllEmployeesForSearch;
		private bool _isUpdatingSelectedChargingFromSearch;

		// cancellation token
		private CancellationTokenSource? _cts;
		private CancellationTokenSource? _loadMoreCts;
		private readonly IEmployeeApiService _employeeApiService;
		private readonly IReferenceDataService _referenceDataService;
		private readonly IPayApiService _payApiService;
		private readonly IActivityTraceService _activityTraceService;
		private readonly IExceptionHandlingService _exceptionHandlingService;
		private readonly IUserApiService _userApiService;

		public ObservableCollection<ChargingSummary> Chargings { get; } = new();
		public ObservableCollection<EmployeeSummary>? Employees { get; set; } = new();

		[ObservableProperty]
		public partial bool IsLoadingMore { get; set; }

		[ObservableProperty]
		public partial string SearchText { get; set; } = string.Empty;

		[ObservableProperty]
		public partial ChargingSummary? SelectedCharging { get; set; }
		public bool IsEditingEmployee { get; set; }

		public EmployeeContentViewModel(
        IActivityTraceService activityTraceService, 
        IExceptionHandlingService exceptionHandlingService,
        IEmployeeApiService employeeApiService,
				IUserApiService userApiService,
				IPayApiService payApiService,
				IReferenceDataService referenceDataService
		) : base(activityTraceService, exceptionHandlingService)
    {
				_activityTraceService = activityTraceService;
				_exceptionHandlingService = exceptionHandlingService;
        _employeeApiService = employeeApiService;
				_referenceDataService = referenceDataService;
				_payApiService = payApiService;
				_userApiService = userApiService;
		}

		public override async Task InitializeAsync()
		{
				CancelAndDispose();
				_cts = new CancellationTokenSource();
				LoadChargings(selectFirstCharging: true);
				SearchText = string.Empty;
				_hasLoadedAllEmployeesForSearch = false;

				try
				{
						IsBusy = true;
						skip = 0; 
						take = 50;
						var employees = await _employeeApiService.GetEmployeesAsync(skip, take, _cts.Token);
						_allEmployees.Clear();
						ResetEmployees([]);

						if(employees != null && employees.Items.Count > 0)
						{
								_allEmployees.AddRange(employees.Items);
								ApplyEmployeeFilter();
								skip += employees.Items.Count;
						}
				}
				catch (OperationCanceledException)
				{
						await _activityTraceService.LogCommandAsync("EmployeeContentViewModel initialization was canceled.");
				}
				catch (Exception ex)
				{
						_exceptionHandlingService.Handle(ex, "Initializing employees failed.");
				}
				finally
				{
						IsBusy = false;
				}

				await base.InitializeAsync();
    }

		[RelayCommand]
		public async Task LoadMoreEmployeesAsync()
		{
				// prevent overlapping loads
				if (IsLoadingMore)
						return;

				CancelAndDisposeLoadMore();
				_loadMoreCts = new CancellationTokenSource();

				try
				{
						IsLoadingMore = true;
						var employees = await _employeeApiService
								.GetEmployeesAsync(skip: skip, take: take, _loadMoreCts.Token);

						if (employees.Items.Count == 0)
								return; // no more data

						_allEmployees.AddRange(employees.Items);
						AppendVisibleEmployees(employees.Items);

						skip += employees.Items.Count;
				}
				catch (OperationCanceledException)
				{
						await _activityTraceService.LogCommandAsync("Load more employees was canceled.");
				}
				catch (Exception ex)
				{
						_exceptionHandlingService.Handle(ex, "Load more employees");
				}
				finally
				{
						IsLoadingMore = false;
				}
		}

		[RelayCommand]
		public async Task EditEmployeeAsync(EmployeeSummary employee)
		{
				try
				{
						await TraceCommandAsync(nameof(EditEmployeeAsync), employee?.Id);
						IsEditingEmployee = true;

						if (employee is null)
								return;

						await NavigateAsync(
								nameof(EmployeeDetailsPage),
								new Dictionary<string, object>
								{
										["Employee"] = employee
								});
				}
				catch (Exception ex)
				{
						_exceptionHandlingService.Handle(ex, "Opening employee details");
				}
		}

		[RelayCommand]
		public async Task DeleteEmployeeAsync(EmployeeSummary employee)
		{
				CancelAndDispose();
				_cts = new CancellationTokenSource();

				try
				{
						bool confirmed = await Shell.Current.DisplayAlert(
								"Delete shift",
								$"Are you sure you want to delete '{employee.FullName}'?",
								"Yes",
								"No");

						if (!confirmed)
						{
								return;
						}

						IsBusy = true;
						var deleted = await _employeeApiService.DeleteEmployeeAsync(employee.Id, _cts.Token);
						if (!deleted)
						{
								await Shell.Current.DisplayAlert("Delete failed", "Unable to delete employee.", "Okay");
								return;
						}

						_allEmployees.RemoveAll(e => e.Id == employee.Id);
						RemoveVisibleEmployee(employee.Id);
				}
				catch (OperationCanceledException)
				{
						await _activityTraceService.LogCommandAsync("Employee delete was canceled.");
				}
				catch (Exception ex)
				{
						_exceptionHandlingService.Handle(ex, "Deleting employee failed.");
				}
				finally
				{
						IsBusy = false;
				}
		}

		[RelayCommand]
		public async Task ImportEmployeesAsync()
		{
				await TraceCommandAsync(nameof(ImportEmployeesAsync));
				CancelAndDispose();
				_cts = new CancellationTokenSource();

				try
				{
						IsBusy = true;

						var employees = new List<EditableEmployee>();

						var customFileType = new FilePickerFileType(
						new Dictionary<DevicePlatform, IEnumerable<string>>
						{
                { DevicePlatform.WinUI, new[] { ".csv" } }, // file extension
								{ DevicePlatform.macOS, new[] { "csv" } }, // UTType values
						});

						PickOptions options = new()
						{
								PickerTitle = "Please select an employee 201 file",
								FileTypes = customFileType,
						};

						var result = await FilePicker.PickAsync(options);

						if (result == null)
						{
								await Shell.Current.DisplayAlert("Import warning", "There is nothing to import. Please select a correct 201 import file.", "Okay");
								return;
						}

						using var stream = await result.OpenReadAsync();
						using var reader = new StreamReader(stream, Encoding.UTF8);

						var headerLine = await reader.ReadLineAsync();
						if (string.IsNullOrWhiteSpace(headerLine))
						{
								await Shell.Current.DisplayAlert("Import error", "Unable to read a correct header file.", "Back");
								return;
						}

						var headers = headerLine.Split(',').Select(h => h.Trim()).ToList();

						while (!reader.EndOfStream)
						{
								var line = await reader.ReadLineAsync();
								if (string.IsNullOrWhiteSpace(line))
										continue;

								var values = line.Split(',');

								try
								{
										string Get(string name)
										{
												var index = headers.FindIndex(h =>
														h.Equals(name, StringComparison.OrdinalIgnoreCase));
												return index >= 0 && index < values.Length
														? values[index].Trim()
														: string.Empty;
										}

										var positionId = _referenceDataService.Positions.FirstOrDefault(i => i.Name.ToLower().Trim().Equals(Get("Position").ToLower()))?.Id ?? null;
										var typeId = _referenceDataService.EmployeeTypes.FirstOrDefault(i => i.Name.ToLower().Trim().Equals(Get("Type").ToLower()))?.Id ?? null;
										var levelId = _referenceDataService.Levels.FirstOrDefault(i => i.Name.ToLower().Trim().Equals(Get("Level").ToLower()))?.Id ?? null;
										var chargingId = _referenceDataService.Chargings.FirstOrDefault(i => i.Name.ToLower().Trim().Equals(Get("Charging").ToLower()))?.Id ?? null;	
										var payId = Guid.Empty;
										var userId = Guid.Empty;

										var employee = new EditableEmployee
										{
												Id = Guid.NewGuid(),

												FirstName = Get("FirstName"),
												LastName = Get("LastName"),
												MiddleName = Get("MiddleName"),

												DateOfBirth = TryDate(Get("DateOfBirth")),

												Gender = TryEnum(Get("Gender"), Gender.NotSet),
												CivilStatus = TryEnum(Get("CivilStatus"), CivilStatus.NotSet),
												BloodType = TryEnum(Get("BloodType"), BloodType.NotSet),
												Status = TryEnum(Get("Status"), Status.NotSet),

												Height = TryDecimal(Get("Height")),
												Weight = TryDecimal(Get("Weight")),
												Remarks = Get("Remarks"),
												MealCredits = TryInt(Get("MealCredits"), 0),
												Tenant = TryEnum(Get("Project"), Tenant.Maribago),

												Email = Get("Email"),
												TelNumber = Get("TelNumber"),
												MobileNumber = Get("MobileNumber"),
												Address = Get("Address"),
												ProvincialAddress = Get("ProvincialAddress"),

												MothersMaidenName = Get("MothersMaidenName"),
												FathersName = Get("FathersName"),
												EmergencyContact = Get("EmergencyContact"),
												RelationshipContact = Get("RelationshipContact"),
												AddressContact = Get("AddressContact"),
												TelNoContact = Get("TelNoContact"),
												MobileNoContact = Get("MobileNoContact"),

												EducationalAttainment = TryEnum(Get("EducationalAttainment"), EducationalAttainment.NotSet),
												CourseGraduated = Get("CourseGraduated"),
												UniversityGraduated = Get("UniversityGraduated"),

												DateHired = TryDate(Get("DateHired")),
												DateRegularized = TryDate(Get("DateRegularized")),
												DateResigned = TryDate(Get("DateResigned")),
												DateTerminated = TryDate(Get("DateTerminated")),

												TinNo = Get("TinNo"),
												SssNo = Get("SssNo"),
												HdmfNo = Get("HdmfNo"),
												PhicNo = Get("PhicNo"),
												BankAccount = Get("BankAccount"),

												HasServiceCharge = TryBool(Get("HasServiceCharge")),

												Username = Get("Username"),
												PasswordHash = Get("Password"),
												Credential = TryEnum(Get("Credential"), Credential.Employee),// todo: decouple from core

												BasicPay = TryDecimal(Get("BasicPay")),
												DailyRate = TryDecimal(Get("DailyRate")),
												HourlyRate = TryDecimal(Get("HourlyRate")),
												HdmfCon = TryDecimal(Get("HdmfCon")),
												HdmfEr = TryDecimal(Get("HdmfEr")),

												// IDs
												PositionId = positionId,
												TypeId = typeId,
												LevelId = levelId,
												ChargingId = chargingId,
												PayId = payId,
												UserId = userId,

												CreatedDate = DateTime.UtcNow,
												CreateBy = Guid.Empty
										};

										employees.Add(employee);
								}
								catch
								{
										// Skip bad row (log if needed)
										continue;
								}
						}

						// loop this
						int successCount = 0;
						foreach(var employee in employees)
						{
								var user = await _userApiService.CreateUserAsync(new UserRecordDto
								{
										Username = employee.Username,
										PasswordHash = employee.PasswordHash,
										IsGlobalSupervisor = false,
										SupervisedGroup = null,
										Credential = employee.Credential
								}, _cts.Token);

								var pay = await _payApiService.CreatePayAsync(new PayRecordDto
								{
										BasicPay = employee.BasicPay,
										DailyRate = employee.DailyRate,
										HourlyRate = employee.HourlyRate,
										HdmfEmployeeContribution = employee.HdmfCon,
										HdmfEmployerContribution = employee.HdmfEr
								}, _cts.Token);

								var employeePayload = employee.ToCreateRequest(employee.ToSummary(-1));

								if(pay != null)
										employeePayload.PayId = pay.Id;

								if(user != null)
										employeePayload.UserId = user.Id;

								var ret = await _employeeApiService.CreateEmployeeAsync(employeePayload, _cts.Token);
								if (ret)
										successCount++;
						}

						MainThread.BeginInvokeOnMainThread(async () => {
								await Shell.Current.DisplayAlert(
								"Import result",
								$"Successfully imported {successCount} out of {employees.Count} records.",
								"Okay");
						});


						InitializeCommand.Execute(null);
				}
				finally
				{
						IsBusy = false;
				}
		}

		private int TryInt(string value, int defaultValue)
		=> int.TryParse(value, out var v) ? v : defaultValue;

		private decimal? TryDecimal(string value)
				=> decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : null;

		private bool TryBool(string value)
				=> bool.TryParse(value, out var v) && v;

		private DateTime? TryDate(string value)
				=> DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d) ? d : null;

		private T TryEnum<T>(string value, T defaultValue)
				where T : struct, Enum
		{
				if (string.IsNullOrWhiteSpace(value))
						return defaultValue;

				return Enum.TryParse<T>(value, ignoreCase: true, out var result)
						? result
						: defaultValue;
		}

		[RelayCommand]
		public async Task ExportEmployeesAsync()
		{
				await TraceCommandAsync(nameof(ExportEmployeesAsync), new { Count = Employees?.Count ?? 0 });
				if (IsBusy)
				{
						return;
				}

				if (Employees is null || Employees.Count == 0)
				{
						await Shell.Current.DisplayAlert("Export", "No employees to export.", "Okay");
						return;
				}

				try
				{
						IsBusy = true;

						bool confirmed = await Shell.Current.DisplayAlert(
								"Export employees",
								"Export employee records to your Downloads folder?",
								"Yes",
								"No");

						if (!confirmed)
						{
								return;
						}

						var csv = new StringBuilder();
						csv.AppendLine("FirstName,LastName,MiddleName,DateOfBirth,Gender,CivilStatus,BloodType,Status,Tenant,Position,Section,Department,Charging,Type,Level,MealCredits,Email,TelNumber,MobileNumber,Address,ProvincialAddress,DateHired,DateRegularized,DateResigned,DateTerminated,TinNo,SssNo,HdmfNo,PhicNo,BankAccount,HasServiceCharge,BasicPay,DailyRate,HourlyRate");

						foreach (var employee in Employees)
						{
							csv.AppendLine(string.Join(",", new[]
							{
								EscapeCsv(employee.FirstName),
								EscapeCsv(employee.LastName),
								EscapeCsv(employee.MiddleName),
								EscapeCsv(employee.DateOfBirth?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
								EscapeCsv(employee.Gender.ToString()),
								EscapeCsv(employee.CivilStatus.ToString()),
								EscapeCsv(employee.BloodType.ToString()),
								EscapeCsv(employee.Status.ToString()),
								EscapeCsv(employee.Tenant.ToString()),
								EscapeCsv(employee.Position),
								EscapeCsv(employee.Section),
								EscapeCsv(employee.Department),
								EscapeCsv(employee.Charging),
								EscapeCsv(employee.Type),
								EscapeCsv(employee.Level),
								EscapeCsv(employee.MealCredits.ToString(CultureInfo.InvariantCulture)),
								EscapeCsv(employee.ContactInfo.Email),
								EscapeCsv(employee.ContactInfo.TelNumber),
								EscapeCsv(employee.ContactInfo.MobileNumber),
								EscapeCsv(employee.ContactInfo.Address),
								EscapeCsv(employee.ContactInfo.ProvincialAddress),
								EscapeCsv(employee.EmploymentInfo.DateHired?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
								EscapeCsv(employee.EmploymentInfo.DateRegularized?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
								EscapeCsv(employee.EmploymentInfo.DateResigned?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
								EscapeCsv(employee.EmploymentInfo.DateTerminated?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
								EscapeCsv(employee.EmploymentInfo.TinNo),
								EscapeCsv(employee.EmploymentInfo.SssNo),
								EscapeCsv(employee.EmploymentInfo.HdmfNo),
								EscapeCsv(employee.EmploymentInfo.PhicNo),
								EscapeCsv(employee.EmploymentInfo.BankAccount),
								EscapeCsv(employee.EmploymentInfo.HasServiceCharge.ToString()),
								EscapeCsv(employee.Pay.BasicPay?.ToString(CultureInfo.InvariantCulture)),
								EscapeCsv(employee.Pay.DailyRate?.ToString(CultureInfo.InvariantCulture)),
								EscapeCsv(employee.Pay.HourlyRate?.ToString(CultureInfo.InvariantCulture))
							}));
						}

						var fileName = $"employees_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
						var downloadsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
						Directory.CreateDirectory(downloadsDirectory);
						var filePath = Path.Combine(downloadsDirectory, fileName);
						await File.WriteAllTextAsync(filePath, csv.ToString(), Encoding.UTF8);

						await Shell.Current.DisplayAlert("Export", $"Employees exported to {filePath}", "Okay");
				}
				finally
				{
						IsBusy = false;
				}
		}

		private static string EscapeCsv(string? value)
		{
				if (string.IsNullOrEmpty(value))
				{
						return string.Empty;
				}

				var escaped = value.Replace("\"", "\"\"");
				return escaped.IndexOfAny([',', '"', '\n', '\r']) >= 0
						? $"\"{escaped}\""
						: escaped;
		}

		partial void OnSearchTextChanged(string value)
		{
				if (string.IsNullOrWhiteSpace(value))
				{
						ApplyEmployeeFilter();
						return;
				}

				_ = RefreshSearchResultsAsync();
		}

		partial void OnSelectedChargingChanged(ChargingSummary? value)
		{
				if (_isUpdatingSelectedChargingFromSearch)
				{
						return;
				}

				ApplyEmployeeFilter();
		}

		private void ApplyEmployeeFilter()
		{
				IEnumerable<EmployeeSummary> filteredEmployees = _allEmployees;
				if (string.IsNullOrWhiteSpace(SearchText))
				{
						filteredEmployees = filteredEmployees.Where(EmployeeMatchesCharging);
				}
				else
				{
						filteredEmployees = filteredEmployees.Where(EmployeeMatchesSearch);
						UpdateSelectedChargingFromSearchResults(filteredEmployees);
				}

				ResetEmployees(filteredEmployees);
		}

		private void ResetEmployees(IEnumerable<EmployeeSummary> filteredEmployees)
		{
				if (Employees is null)
				{
						return;
				}

				Employees.Clear();
				foreach (var employee in filteredEmployees)
				{
						Employees.Add(employee);
				}
		}

		private void AppendVisibleEmployees(IEnumerable<EmployeeSummary> newEmployees)
		{
				if (Employees is null)
				{
						return;
				}

				IEnumerable<EmployeeSummary> employeesToAdd = newEmployees;
				employeesToAdd = string.IsNullOrWhiteSpace(SearchText)
						? newEmployees.Where(EmployeeMatchesCharging)
						: newEmployees.Where(EmployeeMatchesSearch);

				foreach (var employee in employeesToAdd)
				{
						Employees.Add(employee);
				}
		}

		private void RemoveVisibleEmployee(Guid employeeId)
		{
				if (Employees is null)
				{
						return;
				}

				var visibleEmployee = Employees.FirstOrDefault(e => e.Id == employeeId);
				if (visibleEmployee is not null)
				{
						Employees.Remove(visibleEmployee);
				}
		}

		private void LoadChargings(bool selectFirstCharging = false)
		{
				Chargings.Clear();
				Chargings.Add(new ChargingSummary
				{
						Id = Guid.Empty,
						Name = AllChargingName,
            Description = AllChargingName
        });

				foreach (var charging in _referenceDataService.Chargings.OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase))
				{
						Chargings.Add(charging);
				}

				if (selectFirstCharging || SelectedCharging is null)
				{
						SelectedCharging = Chargings.FirstOrDefault();
						return;
				}

				SelectedCharging = Chargings.FirstOrDefault(charging => charging.Id == SelectedCharging.Id)
						?? Chargings.FirstOrDefault();
		}

		private bool EmployeeMatchesCharging(EmployeeSummary employee)
		{
				if (SelectedCharging is null
						|| SelectedCharging.Id == Guid.Empty
						|| string.Equals(SelectedCharging.Name, AllChargingName, StringComparison.OrdinalIgnoreCase))
				{
						return true;
				}

				return employee.ChargingId == SelectedCharging.Id;
		}

		private bool EmployeeMatchesSearch(EmployeeSummary employee)
		{
				var search = SearchText.Trim();
				return employee.FullName.Contains(search, StringComparison.OrdinalIgnoreCase)
						|| (employee.Position?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
						|| (employee.Department?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
						|| (employee.Section?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
						|| (employee.Type?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
						|| (employee.Level?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
						|| (employee.Email?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false);
		}

		private async Task RefreshSearchResultsAsync()
		{
				try
				{
						await EnsureAllEmployeesLoadedForSearchAsync();
						ApplyEmployeeFilter();
				}
				catch (OperationCanceledException)
				{
						await _activityTraceService.LogCommandAsync("Employee search refresh was canceled.");
				}
				catch (Exception ex)
				{
						_exceptionHandlingService.Handle(ex, "Refreshing employee search");
				}
		}

		private async Task EnsureAllEmployeesLoadedForSearchAsync()
		{
				if (_hasLoadedAllEmployeesForSearch)
				{
						return;
				}

				await _searchLoadLock.WaitAsync();
				try
				{
						if (_hasLoadedAllEmployeesForSearch)
						{
								return;
						}

						while (true)
						{
								CancellationToken token = _cts?.Token ?? CancellationToken.None;
								var employees = await _employeeApiService.GetEmployeesAsync(skip, take, token);
								if (employees?.Items is null || employees.Items.Count == 0)
								{
										_hasLoadedAllEmployeesForSearch = true;
										return;
								}

								_allEmployees.AddRange(employees.Items);
								skip += employees.Items.Count;

								if (employees.Items.Count < take)
								{
										_hasLoadedAllEmployeesForSearch = true;
										return;
								}
						}
				}
				finally
				{
						_searchLoadLock.Release();
				}
		}

		private void UpdateSelectedChargingFromSearchResults(IEnumerable<EmployeeSummary> filteredEmployees)
		{
				if (string.IsNullOrWhiteSpace(SearchText))
				{
						return;
				}

				List<Guid> matchingChargingIds = filteredEmployees
						.Select(employee => employee.ChargingId)
						.OfType<Guid>()
						.Distinct()
						.ToList();

				if (matchingChargingIds.Count == 0)
				{
						return;
				}

				ChargingSummary? nextSelectedCharging = matchingChargingIds.Count == 1
						? Chargings.FirstOrDefault(c => c.Id == matchingChargingIds[0])
						: Chargings.FirstOrDefault(c => c.Id == Guid.Empty);

				if (nextSelectedCharging is null || SelectedCharging?.Id == nextSelectedCharging.Id)
				{
						return;
				}

				_isUpdatingSelectedChargingFromSearch = true;
				try
				{
						SelectedCharging = nextSelectedCharging;
				}
				finally
				{
						_isUpdatingSelectedChargingFromSearch = false;
				}
		}

		public void CancelInitialization()
		{
				_cts?.Cancel();
		}

		public void Dispose()
		{
				CancelAndDispose();
				CancelAndDisposeLoadMore();
				_allEmployees.Clear();
				Employees!.Clear();
				Employees = null;
		}

		private void CancelAndDispose()
		{
				if (_cts is null)
						return;

				_cts.Cancel();
				_cts.Dispose();
				_cts = null;
		}

		private void CancelAndDisposeLoadMore()
		{
				if (_loadMoreCts != null)
				{
						_loadMoreCts.Cancel();
						_loadMoreCts.Dispose();
						_loadMoreCts = null;
				}
		}
}

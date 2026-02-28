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
		private int skip = 0;
		private int take = 50;

		// cancellation token
		private CancellationTokenSource? _cts;
		private CancellationTokenSource? _loadMoreCts;
		private readonly IEmployeeApiService _employeeApiService;
		private readonly IReferenceDataService _referenceDataService;
		private readonly IPayApiService _payApiService;
		private readonly IActivityTraceService _activityTraceService;
		private readonly IExceptionHandlingService _exceptionHandlingService;
		private readonly IUserApiService _userApiService;

		public ObservableCollection<EmployeeSummary>? Employees { get; set; } = new();

		[ObservableProperty]
		public partial bool IsLoadingMore { get; set; }
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

				try
				{
						IsBusy = true;
						skip = 0; 
						take = 50;
						var employees = await _employeeApiService.GetEmployeesAsync(skip, take, _cts.Token);
						if(employees != null && employees.Items.Count > 0)
						{
								Employees!.Clear();
								foreach(var employee in employees.Items)
										Employees.Add(employee);
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
						
						foreach (var employee in employees.Items)
								Employees!.Add(employee);

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
				IsEditingEmployee = true;

				if (employee is null)
						return;

				await Shell.Current.GoToAsync(
						nameof(EmployeeDetailsPage),
						new Dictionary<string, object>
						{
								["Employee"] = employee
						});
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

						Employees?.Remove(employee);
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

								await _employeeApiService.CreateEmployeeAsync(employeePayload, _cts.Token);

								InitializeCommand.Execute(null);
						}
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
				CancelAndDispose();
				_cts = new CancellationTokenSource();

				try
				{
						IsBusy = true;
				}
				finally
				{
						_cts?.Cancel();
						_cts?.Dispose();
						IsBusy = false;
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

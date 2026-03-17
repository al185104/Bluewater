using System.Collections.ObjectModel;
using Bluewater.App.Extensions;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

public partial class LeaveViewModel : BaseViewModel
{
		private readonly ILeaveApiService leaveApiService;
		private readonly IEmployeeApiService employeeApiService;
		private readonly IReferenceDataService referenceDataService;
		private readonly List<LeaveSummary> allLeaves = [];
		private readonly List<EmployeeSummary> allEmployees = [];
		private bool hasInitialized;

		public LeaveViewModel(
			ILeaveApiService leaveApiService,
			IEmployeeApiService employeeApiService,
			IReferenceDataService referenceDataService,
			IActivityTraceService activityTraceService,
			IExceptionHandlingService exceptionHandlingService)
			: base(activityTraceService, exceptionHandlingService)
		{
				this.leaveApiService = leaveApiService;
				this.employeeApiService = employeeApiService;
				this.referenceDataService = referenceDataService;
				EditableLeave = CreateNewLeave();
				AvailableStatuses = Enum.GetValues<ApplicationStatusDto>();
		}

		public ObservableCollection<LeaveSummary> Leaves { get; } = new();
		public ObservableCollection<EmployeeSummary> Employees { get; } = new();
		public ObservableCollection<LeaveCreditSummary> LeaveCredits { get; } = new();

		public IReadOnlyList<ApplicationStatusDto> AvailableStatuses { get; }

		[ObservableProperty]
		public partial LeaveSummary? SelectedLeave { get; set; }

		[ObservableProperty]
		public partial LeaveSummary EditableLeave { get; set; }

		[ObservableProperty]
		public partial TenantDto TenantFilter { get; set; } = TenantDto.Maribago;

		[ObservableProperty]
		public partial string SearchText { get; set; } = string.Empty;

		[ObservableProperty]
		public partial EmployeeSummary? SelectedEmployee { get; set; }

		[ObservableProperty]
		public partial LeaveCreditSummary? SelectedLeaveCredit { get; set; }

		[ObservableProperty]
		public partial ApplicationStatusDto SelectedStatus { get; set; } = ApplicationStatusDto.Pending;

		public override async Task InitializeAsync()
		{
				if (hasInitialized)
				{
						return;
				}

				hasInitialized = true;
				await TraceCommandAsync(nameof(InitializeAsync));
				await LoadReferenceDataAsync();
				await LoadEmployeesAsync();
				await LoadLeavesAsync();
		}

		[RelayCommand]
		private async Task RefreshAsync()
		{
				await TraceCommandAsync(nameof(RefreshAsync));
				await LoadReferenceDataAsync();
				await LoadEmployeesAsync();
				await LoadLeavesAsync();
		}

		[RelayCommand]
		private void BeginCreateLeave()
		{
				EditableLeave = CreateNewLeave();
				SelectedLeave = null;
				SelectedLeaveCredit = null;
				SelectedStatus = ApplicationStatusDto.Pending;

				if (SelectedEmployee is not null)
				{
						EditableLeave.EmployeeId = SelectedEmployee.Id;
						EditableLeave.EmployeeName = SelectedEmployee.FullName;
				}
		}

		[RelayCommand]
		private void BeginEditLeave(LeaveSummary? leave)
		{
				if (leave is null)
				{
						return;
				}

				SelectedLeave = leave;
				EditableLeave = CloneLeave(leave);
				SelectedLeaveCredit = LeaveCredits.FirstOrDefault(credit => credit.Id == leave.LeaveCreditId);
				SelectedStatus = leave.Status;
		}

		[RelayCommand]
		private async Task SaveLeaveAsync()
		{
				if (EditableLeave.EmployeeId is null || EditableLeave.EmployeeId.Value == Guid.Empty)
				{
						return;
				}

				if (SelectedLeaveCredit is null)
				{
						return;
				}

				EditableLeave.LeaveCreditId = SelectedLeaveCredit.Id;
				EditableLeave.LeaveCreditName = SelectedLeaveCredit.Description;
				EditableLeave.Status = SelectedStatus;

				bool isNew = EditableLeave.Id == Guid.Empty;

				try
				{
						IsBusy = true;

						LeaveSummary? saved = isNew
							? await leaveApiService.CreateLeaveAsync(EditableLeave)
							: await leaveApiService.UpdateLeaveAsync(EditableLeave);

						LeaveSummary result = saved ?? EditableLeave;
						int existingIndex = allLeaves.FindIndex(item => item.Id == result.Id);
						if (existingIndex >= 0)
						{
								allLeaves[existingIndex] = result;
						}
						else
						{
								allLeaves.Add(result);
						}

						ApplyLeaveFilter();
						EditableLeave = CloneLeave(result);
						SelectedLeave = result;
						await TraceCommandAsync(nameof(SaveLeaveAsync), result.Id);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, isNew ? "Creating leave" : "Updating leave");
				}
				finally
				{
						IsBusy = false;
				}
		}

		[RelayCommand]
		private async Task DeleteLeaveAsync(LeaveSummary? leave)
		{
				if (leave is null)
				{
						return;
				}

				try
				{
						IsBusy = true;

						bool deleted = await leaveApiService.DeleteLeaveAsync(leave.Id);

						if (deleted)
						{
								allLeaves.RemoveAll(item => item.Id == leave.Id);
								ApplyLeaveFilter();
								await TraceCommandAsync(nameof(DeleteLeaveAsync), leave.Id);
						}
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Deleting leave");
				}
				finally
				{
						IsBusy = false;
				}
		}

		partial void OnSearchTextChanged(string value)
		{
				ApplyEmployeeFilter();
				ApplyLeaveFilter();
		}

		partial void OnSelectedEmployeeChanged(EmployeeSummary? value)
		{
				if (value is not null)
				{
						EditableLeave.EmployeeId = value.Id;
						EditableLeave.EmployeeName = value.FullName;
				}

				ApplyLeaveFilter();
		}

		partial void OnSelectedLeaveCreditChanged(LeaveCreditSummary? value)
		{
				if (value is null)
				{
						return;
				}

				EditableLeave.LeaveCreditId = value.Id;
				EditableLeave.LeaveCreditName = value.Description;
		}

		partial void OnSelectedStatusChanged(ApplicationStatusDto value)
		{
				EditableLeave.Status = value;
		}

		private async Task LoadLeavesAsync()
		{
				try
				{
						IsBusy = true;

						IReadOnlyList<LeaveSummary> leaves = await leaveApiService
							.GetLeavesAsync(tenant: TenantFilter)
							.ConfigureAwait(false);

						allLeaves.Clear();
						allLeaves.AddRange(leaves);
						ApplyLeaveFilter();
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Loading leaves");
				}
				finally
				{
						IsBusy = false;
				}
		}

		private async Task LoadEmployeesAsync()
		{
				try
				{
						const int pageSize = 100;
						int skip = 0;
						allEmployees.Clear();

						while (true)
						{
								PagedResult<EmployeeSummary> page = await employeeApiService.GetEmployeesAsync(skip: skip, take: pageSize);
								if (page.Items.Count == 0)
								{
										break;
								}

								allEmployees.AddRange(page.Items);
								skip += page.Items.Count;

								if (page.Items.Count < pageSize)
								{
										break;
								}
						}

						ApplyEmployeeFilter();
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Loading employees");
				}
		}

		private async Task LoadReferenceDataAsync()
		{
				try
				{
						await referenceDataService.InitializeAsync();
						LeaveCredits.Clear();
						foreach (LeaveCreditSummary leaveCredit in referenceDataService.LeaveCredits)
						{
								LeaveCredits.Add(leaveCredit);
						}
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Loading leave credits");
				}
		}

		private void ApplyEmployeeFilter()
		{
				IEnumerable<EmployeeSummary> filteredEmployees = allEmployees;
				if (!string.IsNullOrWhiteSpace(SearchText))
				{
						string search = SearchText.Trim();
						filteredEmployees = allEmployees.Where(employee =>
							employee.FullName.Contains(search, StringComparison.OrdinalIgnoreCase)
							|| (employee.Position?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
							|| (employee.Department?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
							|| (employee.Section?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));
				}

				Employees.Clear();
				foreach (EmployeeSummary employee in filteredEmployees)
				{
						Employees.Add(employee);
				}

				if (SelectedEmployee is not null && !Employees.Any(employee => employee.Id == SelectedEmployee.Id))
				{
						SelectedEmployee = null;
				}
		}

		private void ApplyLeaveFilter()
		{
				IEnumerable<LeaveSummary> filteredLeaves = allLeaves;
				if (SelectedEmployee is not null)
				{
						filteredLeaves = filteredLeaves.Where(leave => leave.EmployeeId == SelectedEmployee.Id);
				}
				else if (!string.IsNullOrWhiteSpace(SearchText))
				{
						string search = SearchText.Trim();
						filteredLeaves = filteredLeaves.Where(leave =>
							leave.EmployeeName.Contains(search, StringComparison.OrdinalIgnoreCase));
				}

				Leaves.Clear();
				foreach (LeaveSummary leave in filteredLeaves.OrderByDescending(item => item.StartDate))
				{
						Leaves.Add(leave);
				}

				UpdateLeaveRowIndexes();
		}

		private static LeaveSummary CreateNewLeave()
		{
				return new LeaveSummary
				{
						Id = Guid.Empty,
						StartDate = DateTime.Today,
						EndDate = DateTime.Today,
						Status = ApplicationStatusDto.Pending,
						RowIndex = 0
				};
		}

		private static LeaveSummary CloneLeave(LeaveSummary leave)
		{
				return new LeaveSummary
				{
						Id = leave.Id,
						StartDate = leave.StartDate,
						EndDate = leave.EndDate,
						IsHalfDay = leave.IsHalfDay,
						Status = leave.Status,
						EmployeeId = leave.EmployeeId,
						LeaveCreditId = leave.LeaveCreditId,
						EmployeeName = leave.EmployeeName,
						LeaveCreditName = leave.LeaveCreditName,
						RowIndex = leave.RowIndex
				};
		}

		private void UpdateLeaveRowIndexes()
		{
				Leaves.UpdateRowIndexes();
		}
}

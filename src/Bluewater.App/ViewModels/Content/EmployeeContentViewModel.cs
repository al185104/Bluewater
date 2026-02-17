using System.Collections.ObjectModel;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using Bluewater.App.Views.Modals;
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
		private readonly IActivityTraceService _activityTraceService;
		private readonly IExceptionHandlingService _exceptionHandlingService;

		public ObservableCollection<EmployeeSummary>? Employees { get; set; } = new();

		[ObservableProperty]
		public partial bool IsLoadingMore { get; set; }
		public bool IsEditingEmployee { get; set; }

		public EmployeeContentViewModel(
        IActivityTraceService activityTraceService, 
        IExceptionHandlingService exceptionHandlingService,
        IEmployeeApiService employeeApiService
    ) : base(activityTraceService, exceptionHandlingService)
    {
				_activityTraceService = activityTraceService;
				_exceptionHandlingService = exceptionHandlingService;
        _employeeApiService = employeeApiService;
		}

    public override async Task InitializeAsync()
    {
				CancelAndDispose();
				_cts = new CancellationTokenSource();

				try
				{
						IsBusy = true;
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
						IsBusy = true;
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
				}
				finally
				{
						IsBusy = false;
				}
		}

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

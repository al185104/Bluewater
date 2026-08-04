using Bluewater.App.Enums;
using Bluewater.App.Helpers;
using Bluewater.App.Interfaces;
using Bluewater.App.ViewModels.Base;
using Bluewater.Core.UserAggregate.Enum;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

public partial class HomeViewModel : BaseViewModel
{
		[ObservableProperty]
		public partial bool IsToggled { get; set; } = true;

		[ObservableProperty]
		public partial MainSectionEnum CurrentSection { get; set; } = MainSectionEnum.Dashboard;

		[ObservableProperty]
		public partial Credential CurrentCredential { get; set; } = Credential.None;

		[ObservableProperty]
		public partial bool IsDashboardVisible { get; set; }

		[ObservableProperty]
		public partial bool IsEmployeesVisible { get; set; }

		[ObservableProperty]
		public partial bool IsShiftsVisible { get; set; }

		[ObservableProperty]
		public partial bool IsSchedulesVisible { get; set; }

		[ObservableProperty]
		public partial bool IsMealCreditVisible { get; set; }

		[ObservableProperty]
		public partial bool IsLeavesVisible { get; set; }

		[ObservableProperty]
		public partial bool IsFormsVisible { get; set; }

		[ObservableProperty]
		public partial bool IsTimesheetVisible { get; set; }

		[ObservableProperty]
		public partial bool IsPayrollVisible { get; set; }

		[ObservableProperty]
		public partial bool IsProfileVisible { get; set; }

		[ObservableProperty]
		public partial bool IsSettingsVisible { get; set; }

		public event Func<MainSectionEnum, Task>? NavigateRequested;

		public event Func<Task>? LogoutRequested;

		private readonly IDashboardApiService _dashboardApiService;

		public HomeViewModel(
		IDashboardApiService dashboardApiService,
		IActivityTraceService activityTraceService,
		IExceptionHandlingService exceptionHandlingService)
		: base(activityTraceService, exceptionHandlingService)
		{
				_dashboardApiService = dashboardApiService;
				TenantPreferences.EnsureSelectedTenant();
				ApplyNavigationAccess();
		}

		public override Task InitializeAsync()
		{
				ApplyNavigationAccess();
				return Task.CompletedTask;
		}

		[RelayCommand]
		async Task ToggleMenuAsync()
		{
				try
				{
						IsToggled = !IsToggled;
						await TraceCommandAsync(nameof(ToggleMenuAsync), new { IsToggled }).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Toggling main menu");
				}
		}

		[RelayCommand]
		private async Task NavigateAsync(MainSectionEnum section)
		{
				try
				{
						if (!CanNavigate(section))
						{
								return;
						}

						CurrentSection = section;
            await TraceCommandAsync(nameof(NavigateAsync), new { Section = section.ToString() }).ConfigureAwait(false);

						var handler = NavigateRequested;
						if (handler is null) return;

            MainThread.BeginInvokeOnMainThread(async () => { 
						      await handler.Invoke(section);
            });
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, $"Navigating to {section}");
				}
		}

		[RelayCommand]
		private async Task LogoutAsync()
		{
				try
				{
						await TraceCommandAsync(nameof(LogoutAsync), new { Username = LoginSession.CurrentUsername }).ConfigureAwait(false);
						LoginSession.ClearCurrentUser();

						var handler = LogoutRequested;
						if (handler is null)
						{
								await MainThread.InvokeOnMainThreadAsync(async () =>
								{
										Shell? shell = Shell.Current ?? Application.Current?.Windows.FirstOrDefault()?.Page as Shell;
										if (shell is not null)
										{
												await shell.GoToAsync("//LoginPage");
										}
								});
								return;
						}

						await MainThread.InvokeOnMainThreadAsync(async () => await handler.Invoke());
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Logging out");
				}
		}

		private void ApplyNavigationAccess()
		{
				CurrentCredential = LoginSession.CurrentCredential;

				IsProfileVisible = HasCredential(
					Credential.Employee,
					Credential.Scheduler,
					Credential.Payroll,
					Credential.Manager,
					Credential.Supervisor,
					Credential.Admin,
					Credential.SuperAdmin);

				IsDashboardVisible = HasCredential(
					Credential.Employee,
					Credential.Scheduler,
					Credential.Payroll,
					Credential.Manager,
					Credential.Supervisor,
					Credential.Admin,
					Credential.SuperAdmin);

				IsPayrollVisible = HasCredential(
					Credential.Payroll,
					Credential.Manager,
					Credential.Supervisor,
					Credential.Admin,
					Credential.SuperAdmin);

				IsEmployeesVisible = HasCredential(
					Credential.Manager,
					Credential.Supervisor,
					Credential.Admin,
					Credential.SuperAdmin);

				IsShiftsVisible = HasCredential(
					Credential.Scheduler,
					Credential.Manager,
					Credential.Supervisor,
					Credential.Admin,
					Credential.SuperAdmin);

				IsSchedulesVisible = HasCredential(
					Credential.Scheduler,
					Credential.Manager,
					Credential.Supervisor,
					Credential.Admin,
					Credential.SuperAdmin);

				IsMealCreditVisible = false;

				IsLeavesVisible = HasCredential(
					Credential.Employee,
					Credential.Manager,
					Credential.Supervisor,
					Credential.Admin,
					Credential.SuperAdmin);

				IsFormsVisible = HasCredential(
					Credential.Employee,
					Credential.Payroll,
					Credential.Manager,
					Credential.Supervisor,
					Credential.Admin,
					Credential.SuperAdmin);

				IsTimesheetVisible = HasCredential(
					Credential.Scheduler,
					Credential.Payroll,
					Credential.Manager,
					Credential.Supervisor,
					Credential.Admin,
					Credential.SuperAdmin);

				IsSettingsVisible = HasCredential(Credential.Admin, Credential.SuperAdmin);

				if (!CanNavigate(CurrentSection))
				{
						CurrentSection = IsDashboardVisible ? MainSectionEnum.Dashboard : MainSectionEnum.Profile;
				}
		}

		private bool CanNavigate(MainSectionEnum section)
		{
				return section switch
				{
						MainSectionEnum.Dashboard => IsDashboardVisible,
						MainSectionEnum.Employees => IsEmployeesVisible,
						MainSectionEnum.Shifts => IsShiftsVisible,
						MainSectionEnum.Schedules => IsSchedulesVisible,
						MainSectionEnum.MealCredit => IsMealCreditVisible,
						MainSectionEnum.Leaves => IsLeavesVisible,
						MainSectionEnum.Timesheet => IsTimesheetVisible,
						MainSectionEnum.Attendance => false,
						MainSectionEnum.Payroll => IsPayrollVisible,
						MainSectionEnum.Profile => IsProfileVisible,
						MainSectionEnum.Users => false,
						MainSectionEnum.Forms => IsFormsVisible,
						MainSectionEnum.Settings => IsSettingsVisible,
						_ => false
				};
		}

		private bool HasCredential(params Credential[] allowedCredentials)
		{
				return allowedCredentials.Contains(CurrentCredential);
		}

}

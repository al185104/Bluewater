using Bluewater.App.Enums;
using Bluewater.App.ViewModels;
using Bluewater.App.ViewModels.Content;
using Bluewater.App.Views.Controls;

namespace Bluewater.App.Views;

public sealed partial class HomePage : ContentPage
{
		private readonly IServiceProvider _services;

		public HomePage(HomeViewModel vm, IServiceProvider services)
		{
				InitializeComponent();
				BindingContext = vm;
				_services = services;
				vm.NavigateRequested += OnNavigateRequestedAsync;

				Host.Content = services.GetRequiredService<DashboardView>();
		}

		private Task OnNavigateRequestedAsync(MainSectionEnum section)
		{
				Host.Content = section switch
				{
						MainSectionEnum.Dashboard => _services.GetRequiredService<DashboardView>(),
						MainSectionEnum.Employees => _services.GetRequiredService<EmployeesView>(),
						MainSectionEnum.Shifts => _services.GetRequiredService<ShiftsView>(),
						MainSectionEnum.Schedules => _services.GetRequiredService<SchedulesView>(),
						MainSectionEnum.MealCredit => _services.GetRequiredService<MealCreditView>(),
						MainSectionEnum.Leaves => _services.GetRequiredService<LeavesView>(),
						MainSectionEnum.Timesheet => _services.GetRequiredService<TimesheetView>(),
						MainSectionEnum.Attendance => _services.GetRequiredService<AttendanceView>(),
						MainSectionEnum.Payroll => _services.GetRequiredService<PayrollView>(),
						MainSectionEnum.Users => _services.GetRequiredService<UsersView>(),
						_ => _services.GetRequiredService<DashboardView>()
				};

				return Task.CompletedTask;
		}

		protected override async void OnAppearing()
		{
				base.OnAppearing();

				if (BindingContext is HomeViewModel viewModel)
				{
						await viewModel.InitializeAsync();
				}
		}

}

using Bluewater.App.Enums;
using Bluewater.App.ViewModels;
using Bluewater.App.ViewModels.Content;
using Bluewater.App.Views.Controls;
using CommunityToolkit.Maui.Extensions;

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
				vm.LogoutRequested += OnLogoutRequestedAsync;

				SetHostContent(services.GetRequiredService<DashboardView>());
		}

		private Task OnNavigateRequestedAsync(MainSectionEnum section)
		{
				SetHostContent(section switch
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
						MainSectionEnum.Profile => _services.GetRequiredService<ProfileView>(),
						MainSectionEnum.Forms => _services.GetRequiredService<FormsView>(),
						MainSectionEnum.Settings => _services.GetRequiredService<SettingsView>(),
						_ => _services.GetRequiredService<DashboardView>()
				});

				return Task.CompletedTask;
		}

		private async Task OnLogoutRequestedAsync()
		{
				DisposeHostContent();
				if (BindingContext is HomeViewModel viewModel)
				{
						viewModel.CurrentSection = MainSectionEnum.Dashboard;
				}

				Shell? shell = FindParentShell() ?? Application.Current?.Windows.FirstOrDefault()?.Page as Shell;
				if (shell is null)
				{
						return;
				}

				await shell.GoToAsync("//LoginPage");
		}

		private Shell? FindParentShell()
		{
				Element? current = this;
				while (current is not null)
				{
						if (current is Shell shell)
						{
								return shell;
						}

						current = current.Parent;
				}

				return null;
		}

		private void SetHostContent(View content)
		{
				DisposeHostContent();
				Host.Content = content;
		}

		private void DisposeHostContent()
		{
				if (Host.Content is null)
				{
						return;
				}

				if (Host.Content.BindingContext is IDisposable disposableContext)
				{
						disposableContext.Dispose();
				}

				if (Host.Content is IDisposable disposableContent)
				{
						disposableContent.Dispose();
				}

				Host.Content = null;
		}

		protected override async void OnAppearing()
		{
				base.OnAppearing();

				if (BindingContext is HomeViewModel viewModel)
				{
						await viewModel.InitializeAsync();
						if (Host.Content is null)
						{
								await OnNavigateRequestedAsync(viewModel.CurrentSection);
						}
				}
		}

		private void btnFeedback_Clicked(object sender, EventArgs e)
		{
				// test of exception
				throw new Exception("Something went wrong when clicking the feedback button!");
    }

		private void btnWhatsNew_Clicked(object sender, EventArgs e)
		{
				this.ShowPopup(new WhatsNewPopup());
		}
}

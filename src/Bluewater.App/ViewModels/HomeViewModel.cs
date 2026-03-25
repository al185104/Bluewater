using Bluewater.App.Enums;
using Bluewater.App.Helpers;
using Bluewater.App.Interfaces;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

public partial class HomeViewModel : BaseViewModel
{
		[ObservableProperty]
		public partial bool IsToggled { get; set; } = true;

		[ObservableProperty]
		public partial MainSectionEnum CurrentSection { get; set; } = MainSectionEnum.Dashboard;

		public event Func<MainSectionEnum, Task>? NavigateRequested;

		private readonly IDashboardApiService _dashboardApiService;

		public HomeViewModel(
		IDashboardApiService dashboardApiService,
		IActivityTraceService activityTraceService,
		IExceptionHandlingService exceptionHandlingService)
		: base(activityTraceService, exceptionHandlingService)
		{
				_dashboardApiService = dashboardApiService;
				TenantPreferences.EnsureSelectedTenant();
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
}

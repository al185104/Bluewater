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
				IsToggled = !IsToggled;
		}

		[RelayCommand]
		private async Task NavigateAsync(MainSectionEnum section)
		{
				var handler = NavigateRequested;
				if (handler is null) return;

				await handler.Invoke(section);
		}
}

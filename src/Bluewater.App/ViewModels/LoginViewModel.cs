using System.Diagnostics.CodeAnalysis;
using Bluewater.App.Interfaces;
using Bluewater.App.ViewModels.Base;
using Bluewater.App.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "CommunityToolkit.Mvvm RelayCommand attributes are platform-agnostic in .NET MAUI view models.")]
public partial class LoginViewModel : BaseViewModel
{
		private readonly IReferenceDataService referenceDataService;

		public LoginViewModel(
			IActivityTraceService activityTraceService,
			IExceptionHandlingService exceptionHandlingService,
			IReferenceDataService referenceDataService)
			: base(activityTraceService, exceptionHandlingService)
		{
				this.referenceDataService = referenceDataService;
		}

		[ObservableProperty]
		public partial bool ShowRefreshButton { get; set; }

		public override Task InitializeAsync()
		{
				ShowRefreshButton = referenceDataService.HasInitializationFailed;
				return Task.CompletedTask;
		}

		[RelayCommand]
		async Task LoginAsync()
		{
				try
				{
						IsBusy = true;
						await TraceCommandAsync("Login", new { Target = nameof(HomePage) }).ConfigureAwait(false);
						MainThread.BeginInvokeOnMainThread(async () =>
						{
								await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
						});
				}
				finally
				{
						IsBusy = false;
				}
		}

		[RelayCommand]
		async Task RefreshReferenceDataAsync()
		{
				try
				{
						IsBusy = true;
						await referenceDataService.InitializeAsync().ConfigureAwait(false);
						ShowRefreshButton = referenceDataService.HasInitializationFailed;
				}
				catch (Exception ex)
				{
						ShowRefreshButton = true;
						ExceptionHandlingService.Handle(ex, "Refreshing reference data");
				}
				finally
				{
						IsBusy = false;
				}
		}
}

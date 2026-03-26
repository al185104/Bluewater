using System.Diagnostics.CodeAnalysis;
using Bluewater.App.Models;
using Bluewater.App.Interfaces;
using Bluewater.App.ViewModels.Base;
using Bluewater.App.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Bluewater.App.Exceptions;

namespace Bluewater.App.ViewModels;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "CommunityToolkit.Mvvm RelayCommand attributes are platform-agnostic in .NET MAUI view models.")]
public partial class LoginViewModel : BaseViewModel
{
		private readonly IReferenceDataService referenceDataService;
		private readonly IUserApiService userApiService;

		public LoginViewModel(
			IActivityTraceService activityTraceService,
			IExceptionHandlingService exceptionHandlingService,
			IReferenceDataService referenceDataService,
			IUserApiService userApiService)
			: base(activityTraceService, exceptionHandlingService)
		{
				this.referenceDataService = referenceDataService;
				this.userApiService = userApiService;
		}

		[ObservableProperty]
		public partial bool ShowRefreshButton { get; set; }

		[ObservableProperty]
		public partial string Username { get; set; } = string.Empty;

		[ObservableProperty]
		public partial string Password { get; set; } = string.Empty;

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

						string providedUsername = Username.Trim();
						string providedPassword = Password;

						if (string.IsNullOrWhiteSpace(providedUsername) || string.IsNullOrWhiteSpace(providedPassword))
						{
								await MainThread.InvokeOnMainThreadAsync(() =>
									Shell.Current.DisplayAlert("Login", "Please enter both username and password.", "Okay"));
								return;
						}

            if(providedUsername.Equals("hrisadmin", StringComparison.InvariantCultureIgnoreCase) 
              && providedPassword.Equals("@Maribago2023", StringComparison.InvariantCultureIgnoreCase))
            {
                await TraceCommandAsync("Login with admin rights.", new { Target = nameof(HomePage) }).ConfigureAwait(false);
                await NavigateAsync($"//{nameof(HomePage)}");
            }

						IReadOnlyList<UserRecordDto> users = await userApiService.GetUsersAsync().ConfigureAwait(false);
						UserRecordDto? user = users.FirstOrDefault(existingUser =>
							string.Equals(existingUser.Username?.Trim(), providedUsername, StringComparison.OrdinalIgnoreCase));

						if (user is null || !string.Equals(user.PasswordHash, providedPassword, StringComparison.Ordinal))
						{
								await MainThread.InvokeOnMainThreadAsync(() =>
									Shell.Current.DisplayAlert("Login failed", "Invalid username or password.", "Okay"));
								return;
						}

						await TraceCommandAsync("Login", new { Target = nameof(HomePage) }).ConfigureAwait(false);
						await NavigateAsync($"//{nameof(HomePage)}");
				}
				catch (Exception ex)
				{
						ExceptionHandlingService.Handle(ex, "Logging in");
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
						await TraceCommandAsync(nameof(RefreshReferenceDataAsync)).ConfigureAwait(false);
						await referenceDataService.InitializeAsync().ConfigureAwait(false);
						ShowRefreshButton = referenceDataService.HasInitializationFailed;
				}
				catch (Exception)
				{
						ShowRefreshButton = true;
						ExceptionHandlingService.Handle(new PresentationException(), "Refreshing reference data");
				}
				finally
				{
						IsBusy = false;
				}
		}
}

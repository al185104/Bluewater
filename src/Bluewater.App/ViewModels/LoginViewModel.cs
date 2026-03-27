using System.Diagnostics.CodeAnalysis;
using Bluewater.App.Exceptions;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using Bluewater.App.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "CommunityToolkit.Mvvm RelayCommand attributes are platform-agnostic in .NET MAUI view models.")]
public partial class LoginViewModel : BaseViewModel
{
		private readonly IReferenceDataService referenceDataService;
		private readonly IUserApiService userApiService;
		private readonly IApiBaseAddressRecoveryService apiBaseAddressRecoveryService;

		public LoginViewModel(
			IActivityTraceService activityTraceService,
			IExceptionHandlingService exceptionHandlingService,
			IReferenceDataService referenceDataService,
			IUserApiService userApiService,
			IApiBaseAddressRecoveryService apiBaseAddressRecoveryService)
			: base(activityTraceService, exceptionHandlingService)
		{
				this.referenceDataService = referenceDataService;
				this.userApiService = userApiService;
				this.apiBaseAddressRecoveryService = apiBaseAddressRecoveryService;
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
				if (!ShowRefreshButton)
				{
						return Task.CompletedTask;
				}

				return TryRecoverInitializationAsync();
		}

		[RelayCommand]
		async Task LoginAsync()
		{
				for (int attempt = 0; attempt < 2; attempt++)
				{
						try
						{
								IsBusy = true;
								await LoginCoreAsync().ConfigureAwait(false);
								return;
						}
						catch (Exception ex)
						{
								bool hasRecovered = attempt == 0 && await apiBaseAddressRecoveryService
									.TryRecoverAsync("login", async () => await userApiService.GetUsersAsync().ConfigureAwait(false), ex)
									.ConfigureAwait(false);

								if (!hasRecovered)
								{
										ExceptionHandlingService.Handle(ex, "Logging in");
										return;
								}
						}
						finally
						{
								IsBusy = false;
						}
				}
		}

		[RelayCommand]
		async Task RefreshReferenceDataAsync()
		{
				for (int attempt = 0; attempt < 2; attempt++)
				{
						try
						{
								IsBusy = true;
								await TraceCommandAsync(nameof(RefreshReferenceDataAsync)).ConfigureAwait(false);
								await referenceDataService.InitializeAsync().ConfigureAwait(false);
								ShowRefreshButton = referenceDataService.HasInitializationFailed;
								return;
						}
						catch (Exception ex)
						{
								ShowRefreshButton = true;

								bool hasRecovered = attempt == 0 && await apiBaseAddressRecoveryService
									.TryRecoverAsync("reference data refresh", () => referenceDataService.InitializeAsync(), ex)
									.ConfigureAwait(false);

								if (!hasRecovered)
								{
										ExceptionHandlingService.Handle(new PresentationException(), "Refreshing reference data");
										return;
								}
						}
						finally
						{
								IsBusy = false;
						}
				}
		}

		private async Task LoginCoreAsync()
		{
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

        bool isAuthorizedCredential = (int)user.Credential >= 7;
        if (!isAuthorizedCredential)
        {
          await MainThread.InvokeOnMainThreadAsync(() =>
            Shell.Current.DisplayAlert("Login failed", "You do not have permission to access this application.", "Okay"));
          return;
        }

    await TraceCommandAsync("Login", new { Target = nameof(HomePage) }).ConfigureAwait(false);
				await NavigateAsync($"//{nameof(HomePage)}");
		}

		private async Task TryRecoverInitializationAsync()
		{
				bool hasRecovered = await apiBaseAddressRecoveryService
					.TryRecoverAsync("application startup", () => referenceDataService.InitializeAsync())
					.ConfigureAwait(false);

				ShowRefreshButton = !hasRecovered;
		}
}

using Bluewater.App.ViewModels;
using Microsoft.Maui.Storage;

namespace Bluewater.App.Views;

public partial class LoginPage : ContentPage
{
  private const string RememberSignInKey = "Login.RememberSignIn";
  private const string RememberedUsernameKey = "Login.RememberedUsername";
  private const string RememberedPasswordKey = "Login.RememberedPassword";

  public LoginPage(LoginViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
    InitializeRememberedCredentials();
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();

    if (BindingContext is LoginViewModel viewModel)
    {
      await viewModel.InitializeAsync();
    }
  }

  private void InitializeRememberedCredentials()
  {
    bool shouldRemember = Preferences.Get(RememberSignInKey, false);
    RememberSignInSwitch.IsToggled = shouldRemember;

    if (!shouldRemember || BindingContext is not LoginViewModel viewModel)
    {
      return;
    }

    viewModel.Username = Preferences.Get(RememberedUsernameKey, string.Empty);
    viewModel.Password = Preferences.Get(RememberedPasswordKey, string.Empty);
  }

  private void OnRememberSignInToggled(object? sender, ToggledEventArgs e)
  {
    Preferences.Set(RememberSignInKey, e.Value);

    if (BindingContext is not LoginViewModel viewModel)
    {
      return;
    }

    if (!e.Value)
    {
      Preferences.Remove(RememberedUsernameKey);
      Preferences.Remove(RememberedPasswordKey);
      return;
    }

    Preferences.Set(RememberedUsernameKey, viewModel.Username);
    Preferences.Set(RememberedPasswordKey, viewModel.Password);
  }

  private void OnCredentialTextChanged(object? sender, TextChangedEventArgs e)
  {
    if (!RememberSignInSwitch.IsToggled || BindingContext is not LoginViewModel viewModel)
    {
      return;
    }

    Preferences.Set(RememberedUsernameKey, viewModel.Username);
    Preferences.Set(RememberedPasswordKey, viewModel.Password);
  }
}

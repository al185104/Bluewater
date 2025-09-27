using Bluewater.App.ViewModels;

namespace Bluewater.App.Views;

public partial class LoginPage : ContentPage
{
  public LoginPage(LoginViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }
}

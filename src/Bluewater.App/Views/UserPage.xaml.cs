using Bluewater.App.ViewModels;

namespace Bluewater.App.Views;

public partial class UserPage : ContentPage
{
  public UserPage(UserViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }
}

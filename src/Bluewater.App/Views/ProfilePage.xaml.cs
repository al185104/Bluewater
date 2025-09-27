using Bluewater.App.ViewModels;

namespace Bluewater.App.Views;

public partial class ProfilePage : ContentPage
{
  public ProfilePage(ProfileViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }
}

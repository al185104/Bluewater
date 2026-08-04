using Bluewater.App.ViewModels;

namespace Bluewater.App.Views.Controls;

public partial class ProfileView : ContentView
{
  public ProfileView(ProfileViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }

  private async void ContentView_Loaded(object sender, EventArgs e)
  {
    if (BindingContext is ProfileViewModel vm)
    {
      await vm.InitializeAsync();
    }
  }
}

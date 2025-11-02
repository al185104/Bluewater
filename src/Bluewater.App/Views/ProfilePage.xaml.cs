using Bluewater.App.ViewModels;

namespace Bluewater.App.Views;

public partial class ProfilePage : ContentPage
{
  public ProfilePage(ProfileViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();

    if (BindingContext is ProfileViewModel viewModel)
    {
      await viewModel.InitializeAsync();
    }
  }
}

using Bluewater.App.ViewModels;

namespace Bluewater.App.Views;

public partial class UserPage : ContentPage
{
  public UserPage(UserViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();

    if (BindingContext is UserViewModel viewModel)
    {
      await viewModel.InitializeAsync();
    }
  }
}

using Bluewater.App.ViewModels;

namespace Bluewater.App.Views;

public partial class HomePage : ContentPage
{
  public HomePage(HomeViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();

    if (BindingContext is HomeViewModel viewModel)
    {
      await viewModel.InitializeAsync();
    }
  }
}

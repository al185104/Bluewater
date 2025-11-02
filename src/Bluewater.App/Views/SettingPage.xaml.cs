using Bluewater.App.ViewModels;

namespace Bluewater.App.Views;

public partial class SettingPage : ContentPage
{
  public SettingPage(SettingViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();

    if (BindingContext is SettingViewModel viewModel)
    {
      await viewModel.InitializeAsync();
    }
  }
}

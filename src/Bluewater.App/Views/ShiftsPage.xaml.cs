using Bluewater.App.ViewModels;

namespace Bluewater.App.Views;

public partial class ShiftsPage : ContentPage
{
  public ShiftsPage(ShiftsViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();

    if (BindingContext is ShiftsViewModel viewModel)
    {
      await viewModel.InitializeAsync();
    }
  }
}

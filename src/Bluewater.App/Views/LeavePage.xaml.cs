using Bluewater.App.ViewModels;

namespace Bluewater.App.Views;

public partial class LeavePage : ContentPage
{
  public LeavePage(LeaveViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();

    if (BindingContext is LeaveViewModel viewModel)
    {
      await viewModel.InitializeAsync();
    }
  }
}

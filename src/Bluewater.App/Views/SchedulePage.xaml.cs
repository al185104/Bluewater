using Bluewater.App.ViewModels;

namespace Bluewater.App.Views;

public partial class SchedulePage : ContentPage
{
  public SchedulePage(ScheduleViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();

    if (BindingContext is ScheduleViewModel viewModel)
    {
      await viewModel.InitializeAsync();
    }
  }
}

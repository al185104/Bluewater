using Bluewater.App.ViewModels;

namespace Bluewater.App.Views;

public partial class TimesheetPage : ContentPage
{
  public TimesheetPage(TimesheetViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();

    if (BindingContext is TimesheetViewModel viewModel)
    {
      await viewModel.InitializeAsync();
    }
  }
}

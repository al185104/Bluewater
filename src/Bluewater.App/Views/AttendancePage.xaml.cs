using Bluewater.App.ViewModels;

namespace Bluewater.App.Views;

public partial class AttendancePage : ContentPage
{
  public AttendancePage(AttendanceViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();

    if (BindingContext is AttendanceViewModel viewModel)
    {
      await viewModel.InitializeAsync();
    }
  }
}

using Bluewater.App.ViewModels;

namespace Bluewater.App.Views;

public partial class EmployeePage : ContentPage
{
  public EmployeePage(EmployeeViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }

  protected override async void OnAppearing()
  {
    if (BindingContext is EmployeeViewModel viewModel)
    {
      await viewModel.InitializeAsync();
    }
  }
}

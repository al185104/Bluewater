using Bluewater.App.ViewModels;

namespace Bluewater.App.Views;

public partial class MealCreditPage : ContentPage
{
  public MealCreditPage(MealCreditViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();

    if (BindingContext is MealCreditViewModel viewModel)
    {
      await viewModel.InitializeAsync();
    }
  }
}

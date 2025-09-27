using Bluewater.App.ViewModels;

namespace Bluewater.App.Views;

public partial class MealCreditPage : ContentPage
{
  public MealCreditPage(MealCreditViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }
}

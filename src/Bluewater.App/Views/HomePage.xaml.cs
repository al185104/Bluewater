using Bluewater.App.ViewModels;

namespace Bluewater.App.Views;

public partial class HomePage : ContentPage
{
  public HomePage(HomeViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }
}

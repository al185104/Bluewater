using Bluewater.App.ViewModels;

namespace Bluewater.App.Views;

public partial class ShiftsPage : ContentPage
{
  public ShiftsPage(ShiftsViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }
}

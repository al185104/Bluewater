using Bluewater.App.ViewModels;

namespace Bluewater.App.Views;

public partial class PayrollPage : ContentPage
{
  public PayrollPage(PayrollViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }
}

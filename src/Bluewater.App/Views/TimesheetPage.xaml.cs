using Bluewater.App.ViewModels;

namespace Bluewater.App.Views;

public partial class TimesheetPage : ContentPage
{
  public TimesheetPage(TimesheetViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }
}

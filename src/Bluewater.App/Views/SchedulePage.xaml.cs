using Bluewater.App.ViewModels;

namespace Bluewater.App.Views;

public partial class SchedulePage : ContentPage
{
  public SchedulePage(ScheduleViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }
}

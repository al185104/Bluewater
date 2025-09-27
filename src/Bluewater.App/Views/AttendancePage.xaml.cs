using Bluewater.App.ViewModels;

namespace Bluewater.App.Views;

public partial class AttendancePage : ContentPage
{
  public AttendancePage(AttendanceViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }
}

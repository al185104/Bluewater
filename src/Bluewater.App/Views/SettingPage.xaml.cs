using Bluewater.App.ViewModels;

namespace Bluewater.App.Views;

public partial class SettingPage : ContentPage
{
  public SettingPage(SettingViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }
}

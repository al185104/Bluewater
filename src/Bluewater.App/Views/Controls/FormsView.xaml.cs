using Bluewater.App.ViewModels;

namespace Bluewater.App.Views.Controls;

public partial class FormsView : ContentView
{
  public FormsView(FormsViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }

  private void ContentView_Loaded(object sender, EventArgs e)
  {
    if (BindingContext is FormsViewModel vm)
    {
      _ = vm.InitializeAsync();
    }
  }

  private void ContentView_Unloaded(object sender, EventArgs e)
  {
  }
}

using Bluewater.App.ViewModels.Content;

namespace Bluewater.App.Views.Controls;

public partial class DashboardView : ContentView
{
	public DashboardView(DashboardContentViewModel vm)
	{
		InitializeComponent();
    BindingContext = vm;
  }
}

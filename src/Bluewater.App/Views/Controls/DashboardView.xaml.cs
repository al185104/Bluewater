using Bluewater.App.ViewModels.Content;

namespace Bluewater.App.Views.Controls;

public partial class DashboardView : ContentView
{
		public DashboardView(DashboardContentViewModel vm)
		{
				InitializeComponent();
				BindingContext = vm;
		}

		private async void ContentView_Loaded(object sender, EventArgs e)
		{
				if (BindingContext is DashboardContentViewModel vm)
				{
						await vm.InitializeAsync();
				}
		}

		private void ContentView_Unloaded(object sender, EventArgs e)
		{

		}
}

using Bluewater.App.ViewModels;

namespace Bluewater.App.Views.Controls;

public partial class PayrollView : ContentView
{
		public PayrollView(PayrollViewModel vm)
		{
				InitializeComponent();
				BindingContext = vm;
		}

		private async void ContentView_Loaded(object sender, EventArgs e)
		{
				if (BindingContext is PayrollViewModel vm)
				{
						await vm.InitializeAsync();
				}
		}

		private void ContentView_Unloaded(object sender, EventArgs e)
		{
				if (BindingContext is PayrollViewModel vm)
				{
						// reserved for cleanup when disposal is implemented
				}
		}
}

using Bluewater.App.ViewModels.Content;

namespace Bluewater.App.Views.Controls;

public partial class ShiftsView : ContentView
{
		public ShiftsView(ShiftContentViewModel vm)
		{
				InitializeComponent();
				BindingContext = vm;
		}

		private async void ContentView_Loaded(object sender, EventArgs e)
		{
				if (BindingContext is ShiftContentViewModel vm)
				{
						await vm.InitializeAsync();
				}
		}

		private void ContentView_Unloaded(object sender, EventArgs e)
		{
				if (BindingContext is ShiftContentViewModel vm)
				{
						vm.Dispose();
						BindingContext = null;
				}
		}
}

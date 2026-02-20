using Bluewater.App.ViewModels;

namespace Bluewater.App.Views.Controls;

public partial class TimesheetView : ContentView
{
		public TimesheetView(TimesheetsViewModel vm)
		{
				InitializeComponent();
				BindingContext = vm;
		}

		private async void ContentView_Loaded(object sender, EventArgs e)
		{
				if (BindingContext is TimesheetsViewModel vm)
				{
						await vm.InitializeAsync();
				}
		}

		private void ContentView_Unloaded(object sender, EventArgs e)
		{
				if (BindingContext is TimesheetsViewModel vm)
				{
						//vm.Dispose();
						//BindingContext = null;
				}
		}
}

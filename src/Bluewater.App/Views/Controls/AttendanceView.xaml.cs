using Bluewater.App.ViewModels;

namespace Bluewater.App.Views.Controls;

public partial class AttendanceView : ContentView
{
		public AttendanceView(AttendanceViewModel vm)
		{
				InitializeComponent();
				BindingContext = vm;
		}

		private async void ContentView_Loaded(object sender, EventArgs e)
		{
				if (BindingContext is AttendanceViewModel vm)
				{
						await vm.InitializeAsync();
				}
		}

		private void ContentView_Unloaded(object sender, EventArgs e)
		{
				if (BindingContext is AttendanceViewModel vm)
				{
						//vm.Dispose();
						//BindingContext = null;
				}
		}
}

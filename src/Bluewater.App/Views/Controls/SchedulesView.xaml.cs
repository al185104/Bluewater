using Bluewater.App.ViewModels;
using Bluewater.App.ViewModels.Content;

namespace Bluewater.App.Views.Controls;

public partial class SchedulesView : ContentView
{
		public SchedulesView(ScheduleViewModel vm)
		{
				InitializeComponent();
				BindingContext = vm;
		}

		private async void ContentView_Loaded(object sender, EventArgs e)
		{
				if (BindingContext is ScheduleViewModel vm)
				{
						await vm.InitializeAsync();
				}
		}

		private void ContentView_Unloaded(object sender, EventArgs e)
		{
				if (BindingContext is ScheduleViewModel vm)
				{
						vm.Dispose();
						BindingContext = null;
				}
		}
}

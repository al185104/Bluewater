using Bluewater.App.ViewModels;

namespace Bluewater.App.Views.Controls;

public partial class LeavesView : ContentView
{
		public LeavesView(LeaveViewModel vm)
		{
				InitializeComponent();
				BindingContext = vm;
		}

		private void ContentView_Loaded(object sender, EventArgs e)
		{
				if(BindingContext is LeaveViewModel vm)
				{
						_ = vm.InitializeAsync();
				}
		}

		private void ContentView_Unloaded(object sender, EventArgs e)
		{

		}
}

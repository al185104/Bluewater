using Bluewater.App.ViewModels;

namespace Bluewater.App.Views.Controls;

public partial class SettingsView : ContentView
{
		public SettingsView(SettingViewModel vm)
		{
				InitializeComponent();
				BindingContext = vm;
		}

		private async void ContentView_Loaded(object sender, EventArgs e)
		{
				if (BindingContext is SettingViewModel vm)
				{
						await vm.InitializeAsync();
				}
		}

		private void ContentView_Unloaded(object sender, EventArgs e)
		{
				if (BindingContext is SettingViewModel vm)
				{
						//vm.Dispose();
						BindingContext = null;
				}
		}
}

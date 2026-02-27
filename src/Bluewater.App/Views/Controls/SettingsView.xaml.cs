using Bluewater.App.ViewModels;
using Bluewater.App.ViewModels.Content;

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
				if (BindingContext is EmployeeContentViewModel vm)
				{
						await vm.InitializeAsync();
				}
		}
}

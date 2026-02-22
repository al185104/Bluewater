using Bluewater.App.ViewModels;

namespace Bluewater.App.Views.Controls;

public partial class SettingsView : ContentView
{
		public SettingsView(SettingViewModel vm)
		{
				InitializeComponent();
				BindingContext = vm;
		}
}

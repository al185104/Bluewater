using Bluewater.App.ViewModels.Content;

namespace Bluewater.App.Views.Controls;

public partial class EmployeesView : ContentView
{
		public EmployeesView(EmployeeContentViewModel vm)
		{
				InitializeComponent();
				BindingContext = vm;
		}

		private async void ContentView_Loaded(object sender, EventArgs e)
		{
				if(BindingContext is EmployeeContentViewModel vm)
				{
						await vm.InitializeAsync();
				}
		}

		private void ContentView_Unloaded(object sender, EventArgs e)
		{
				//if(BindingContext is EmployeeContentViewModel vm)
				//{
				//		vm.Dispose();
				//		BindingContext = null;
				//}
		}
}

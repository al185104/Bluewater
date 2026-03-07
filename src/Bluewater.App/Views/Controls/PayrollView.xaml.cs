using Bluewater.App.ViewModels;

namespace Bluewater.App.Views.Controls;

public partial class PayrollView : ContentView
{
		public PayrollView(PayrollViewModel vm)
		{
				InitializeComponent();
				BindingContext = vm;
		}
}

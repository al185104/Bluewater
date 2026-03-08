using Bluewater.App.ViewModels.Modals;

namespace Bluewater.App.Views.Modals;

public partial class PayrollDetailsPage : ContentPage
{
		public PayrollDetailsPage(PayrollDetailsViewModel vm)
		{
				InitializeComponent();
				BindingContext = vm;
		}
}

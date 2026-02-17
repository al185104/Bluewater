using Bluewater.App.ViewModels.Modals;

namespace Bluewater.App.Views.Modals;

public partial class EmployeeDetailsPage : ContentPage
{
		private readonly EmployeeDetailsViewModel _vm;

		public EmployeeDetailsPage(EmployeeDetailsViewModel vm)
		{
				InitializeComponent();
				BindingContext = _vm = vm;
		}
}

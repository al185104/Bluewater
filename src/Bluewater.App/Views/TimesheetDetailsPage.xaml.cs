using Bluewater.App.ViewModels.Modals;

namespace Bluewater.App.Views;

public partial class TimesheetDetailsPage : ContentPage
{
		public TimesheetDetailsPage(TimesheetDetailsViewModel vm)
		{
				InitializeComponent();
				BindingContext = vm;
		}
}

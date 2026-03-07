using Bluewater.App.ViewModels;

namespace Bluewater.App.Views.Controls;

public partial class AttendanceView : ContentView
{
		public AttendanceView(AttendanceViewModel vm)
		{
				InitializeComponent();
				BindingContext = vm;
		}
}

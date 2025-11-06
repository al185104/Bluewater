using Bluewater.App.Models;

namespace Bluewater.App.Views;

[QueryProperty(nameof(Summary), nameof(Summary))]
public partial class AttendanceDetailPage : ContentPage
{
  private EmployeeAttendanceSummary? summary;

  public AttendanceDetailPage()
  {
    InitializeComponent();
  }

  public EmployeeAttendanceSummary? Summary
  {
    get => summary;
    set
    {
      summary = value;
      BindingContext = summary;
      Title = summary is null || string.IsNullOrWhiteSpace(summary.Name)
        ? "Attendance Details"
        : $"{summary.Name}'s Attendances";
    }
  }
}

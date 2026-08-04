using CommunityToolkit.Maui.Views;

namespace Bluewater.App.Views;

public partial class WhatsNewPopup : Popup
{
  public WhatsNewPopup()
  {
    InitializeComponent();
    BindingContext = this;
  }

  public IReadOnlyList<WhatsNewItem> Updates { get; } =
  [
    new(1, "Department-wide schedule imports", "Schedule imports now support multiple employees across the selected department instead of limiting updates to one charging.", "2026-06-27", "fec0646"),
    new(2, "Logout support", "A Logout action is now available from the home screen and returns the app to the login page after clearing the current session.", "2026-06-27", "fec0646"),
    new(3, "Profile page", "Employees can now open a Profile page with linked employee details, leave balances, and filed forms.", "2026-06-27", "fec0646"),
    new(4, "Authorization-aware navigation", "Home navigation now reflects the signed-in user's authorization so each role sees the sections available to them.", "2026-06-27", "fec0646"),
    new(5, "Free-day shift support", "All-midnight non-rest shifts now grant 8 work hours and are excluded from payroll absences.", "2026-06-06", "b9b3a4b"),
    new(6, "Manual search for payroll, schedules, and timesheets", "Those pages now load their CollectionView data only when Search is clicked, reducing automatic backend calls.", "2026-06-06", "b9b3a4b"),
    new(7, "Schedule import fixes", "Schedule CSV import handling was refined for matrix imports and schedule view updates.", "2026-05-31", "8d72503"),
    new(8, "Payroll period refinements", "Payroll calculation and list handling were updated, including stronger coverage for attendance-based payroll values.", "2026-05-30", "59536f1"),
    new(9, "Approved leave undertime handling", "Approved leave shifts are excluded from undertime totals to keep summaries aligned with leave status.", "2026-05-12", "de44654"),
    new(10, "Service charge upload in payroll", "Payroll gained support for uploading service charge data for the selected pay period.", "2026-05-12", "dc6ea3a")
  ];

  private async void OnCloseClicked(object sender, EventArgs e)
  {
    await CloseAsync();
  }
}

public sealed record WhatsNewItem(
  int Number,
  string Title,
  string Description,
  string Date,
  string Commit);

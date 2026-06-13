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
    new(1, "Free-day shift support", "All-midnight non-rest shifts now grant 8 work hours and are excluded from payroll absences.", "2026-06-06", "b9b3a4b"),
    new(2, "Manual search for payroll, schedules, and timesheets", "Those pages now load their CollectionView data only when Search is clicked, reducing automatic backend calls.", "2026-06-06", "b9b3a4b"),
    new(3, "Schedule import fixes", "Schedule CSV import handling was refined for matrix imports and schedule view updates.", "2026-05-31", "8d72503"),
    new(4, "Payroll period refinements", "Payroll calculation and list handling were updated, including stronger coverage for attendance-based payroll values.", "2026-05-30", "59536f1"),
    new(5, "Approved leave undertime handling", "Approved leave shifts are excluded from undertime totals to keep summaries aligned with leave status.", "2026-05-12", "de44654"),
    new(6, "Service charge upload in payroll", "Payroll gained support for uploading service charge data for the selected pay period.", "2026-05-12", "dc6ea3a"),
    new(7, "Service charge CSV compatibility", "Service charge imports now support a simple Barcode,Amount CSV format.", "2026-05-12", "a751f4d"),
    new(8, "Payroll upload groundwork", "Initial payroll service charge upload work was added across the payroll view and supporting logic.", "2026-05-12", "22ca68c"),
    new(9, "Timesheet details editing updates", "Timesheet detail editing logic was adjusted to support smoother edit flows.", "2026-05-03", "541a122"),
    new(10, "Editable timesheet record creation", "Timesheet editing now better handles creating editable records when the expected entry does not already exist.", "2026-05-02", "7731142")
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

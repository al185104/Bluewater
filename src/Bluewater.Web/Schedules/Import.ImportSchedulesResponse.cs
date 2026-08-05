namespace Bluewater.Web.Schedules;

public class ImportSchedulesResponse
{
  public int Attempted { get; set; }
  public int Created { get; set; }
  public int Updated { get; set; }
  public int Deleted { get; set; }
  public int SkippedPayrollLocked { get; set; }
  public int SkippedUnchanged { get; set; }
  public int SkippedInvalid { get; set; }

  public int Persisted => Created + Updated + Deleted;
}

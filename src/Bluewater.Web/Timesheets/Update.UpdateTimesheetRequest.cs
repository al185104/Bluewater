using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Timesheets;

public class UpdateTimesheetRequest
{
  public const string Route = "/Timesheets";

  [Required]
  public Guid Id { get; set; }

  [Required]
  public Guid EmployeeId { get; set; }

  public DateTime? TimeIn1 { get; set; }

  public DateTime? TimeOut1 { get; set; }

  public DateTime? TimeIn2 { get; set; }

  public DateTime? TimeOut2 { get; set; }

  public DateOnly? EntryDate { get; set; }

  public bool IsLocked { get; set; }
}

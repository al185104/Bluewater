using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Timesheets;
public class CreateTimesheetRequest
{
  public const string Route = "/Timesheets";

  [Required]
  public string username { get; set; } = null!;
  //public Guid employeeId { get; set; }
  public DateTime? timeInput { get; set; }
  public DateOnly? entryDate { get; set; }
  public int inputType { get; set; }
}

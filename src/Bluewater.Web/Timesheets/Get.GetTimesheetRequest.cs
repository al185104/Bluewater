using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Timesheets;

public class GetTimesheetRequest
{
  public const string Route = "/Timesheets/{TimesheetId:guid}";

  [Required]
  public Guid TimesheetId { get; set; }
}

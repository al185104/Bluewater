using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Timesheets;

public class TimesheetListRequest
{
  [Range(0, int.MaxValue)]
  public int? Skip { get; set; }

  [Range(1, int.MaxValue)]
  public int? Take { get; set; }

  [Required]
  public string Name { get; set; } = string.Empty;

  [Required]
  public DateOnly StartDate { get; set; }

  [Required]
  public DateOnly EndDate { get; set; }
}

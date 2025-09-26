using System.ComponentModel.DataAnnotations;
using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.Web.Timesheets;

public class TimesheetListAllRequest
{
  [Range(0, int.MaxValue)]
  public int? Skip { get; set; }

  [Range(1, int.MaxValue)]
  public int? Take { get; set; }

  [Required]
  public DateOnly StartDate { get; set; }

  [Required]
  public DateOnly EndDate { get; set; }

  [Required]
  public Tenant Tenant { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Schedules;

public class CreateScheduleRequest
{
  public const string Route = "/Schedules";

  [Required]
  public Guid EmployeeId { get; set; }

  [Required]
  public Guid ShiftId { get; set; }

  [Required]
  public DateOnly ScheduleDate { get; set; }

  public bool IsDefault { get; set; }
}

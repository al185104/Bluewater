using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Schedules;

public class UpdateScheduleRequest
{
  public const string Route = "/Schedules/{ScheduleId:guid}";
  public static string BuildRoute(Guid scheduleId) => Route.Replace("{ScheduleId:guid}", scheduleId.ToString());

  [Required]
  public Guid ScheduleId { get; set; }

  [Required]
  public Guid EmployeeId { get; set; }

  [Required]
  public Guid ShiftId { get; set; }

  [Required]
  public DateOnly ScheduleDate { get; set; }

  public bool IsDefault { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Shifts;

public class CreateShiftRequest
{
  public const string Route = "/Shifts";

  [Required]
  public string? Name { get; set; }
  public TimeOnly? ShiftStartTime { get; set; }
  public TimeOnly? ShiftBreakTime { get; set; }
  public TimeOnly? ShiftBreakEndTime { get; set; }
  public TimeOnly? ShiftEndTime { get; set; }
  public decimal BreakHours { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Shifts;

public class UpdateShiftRequest
{
  public const string Route = "/Shifts/{ShiftId:guid}";
  public static string BuildRoute(Guid shiftId) => Route.Replace("{ShiftId:guid}", shiftId.ToString());

  public Guid ShiftId { get; set; }

  [Required]
  public Guid Id { get; set; }

  [Required]
  public string? Name { get; set; }
  public TimeOnly? ShiftStartTime { get; set; }
  public TimeOnly? ShiftBreakTime { get; set; }
  public TimeOnly? ShiftBreakEndTime { get; set; }
  public TimeOnly? ShiftEndTime { get; set; }
  public decimal BreakHours { get; set; }
}

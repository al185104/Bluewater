namespace Bluewater.UseCases.Shifts;
public record ShiftImportDTO()
{
  public string Name { get; set; } = null!;
  public string? ShiftStartTime { get; set; }
  public string? ShiftBreakTime { get; set; }
  public string? ShiftBreakEndTime { get; set; }
  public string? ShiftEndTime { get; set; }
}

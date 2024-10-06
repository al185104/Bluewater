namespace Bluewater.UseCases.Shifts;
public record ShiftDTO()
{
  public Guid Id { get; init; }
  public string Name { get; set; } = null!;
  public TimeOnly? ShiftStartTime { get; set; }
  public TimeOnly? ShiftBreakTime { get; set; }
  public TimeOnly? ShiftBreakEndTime { get; set; }
  public TimeOnly? ShiftEndTime { get; set; }
  public decimal BreakHours { get; set; }

  public ShiftDTO(Guid id, string name, TimeOnly? start, TimeOnly? breakStart, TimeOnly? breakEnd, TimeOnly? end, decimal breakHours) : this()
  {
    Id = id;
    Name = name;
    ShiftStartTime = start;
    ShiftBreakTime = breakStart;
    ShiftBreakEndTime = breakEnd;
    ShiftEndTime = end;
    BreakHours = breakHours;
  }
}

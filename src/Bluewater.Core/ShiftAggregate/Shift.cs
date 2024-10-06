using Ardalis.SharedKernel;

namespace Bluewater.Core.ShiftAggregate;
public class Shift(string name, TimeOnly? start, TimeOnly? breakStart, TimeOnly? breakEnd, TimeOnly? end, decimal? breakHours) : EntityBase<Guid>, IAggregateRoot
{
    public string Name { get; private set; } = name;
    public TimeOnly? ShiftStartTime { get; private set; } = start;
    public TimeOnly? ShiftBreakTime { get; private set; } = breakStart;
    public TimeOnly? ShiftBreakEndTime { get; private set; } = breakEnd;
    public TimeOnly? ShiftEndTime { get; private set; } = end;
    public decimal BreakHours { get; private set; } = breakHours ?? 1;

    public DateTime CreatedDate { get; private set; } = DateTime.Now;
    public Guid CreateBy { get; private set; } = Guid.Empty;
    public DateTime UpdatedDate { get; private set; } = DateTime.Now;
    public Guid UpdateBy { get; private set; } = Guid.Empty;

    public Shift() : this(string.Empty, null, null, null, null, null) {}

    public void UpdateShift(string name, TimeOnly? start, TimeOnly? breakstart, TimeOnly? breakend, TimeOnly? end, decimal breakhours)
    {
      Name = name;
      ShiftStartTime = start;
      ShiftBreakTime = breakstart;
      ShiftBreakEndTime = breakend;
      ShiftEndTime = end;
      BreakHours = breakhours;
    }
}

using Bluewater.Core.ShiftAggregate;
using Bluewater.UseCases.Shifts;

namespace Bluewater.UseCases.Schedules;
public record ScheduleDTO()
{
  public Guid Id { get; init; }
    public Guid EmployeeId { get; private set; }
    public string Name { get; private set; } = null!;
    public Guid ShiftId { get; private set; }
    public DateOnly ScheduleDate { get; private set; }
    public bool IsDefault { get; private set; }

    // virtual properties
    public ShiftDTO? Shift { get; private set; }

  public ScheduleDTO(Guid id, string name, Guid employeeId, Guid shiftId, DateOnly scheduleDate, bool isDefault, ShiftDTO? shift = null) : this()
  {
    Id = id;
    EmployeeId = employeeId;
    Name = name;
    ShiftId = shiftId;
    ScheduleDate = scheduleDate;
    IsDefault = isDefault;
    Shift = shift;
  }
}

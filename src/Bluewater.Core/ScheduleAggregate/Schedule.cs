using System.Runtime.CompilerServices;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.ShiftAggregate;

namespace Bluewater.Core.ScheduleAggregate;
public class Schedule(Guid employeeId, Guid shiftId, DateOnly scheduleDate, bool isDefault) : EntityBase<Guid>, IAggregateRoot
{
    // foreign keys
    public Guid EmployeeId { get; private set; } = employeeId;
    public Guid ShiftId { get; private set; } = shiftId;
    public DateOnly ScheduleDate { get; private set; } = scheduleDate;
    public bool IsDefault { get; private set; } = isDefault;
    
    public DateTime CreatedDate { get; private set; } = DateTime.Now;
    public Guid CreateBy { get; private set; } = Guid.Empty;
    public DateTime UpdatedDate { get; private set; } = DateTime.Now;
    public Guid UpdateBy { get; private set; } = Guid.Empty;

    // virtual properties
    public virtual Employee Employee { get; private set; } = null!;
    public virtual Shift Shift { get; private set; } = null!;
    public Schedule() : this(Guid.Empty, Guid.Empty, DateOnly.MinValue, false) {}

    public void UpdateSchedule(Guid employeeId, Guid shiftId, DateOnly scheduleDate, bool isDefault)
    {
      EmployeeId = employeeId;
      ShiftId = shiftId;
      ScheduleDate = scheduleDate;
      IsDefault = isDefault;
      UpdatedDate = DateTime.Now;
    }
}

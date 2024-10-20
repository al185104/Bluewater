using Ardalis.Specification;

namespace Bluewater.Core.ScheduleAggregate.Specifications;
public class DefaultShiftByEmpIdSpec : Specification<Schedule>
{
  public DefaultShiftByEmpIdSpec(Guid empID, DayOfWeek? dayOfWeek = null)
  {
    if(dayOfWeek.HasValue)
      Query
        .Where(Schedule => Schedule.EmployeeId == empID && Schedule.IsDefault && Schedule.ScheduleDate.DayOfWeek == dayOfWeek)
        .Include(Schedule => Schedule.Shift);
    else
      Query
          .Where(Schedule => Schedule.EmployeeId == empID && Schedule.IsDefault)
          .Include(Schedule => Schedule.Shift);
  }
}

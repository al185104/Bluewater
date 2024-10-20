using Ardalis.Specification;

namespace Bluewater.Core.ScheduleAggregate.Specifications;
public class ScheduleByIdSpec : Specification<Schedule>
{
  public ScheduleByIdSpec(Guid empID, DateOnly startDate, DateOnly endDate)
  {
    Query
        .Where(Schedule => Schedule.EmployeeId == empID && Schedule.ScheduleDate >= startDate && Schedule.ScheduleDate <= endDate)
        .Include(Schedule => Schedule.Shift);
  }
}

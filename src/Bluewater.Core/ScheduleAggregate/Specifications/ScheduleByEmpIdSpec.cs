using Ardalis.Specification;

namespace Bluewater.Core.ScheduleAggregate.Specifications;
public class ScheduleByEmpIdSpec : Specification<Schedule>
{
  public ScheduleByEmpIdSpec(Guid empID, DateOnly startDate, DateOnly endDate)
  {
    Query
        .Where(Schedule => Schedule.EmployeeId == empID && Schedule.ScheduleDate >= startDate && Schedule.ScheduleDate <= endDate)
        .Include(Schedule => Schedule.Shift)
        .OrderByDescending(Schedule => Schedule.UpdatedDate);
  }
}

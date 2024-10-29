using Ardalis.Specification;

namespace Bluewater.Core.ScheduleAggregate.Specifications;
public class ScheduleByEmpIdAndDateSpec : Specification<Schedule>
{
  public ScheduleByEmpIdAndDateSpec(Guid empID, DateOnly entryDate)
  {
    Query
        .Where(Schedule => Schedule.EmployeeId == empID && Schedule.ScheduleDate == entryDate)
        .Include(Schedule => Schedule.Shift);
  }
}

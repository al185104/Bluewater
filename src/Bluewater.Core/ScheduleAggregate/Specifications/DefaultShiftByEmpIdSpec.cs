using Ardalis.Specification;

namespace Bluewater.Core.ScheduleAggregate.Specifications;
public class DefaultShiftByEmpIdSpec : Specification<Schedule>
{
  public DefaultShiftByEmpIdSpec(Guid empID)
  {
      Query
          .Where(Schedule => Schedule.EmployeeId == empID && Schedule.IsDefault)
          .Include(Schedule => Schedule.Shift);
  }
}

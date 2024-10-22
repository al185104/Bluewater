using Ardalis.Specification;

namespace Bluewater.Core.ScheduleAggregate.Specifications;
public class ScheduleByIdSpec : Specification<Schedule>
{
  public ScheduleByIdSpec(Guid id)
  {
    Query
        .Where(Schedule => Schedule.Id == id)
        .Include(Schedule => Schedule.Shift);
  }
}

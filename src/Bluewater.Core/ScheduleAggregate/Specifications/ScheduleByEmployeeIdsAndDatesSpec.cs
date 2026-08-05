using Ardalis.Specification;

namespace Bluewater.Core.ScheduleAggregate.Specifications;

public sealed class ScheduleByEmployeeIdsAndDatesSpec : Specification<Schedule>
{
  public ScheduleByEmployeeIdsAndDatesSpec(IEnumerable<Guid> employeeIds, DateOnly startDate, DateOnly endDate)
  {
    List<Guid> ids = [.. employeeIds.Where(id => id != Guid.Empty).Distinct()];

    Query
      .Where(schedule =>
        ids.Contains(schedule.EmployeeId) &&
        schedule.ScheduleDate >= startDate &&
        schedule.ScheduleDate <= endDate)
      .Include(schedule => schedule.Shift);
  }
}

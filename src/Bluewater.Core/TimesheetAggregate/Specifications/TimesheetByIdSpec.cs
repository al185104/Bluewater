using Ardalis.Specification;

namespace Bluewater.Core.TimesheetAggregate.Specifications;
public class TimesheetByIdSpec : Specification<Timesheet>
{
  public TimesheetByIdSpec(Guid id)
  {
    Query
        .Where(Timesheet => Timesheet.Id == id)
        .Include(Timesheet => Timesheet.Employee);
  }
}

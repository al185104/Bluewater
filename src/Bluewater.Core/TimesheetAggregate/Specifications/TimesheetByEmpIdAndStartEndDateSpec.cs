using Ardalis.Specification;

namespace Bluewater.Core.TimesheetAggregate.Specifications;
public class TimesheetByEmpIdAndStartEndDateSpec : Specification<Timesheet>
{
  public TimesheetByEmpIdAndStartEndDateSpec(Guid id, DateOnly startDate, DateOnly endDate)
  {
    Query
        .Where(Timesheet => Timesheet.EmployeeId == id && Timesheet.EntryDate >= startDate && Timesheet.EntryDate <= endDate);
  } 
}

using Ardalis.Specification;

namespace Bluewater.Core.TimesheetAggregate.Specifications;
public class TimesheetByEmpIdAndEntryDate : Specification<Timesheet>
{
  public TimesheetByEmpIdAndEntryDate(Guid empId, DateOnly entryDate)
  {
    Query
        .Where(Timesheet => Timesheet.EmployeeId == empId && Timesheet.EntryDate == entryDate)
        .Include(Timesheet => Timesheet.Employee);
  }
}

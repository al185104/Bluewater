using Ardalis.Specification;
using Bluewater.Core.AttendanceAggregate;
using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.Core.AttendanceAggregate.Specifications;

public class AttendanceByDateRangeSpec : Specification<Attendance>
{
  public AttendanceByDateRangeSpec(
    DateOnly startDate,
    DateOnly endDate,
    Tenant tenant,
    bool includeRelated = true)
  {
    Query
      .AsNoTracking()
      .Where(attendance =>
        attendance.EntryDate.HasValue &&
        attendance.EntryDate.Value >= startDate &&
        attendance.EntryDate.Value <= endDate);

    if (includeRelated)
    {
      Query
        .Include(attendance => attendance.Employee)
        .Include(attendance => attendance.Shift)
        .Include(attendance => attendance.Timesheet);
    }

    Query.Where(attendance => attendance.Employee != null && attendance.Employee.Tenant == tenant);
  }
}

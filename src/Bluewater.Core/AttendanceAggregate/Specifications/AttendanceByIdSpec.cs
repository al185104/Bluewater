using Ardalis.Specification;

namespace Bluewater.Core.AttendanceAggregate.Specifications;
public class AttendanceByIdSpec : Specification<Attendance>
{
  public AttendanceByIdSpec(Guid id)
  {
    Query
        .Where(Attendance => Attendance.Id == id)
        .Include(Attendance => Attendance.Employee)
        .Include(Attendance => Attendance.Timesheet)
        .Include(Attendance => Attendance.Shift);
  }
}

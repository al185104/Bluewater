using Ardalis.Specification;

namespace Bluewater.Core.AttendanceAggregate.Specifications;
public class AttendanceByEmpIdAndDatesSpec : Specification<Attendance>
{
  public AttendanceByEmpIdAndDatesSpec(Guid empID, DateOnly startDate, DateOnly endDate)
  {
    Query
        .Where(Attendance => Attendance.EmployeeId == empID && Attendance.EntryDate >= startDate && Attendance.EntryDate <= endDate)
        .Include(Attendance => Attendance.Employee)
        .Include(Attendance => Attendance.Timesheet)
        .Include(Attendance => Attendance.Shift)
        .OrderByDescending(Attendance => Attendance.UpdatedDate);
  }
}

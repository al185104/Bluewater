using Ardalis.Specification;

namespace Bluewater.Core.AttendanceAggregate.Specifications;
public class AttendanceByEmpIdAndDateSpec : Specification<Attendance>
{
  public AttendanceByEmpIdAndDateSpec(Guid empID, DateOnly? entryDate)
  {
    Query
        .Where(Attendance => Attendance.EmployeeId == empID && Attendance.EntryDate == entryDate)
        .Include(Attendance => Attendance.Employee)
        .Include(Attendance => Attendance.Timesheet)
        .Include(Attendance => Attendance.Shift)
        .OrderByDescending(Attendance => Attendance.UpdatedDate);
  }
}

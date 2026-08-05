using Ardalis.Specification;
using Bluewater.Core.Forms.Enum;

namespace Bluewater.Core.Forms.LeaveAggregate.Specifications;

public sealed class LeavesByEmployeeLeaveCreditAndDateRangeSpec : Specification<Leave>
{
  public LeavesByEmployeeLeaveCreditAndDateRangeSpec(
    Guid employeeId,
    Guid leaveCreditId,
    DateTime startDate,
    DateTime endDate)
  {
    DateTime rangeStart = startDate.Date;
    DateTime rangeEnd = endDate.Date;

    Query
      .AsNoTracking()
      .Where(leave =>
        leave.EmployeeId == employeeId &&
        leave.LeaveCreditId == leaveCreditId &&
        leave.Status != ApplicationStatus.Rejected &&
        leave.StartDate.Date <= rangeEnd &&
        leave.EndDate.Date >= rangeStart);
  }
}

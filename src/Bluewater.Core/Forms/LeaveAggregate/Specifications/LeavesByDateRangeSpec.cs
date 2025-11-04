using Ardalis.Specification;
using Bluewater.Core.EmployeeAggregate.Enum;
using Bluewater.Core.Forms.Enum;
using Bluewater.Core.Forms.LeaveAggregate;

namespace Bluewater.Core.Forms.LeaveAggregate.Specifications;

public class LeavesByDateRangeSpec : Specification<Leave>
{
  public LeavesByDateRangeSpec(
    DateTime startDate,
    DateTime endDate,
    Tenant tenant,
    ApplicationStatus status = ApplicationStatus.Approved)
  {
    DateTime rangeStart = startDate.Date;
    DateTime rangeEnd = endDate.Date;

    Query
      .AsNoTracking()
      .Where(leave =>
        leave.StartDate.Date <= rangeEnd &&
        leave.EndDate.Date >= rangeStart)
      .Where(leave => leave.Status == status)
      .Include(leave => leave.LeaveCredit)
      .Include(leave => leave.Employee)
      .Where(leave => leave.Employee != null && leave.Employee.Tenant == tenant);
  }
}

using Ardalis.Specification;
using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.Core.Forms.LeaveAggregate.Specifications;
public class LeaveAllSpec : Specification<Leave>
{
    public LeaveAllSpec(Tenant tenant)
    {
    Query
      .AsNoTracking()
      .Where(leave => leave.CreatedDate >= DateTime.Now.AddDays(-30))
      .Include(leave => leave.Employee).Where(i => i.Employee != null && i.Employee.Tenant == tenant)
      .Include(leave => leave.LeaveCredit);
  }
}

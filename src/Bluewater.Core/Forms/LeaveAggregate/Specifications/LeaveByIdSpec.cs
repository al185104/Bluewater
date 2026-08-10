using Ardalis.Specification;

namespace Bluewater.Core.Forms.LeaveAggregate.Specifications;

public sealed class LeaveByIdSpec : Specification<Leave>
{
  public LeaveByIdSpec(Guid leaveId)
  {
    Query
      .Where(leave => leave.Id == leaveId)
      .Include(leave => leave.Employee)
      .Include(leave => leave.LeaveCredit);
  }
}

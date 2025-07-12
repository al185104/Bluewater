using Ardalis.Specification;
using Bluewater.Core.EmployeeAggregate.Enum;
using Bluewater.Core.Forms.FailureInOutAggregate;

namespace Bluewater.Core.FailureInOutAggregate.Specifications;
public class FailureInOutAllSpec : Specification<FailureInOut>
{
  public FailureInOutAllSpec(Tenant tenant)
  {
    Query.Include(FailureInOut => FailureInOut.Employee).Where(i => i.Employee != null && i.Employee.Tenant == tenant);
  }
}

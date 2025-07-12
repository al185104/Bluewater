using Ardalis.Specification;
using Bluewater.Core.EmployeeAggregate.Enum;
using Bluewater.Core.Forms.OtherEarningAggregate;

namespace Bluewater.Core.OtherEarningAggregate.Specifications;
public class OtherEarningAllSpec : Specification<OtherEarning>
{
  public OtherEarningAllSpec(Tenant tenant)
  {
    Query.Include(OtherEarning => OtherEarning.Employee).Where(i => i.Employee != null && i.Employee.Tenant == tenant);
  }
}

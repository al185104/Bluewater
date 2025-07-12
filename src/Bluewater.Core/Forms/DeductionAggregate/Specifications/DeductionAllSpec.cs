using Ardalis.Specification;
using Bluewater.Core.EmployeeAggregate.Enum;
using Bluewater.Core.Forms.DeductionAggregate;

namespace Bluewater.Core.DeductionAggregate.Specifications;
public class DeductionAllSpec : Specification<Deduction>
{
  public DeductionAllSpec(Tenant tenant)
  {
    Query.
      Include(Deduction => Deduction.Employee).Where(i => i.Employee != null && i.Employee.Tenant == tenant);
  }
}

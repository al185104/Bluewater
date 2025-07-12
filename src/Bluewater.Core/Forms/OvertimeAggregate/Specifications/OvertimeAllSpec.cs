using Ardalis.Specification;
using Bluewater.Core.EmployeeAggregate.Enum;
using Bluewater.Core.Forms.OvertimeAggregate;

namespace Bluewater.Core.OvertimeAggregate.Specifications;
public class OvertimeAllSpec : Specification<Overtime>
{
  public OvertimeAllSpec(Tenant tenant)
  {
    Query.Include(Overtime => Overtime.Employee).Where(i => i.Employee != null && i.Employee.Tenant == tenant);
  }
}

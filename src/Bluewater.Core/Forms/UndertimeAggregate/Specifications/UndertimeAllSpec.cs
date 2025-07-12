using Ardalis.Specification;
using Bluewater.Core.EmployeeAggregate.Enum;
using Bluewater.Core.Forms.UndertimeAggregate;

namespace Bluewater.Core.UndertimeAggregate.Specifications;
public class UndertimeAllSpec : Specification<Undertime>
{
  public UndertimeAllSpec(Tenant tenant)
  {
    Query.Include(Undertime => Undertime.Employee).Where(i => i.Employee != null && i.Employee.Tenant == tenant);
  }
}

using Ardalis.Specification;
using Bluewater.Core.EmployeeAggregate.Enum;
using Microsoft.EntityFrameworkCore;

namespace Bluewater.Core.EmployeeAggregate.Specifications;

public class EmployeeCountSpec : Specification<Employee>
{
  public EmployeeCountSpec(Tenant tenant)
  {
    Query
      .AsNoTracking()
      .Where(employee => employee.Tenant == tenant);
  }
}

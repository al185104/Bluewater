using Ardalis.Specification;
using Bluewater.Core.EmployeeAggregate.Enum;
using Microsoft.EntityFrameworkCore;

namespace Bluewater.Core.EmployeeAggregate.Specifications;

public class EmployeeCountSpecByCharging : Specification<Employee>
{
  public EmployeeCountSpecByCharging(string chargingName, Tenant tenant)
  {
    Query
      .AsNoTracking()
      .Include(employee => employee.Charging)
      .Where(employee => employee.Charging != null && employee.Charging.Name == chargingName && employee.Tenant == tenant);
  }
}

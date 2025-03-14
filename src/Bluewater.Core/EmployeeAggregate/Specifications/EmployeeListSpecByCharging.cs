using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;

namespace Bluewater.Core.EmployeeAggregate.Specifications;

public class EmployeeListSpecByCharging : Specification<Employee>
{
  public EmployeeListSpecByCharging(int? skip, int? take, string chargingName)
  {
    Query
        .AsNoTracking()
        .Include(e => e.Charging)
        .Where(e => e.Charging != null && e.Charging.Name == chargingName)
        .OrderBy(e => e.LastName);

    if (skip.HasValue)
      Query.Skip(skip.Value);

    if (take.HasValue)
      Query.Take(take.Value);
  }
}

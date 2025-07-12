using Ardalis.Specification;
using Bluewater.Core.EmployeeAggregate.Enum;
using Microsoft.EntityFrameworkCore;

namespace Bluewater.Core.EmployeeAggregate.Specifications;

public class EmployeeListSpec :  Specification<Employee>
{
    public EmployeeListSpec(int? skip, int? take, Tenant tenant)
    {
        Query
            .AsNoTracking()
            .Where(i => i.Tenant == tenant)
            .OrderBy(e => e.LastName);

        if (skip.HasValue)
            Query.Skip(skip.Value);

        if (take.HasValue)
            Query.Take(take.Value);
    }
}

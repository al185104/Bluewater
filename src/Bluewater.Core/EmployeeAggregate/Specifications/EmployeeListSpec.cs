using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;

namespace Bluewater.Core.EmployeeAggregate.Specifications;

public class EmployeeListSpec :  Specification<Employee>
{
    public EmployeeListSpec(int? skip, int? take)
    {
        Query
            .AsNoTracking()
            .OrderBy(e => e.LastName);

        if (skip.HasValue)
            Query.Skip(skip.Value);

        if (take.HasValue)
            Query.Take(take.Value);
    }
}

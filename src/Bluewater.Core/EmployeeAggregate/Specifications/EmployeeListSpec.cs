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
            .Include(employee => employee.User)
            .Include(employee => employee.Pay)
            .Include(employee => employee.Type)
            .Include(employee => employee.Level)
            .Include(employee => employee.Charging)
                .ThenInclude(charging => charging!.Department)
            .Include(employee => employee.Position)
                .ThenInclude(position => position!.Section)
                    .ThenInclude(section => section!.Department)
                        .ThenInclude(department => department!.Division)
            .Where(i => i.Tenant == tenant && !i.IsDeleted)
            .OrderBy(e => e.LastName);

        if (skip.HasValue)
            Query.Skip(skip.Value);

        if (take.HasValue)
            Query.Take(take.Value);
    }
}

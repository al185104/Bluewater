using Ardalis.Specification;
using Bluewater.Core.EmployeeAggregate.Enum;
using Microsoft.EntityFrameworkCore;

namespace Bluewater.Core.EmployeeAggregate.Specifications;

public sealed class DeletedEmployeeByNameSpec : Specification<Employee>
{
  public DeletedEmployeeByNameSpec(string firstName, string lastName, Tenant tenant)
  {
    string normalizedFirstName = firstName.Trim();
    string normalizedLastName = lastName.Trim();

    Query.Where(employee =>
        employee.IsDeleted
        && employee.Tenant == tenant
        && EF.Functions.Like(employee.FirstName, normalizedFirstName)
        && EF.Functions.Like(employee.LastName, normalizedLastName));
  }
}

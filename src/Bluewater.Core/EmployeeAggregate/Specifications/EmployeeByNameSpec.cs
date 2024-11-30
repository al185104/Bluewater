using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;

namespace Bluewater.Core.EmployeeAggregate.Specifications;
public class EmployeeByNameSpec : Specification<Employee>
{
  public EmployeeByNameSpec(string name)
  {
    Query.Where(employee => 
        EF.Functions.Like(employee.LastName + ", " + employee.FirstName, $"%{name}%") ||
        EF.Functions.Like(employee.FirstName + " " + employee.LastName, $"%{name}%"))
        .Include(employee => employee.ContactInfo)
        .Include(employee => employee.User);
  }
}

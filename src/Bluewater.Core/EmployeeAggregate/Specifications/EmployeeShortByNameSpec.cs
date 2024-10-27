using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;

namespace Bluewater.Core.EmployeeAggregate.Specifications;
public class EmployeeShortByNameSpec : Specification<Employee>
{
  public EmployeeShortByNameSpec(string name)
  {
    Query.Where(employee => 
        EF.Functions.Like(employee.LastName + ", " + employee.FirstName, $"%{name}%") ||
        EF.Functions.Like(employee.FirstName + " " + employee.LastName, $"%{name}%")
    );
  }
}

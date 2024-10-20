using Ardalis.Specification;

namespace Bluewater.Core.EmployeeAggregate.Specifications;
public class EmployeeShortByNameSpec : Specification<Employee>
{
  public EmployeeShortByNameSpec(string Name)
  {
    Query
        .Where(Employee => $"{Employee.LastName}, {Employee.FirstName}".Contains(Name, StringComparison.InvariantCultureIgnoreCase));
  }
}

using Ardalis.Specification;

namespace Bluewater.Core.EmployeeAggregate.Specifications;
public class EmployeeByNameSpec : Specification<Employee>
{
  public EmployeeByNameSpec(string name)
  {
    Query
        .Where(Employee => $"{Employee.LastName}, {Employee.FirstName}" == name)
        .Include(Employee => Employee.User);
  }
}

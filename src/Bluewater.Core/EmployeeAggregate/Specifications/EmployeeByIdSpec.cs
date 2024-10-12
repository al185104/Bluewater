using Ardalis.Specification;

namespace Bluewater.Core.EmployeeAggregate.Specifications;
public class EmployeeByIdSpec : Specification<Employee>
{
  public EmployeeByIdSpec(Guid EmployeeId)
  {
    Query
        .Where(Employee => Employee.Id == EmployeeId);
  }
}

using Ardalis.Specification;

namespace Bluewater.Core.EmployeeAggregate.Specifications;

public class EmployeeByUserIdSpec : Specification<Employee>
{
  public EmployeeByUserIdSpec(Guid userId)
  {
    Query.Where(employee => employee.UserId == userId);
  }
}

using Ardalis.Specification;

namespace Bluewater.Core.DepartmentAggregate.Specifications;
public class DepartmentByIdSpec : Specification<Department>
{
  public DepartmentByIdSpec(Guid DepartmentId)
  {
    Query
        .Where(Department => Department.Id == DepartmentId);
  }
}

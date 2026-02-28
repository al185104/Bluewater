using Ardalis.Specification;

namespace Bluewater.Core.DepartmentAggregate.Specifications;

public class DepartmentByNameSpec : Specification<Department>
{
  public DepartmentByNameSpec(string name)
  {
    var normalizedName = name.Trim();

    Query.Where(department => department.Name.ToLower() == normalizedName.ToLower());
  }
}

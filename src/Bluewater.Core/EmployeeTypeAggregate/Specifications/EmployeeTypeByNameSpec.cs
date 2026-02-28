using Ardalis.Specification;

namespace Bluewater.Core.EmployeeTypeAggregate.Specifications;

public class EmployeeTypeByNameSpec : Specification<EmployeeType>
{
  public EmployeeTypeByNameSpec(string name)
  {
    var normalizedName = name.Trim();

    Query.Where(employeeType => employeeType.Name.ToLower() == normalizedName.ToLower());
  }
}

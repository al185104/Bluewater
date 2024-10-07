using Ardalis.Specification;

namespace Bluewater.Core.EmployeeTypeAggregate.Specifications;
public class EmployeeTypeByIdSpec : Specification<EmployeeType>
{
  public EmployeeTypeByIdSpec(Guid EmployeeTypeId)
  {
    Query
        .Where(EmployeeType => EmployeeType.Id == EmployeeTypeId);
  }
}

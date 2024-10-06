using Ardalis.Specification;

namespace Bluewater.Core.DivisionAggregate.Specifications;
public class DivisionByIdSpec : Specification<Division>
{
  public DivisionByIdSpec(Guid divisionId)
  {
    Query
        .Where(division => division.Id == divisionId);
  }
}

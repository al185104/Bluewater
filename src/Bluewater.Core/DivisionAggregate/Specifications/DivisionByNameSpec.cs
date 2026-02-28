using Ardalis.Specification;

namespace Bluewater.Core.DivisionAggregate.Specifications;

public class DivisionByNameSpec : Specification<Division>
{
  public DivisionByNameSpec(string name)
  {
    var normalizedName = name.Trim();

    Query.Where(division => division.Name.ToLower() == normalizedName.ToLower());
  }
}

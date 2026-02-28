using Ardalis.Specification;

namespace Bluewater.Core.PositionAggregate.Specifications;

public class PositionByNameSpec : Specification<Position>
{
  public PositionByNameSpec(string name)
  {
    var normalizedName = name.Trim();

    Query.Where(position => position.Name.ToLower() == normalizedName.ToLower());
  }
}

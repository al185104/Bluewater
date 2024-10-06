using Ardalis.Specification;

namespace Bluewater.Core.PositionAggregate.Specifications;
public class PositionByIdSpec : Specification<Position>
{
  public PositionByIdSpec(Guid PositionId)
  {
    Query
        .Where(Position => Position.Id == PositionId);
  }
}

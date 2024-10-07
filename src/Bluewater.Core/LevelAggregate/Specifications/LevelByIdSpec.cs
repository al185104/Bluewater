using Ardalis.Specification;

namespace Bluewater.Core.LevelAggregate.Specifications;
public class LevelByIdSpec : Specification<Level>
{
  public LevelByIdSpec(Guid LevelId)
  {
    Query
        .Where(Level => Level.Id == LevelId);
  }
}

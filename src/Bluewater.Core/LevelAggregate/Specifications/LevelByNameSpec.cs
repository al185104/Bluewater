using Ardalis.Specification;

namespace Bluewater.Core.LevelAggregate.Specifications;

public class LevelByNameSpec : Specification<Level>
{
  public LevelByNameSpec(string name)
  {
    var normalizedName = name.Trim();

    Query.Where(level => level.Name.ToLower() == normalizedName.ToLower());
  }
}

using Ardalis.Specification;

namespace Bluewater.Core.SectionAggregate.Specifications;

public class SectionByNameSpec : Specification<Section>
{
  public SectionByNameSpec(string name)
  {
    var normalizedName = name.Trim();

    Query.Where(section => section.Name.ToLower() == normalizedName.ToLower());
  }
}

using Ardalis.Specification;

namespace Bluewater.Core.SectionAggregate.Specifications;
public class SectionByIdSpec : Specification<Section>
{
  public SectionByIdSpec(Guid SectionId)
  {
    Query
        .Where(Section => Section.Id == SectionId);
  }
}

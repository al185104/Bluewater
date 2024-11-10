using Ardalis.Specification;
using Bluewater.Core.Forms.OtherEarningAggregate;

namespace Bluewater.Core.OtherEarningAggregate.Specifications;
public class OtherEarningByDatesSpec : Specification<OtherEarning>
{
  public OtherEarningByDatesSpec(DateOnly start, DateOnly end)
  {
    Query.Where(OtherEarning => OtherEarning.Date >= start && OtherEarning.Date <= end);
    Query.Include(OtherEarning => OtherEarning.Employee);
  }
}

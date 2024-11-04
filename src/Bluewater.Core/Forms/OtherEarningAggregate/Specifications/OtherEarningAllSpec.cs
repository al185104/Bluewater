using Ardalis.Specification;
using Bluewater.Core.Forms.OtherEarningAggregate;

namespace Bluewater.Core.OtherEarningAggregate.Specifications;
public class OtherEarningAllSpec : Specification<OtherEarning>
{
  public OtherEarningAllSpec()
  {
    Query.Include(OtherEarning => OtherEarning.Employee);
  }
}

using Ardalis.Specification;
using Bluewater.Core.Forms.FailureInOutAggregate;

namespace Bluewater.Core.FailureInOutAggregate.Specifications;
public class FailureInOutAllSpec : Specification<FailureInOut>
{
  public FailureInOutAllSpec()
  {
    Query.Include(FailureInOut => FailureInOut.Employee);
  }
}

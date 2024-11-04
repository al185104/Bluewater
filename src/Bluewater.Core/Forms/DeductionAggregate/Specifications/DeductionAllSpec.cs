using Ardalis.Specification;
using Bluewater.Core.Forms.DeductionAggregate;

namespace Bluewater.Core.DeductionAggregate.Specifications;
public class DeductionAllSpec : Specification<Deduction>
{
  public DeductionAllSpec()
  {
    Query.Include(Deduction => Deduction.Employee);
  }
}

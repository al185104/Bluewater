using Ardalis.Specification;
using Bluewater.Core.Forms.DeductionAggregate;

namespace Bluewater.Core.DeductionAggregate.Specifications;
public class DeductionAllSpec : Specification<Deduction>
{
  public DeductionAllSpec()
  {
    // get all including employee
    Query.Include(Deduction => Deduction.Employee);
  }
}

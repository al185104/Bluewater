using Ardalis.Specification;
using Bluewater.Core.Forms.DeductionAggregate;

namespace Bluewater.Core.DeductionAggregate.Specifications;
public class DeductionByIdSpec : Specification<Deduction>
{
  public DeductionByIdSpec(Guid DeductionId)
  {
    Query
        .Where(Deduction => Deduction.Id == DeductionId)
        .Include(Deduction => Deduction.Employee);
  }
}

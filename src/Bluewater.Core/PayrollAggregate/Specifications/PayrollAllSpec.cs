using Ardalis.Specification;

namespace Bluewater.Core.PayrollAggregate.Specifications;
public class PayrollAllSpec : Specification<Payroll, PayrollSummary>
{
  public PayrollAllSpec()
  {
    Query
        .Include(payroll => payroll.Employee);
  }
}

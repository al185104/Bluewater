using Ardalis.Specification;


namespace Bluewater.Core.PayrollAggregate.Specifications;
public class PayrollByIdSpec : Specification<Payroll>
{
  public PayrollByIdSpec(Guid PayrollId)
  {
    Query
        .Where(Payroll => Payroll.Id == PayrollId)
        .Include(Payroll => Payroll.Employee);
  }
}

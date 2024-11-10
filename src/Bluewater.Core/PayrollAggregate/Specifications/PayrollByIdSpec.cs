using Ardalis.Specification;


namespace Bluewater.Core.PayrollAggregate.Specifications;
public class PayrollByIdSpec : Specification<Payroll>
{
  public PayrollByIdSpec(Guid? empId, DateOnly end)
  {
    Query
        .Where(Payroll => Payroll.EmployeeId == empId && Payroll.Date == end)
        .Include(Payroll => Payroll.Employee);
  }
}

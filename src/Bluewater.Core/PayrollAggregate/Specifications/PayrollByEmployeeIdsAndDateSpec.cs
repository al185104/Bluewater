using Ardalis.Specification;

namespace Bluewater.Core.PayrollAggregate.Specifications;

public sealed class PayrollByEmployeeIdsAndDateSpec : Specification<Payroll>
{
  public PayrollByEmployeeIdsAndDateSpec(IEnumerable<Guid> employeeIds, DateOnly payrollDate)
  {
    Guid[] ids = employeeIds.Distinct().ToArray();

    Query
      .Where(payroll => ids.Contains(payroll.EmployeeId) && payroll.Date == payrollDate)
      .Include(payroll => payroll.Employee);
  }
}

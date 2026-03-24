using Ardalis.Specification;

namespace Bluewater.Core.PayrollAggregate.Specifications;

public sealed class PayrollByEmployeeIdsAndDateSpec : Specification<Payroll>
{
  public PayrollByEmployeeIdsAndDateSpec(IEnumerable<Guid> employeeIds, DateOnly payrollDate)
  {
    List<Guid> ids = [.. employeeIds.Distinct()];

    Query
    .Where(payroll =>
        payroll.EmployeeId.HasValue &&
        ids.Contains(payroll.EmployeeId.Value) &&
        payroll.Date == payrollDate)
    .Include(payroll => payroll.Employee);
  }
}

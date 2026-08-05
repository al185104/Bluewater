using Ardalis.Specification;

namespace Bluewater.Core.PayrollAggregate.Specifications;

public sealed class PayrollByEmployeeIdsAndDatesSpec : Specification<Payroll>
{
  public PayrollByEmployeeIdsAndDatesSpec(IEnumerable<Guid> employeeIds, IEnumerable<DateOnly> payrollDates)
  {
    List<Guid> ids = [.. employeeIds.Where(id => id != Guid.Empty).Distinct()];
    List<DateOnly> dates = [.. payrollDates.Distinct()];

    Query
      .Where(payroll =>
        payroll.EmployeeId.HasValue &&
        ids.Contains(payroll.EmployeeId.Value) &&
        dates.Contains(payroll.Date));
  }
}

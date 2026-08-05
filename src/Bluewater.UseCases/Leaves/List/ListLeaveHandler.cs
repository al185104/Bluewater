using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.LeaveAggregate;
using Bluewater.Core.Forms.LeaveAggregate.Specifications;
using Bluewater.Core.PayrollAggregate;
using Bluewater.Core.PayrollAggregate.Specifications;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Leaves.List;

public class ListLeaveHandler(IRepository<Leave> _repository, IRepository<Payroll> payrollRepository) : IQueryHandler<ListLeaveQuery, Result<IEnumerable<LeaveDTO>>>
{
  public async Task<Result<IEnumerable<LeaveDTO>>> Handle(ListLeaveQuery request, CancellationToken cancellationToken)
  {
    var spec = new LeaveAllSpec(request.tenant);
    var leaves = await _repository.ListAsync(spec, cancellationToken);
    if (leaves == null) return Result.NotFound();

    List<Guid> employeeIds = leaves
      .Where(leave => leave.EmployeeId.HasValue)
      .Select(leave => leave.EmployeeId!.Value)
      .Distinct()
      .ToList();

    List<DateOnly> payrollDates = leaves
      .SelectMany(GetPayrollPeriodEnds)
      .Distinct()
      .ToList();

    HashSet<(Guid EmployeeId, DateOnly PayrollDate)> lockedPayrolls = [];
    if (employeeIds.Count > 0 && payrollDates.Count > 0)
    {
      List<Payroll> payrolls = await payrollRepository.ListAsync(
        new PayrollByEmployeeIdsAndDatesSpec(employeeIds, payrollDates),
        cancellationToken);

      lockedPayrolls = payrolls
        .Where(payroll => payroll.EmployeeId.HasValue)
        .Select(payroll => (payroll.EmployeeId!.Value, payroll.Date))
        .ToHashSet();
    }

    var result = leaves.Select(s => new LeaveDTO(
      s.Id,
      s.StartDate,
      s.EndDate,
      s.IsHalfDay,
      (ApplicationStatusDTO)s.Status,
      s.EmployeeId ?? Guid.Empty,
      s.LeaveCreditId,
      $"{s.Employee?.LastName}, {s.Employee?.FirstName}",
      $"{s.LeaveCredit?.LeaveCode}",
      HasPayrollCreated(s, lockedPayrolls)));

    return Result.Success(result);
  }

  private static bool HasPayrollCreated(Leave leave, HashSet<(Guid EmployeeId, DateOnly PayrollDate)> lockedPayrolls)
  {
    if (!leave.EmployeeId.HasValue)
    {
      return false;
    }

    Guid employeeId = leave.EmployeeId.Value;
    return GetPayrollPeriodEnds(leave).Any(payrollDate => lockedPayrolls.Contains((employeeId, payrollDate)));
  }

  private static IEnumerable<DateOnly> GetPayrollPeriodEnds(Leave leave)
  {
    DateTime start = leave.StartDate.Date;
    DateTime end = leave.EndDate.Date;
    if (end < start)
    {
      yield break;
    }

    for (DateTime date = start; date <= end; date = date.AddDays(1))
    {
      yield return GetPayrollPeriodEnd(DateOnly.FromDateTime(date));
    }
  }

  private static DateOnly GetPayrollPeriodEnd(DateOnly date)
  {
    if (date.Day >= 11 && date.Day <= 25)
    {
      return new DateOnly(date.Year, date.Month, 25);
    }

    if (date.Day >= 26)
    {
      DateOnly nextMonth = date.AddMonths(1);
      return new DateOnly(nextMonth.Year, nextMonth.Month, 10);
    }

    return new DateOnly(date.Year, date.Month, 10);
  }
}

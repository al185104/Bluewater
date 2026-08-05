using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.Enum;
using Bluewater.Core.Forms.LeaveAggregate;
using Bluewater.Core.Forms.LeaveAggregate.Specifications;
using Bluewater.Core.PayrollAggregate;
using Bluewater.Core.PayrollAggregate.Specifications;

namespace Bluewater.UseCases.Leaves.Create;

public class CreateLeaveHandler(IRepository<Leave> _repository, IRepository<Payroll> payrollRepository) : ICommandHandler<CreateLeaveCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateLeaveCommand request, CancellationToken cancellationToken)
  {
    if(request.startDate == null || request.endDate == null)
    {
      return Result<Guid>.Invalid(new[] { new ValidationError("Start date and end date are required.") });
    }

    DateTime startDate = request.startDate.Value;
    DateTime endDate = request.endDate.Value;

    if (endDate.Date < startDate.Date)
    {
      return Result<Guid>.Invalid(new[] { new ValidationError("End date cannot be earlier than start date.") });
    }

    bool hasConflictingLeave = await _repository.AnyAsync(
      new LeavesByEmployeeLeaveCreditAndDateRangeSpec(request.employeeId, request.leaveCreditId, startDate, endDate),
      cancellationToken);

    if (hasConflictingLeave)
    {
      return Result<Guid>.Invalid(new[] { new ValidationError("A leave that has not been rejected already exists for this employee, leave code, and date range.") });
    }

    List<DateOnly> payrollDates = GetPayrollPeriodEnds(startDate, endDate).ToList();
    bool hasPayrollCreated = await payrollRepository.AnyAsync(
      new PayrollByEmployeeIdsAndDatesSpec([request.employeeId], payrollDates),
      cancellationToken);

    if (hasPayrollCreated)
    {
      return Result<Guid>.Invalid(new[] { new ValidationError("This leave date range has already been processed in payroll for the employee.") });
    }

    var newLeave = new Leave(request.employeeId, request.leaveCreditId, startDate, endDate, request.isHalfDay);
    newLeave.UpdateLeave(request.employeeId, request.leaveCreditId, startDate, endDate, request.isHalfDay, ApplicationStatus.Pending);
    var createdItem = await _repository.AddAsync(newLeave, cancellationToken);
    return createdItem.Id;
  }

  private static IEnumerable<DateOnly> GetPayrollPeriodEnds(DateTime startDate, DateTime endDate)
  {
    for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
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

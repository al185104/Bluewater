using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.EmployeeAggregate.Specifications;
using Bluewater.Core.PayrollAggregate;
using Bluewater.Core.PayrollAggregate.Specifications;
using Bluewater.Core.ScheduleAggregate;
using Bluewater.Core.ScheduleAggregate.Specifications;
using Bluewater.Core.ShiftAggregate;
using Bluewater.Core.ShiftAggregate.Specifications;

namespace Bluewater.UseCases.Schedules.Import;

public class ImportSchedulesHandler(
  IRepository<Schedule> scheduleRepository,
  IRepository<Payroll> payrollRepository,
  IRepository<Employee> employeeRepository,
  IRepository<Shift> shiftRepository) : ICommandHandler<ImportSchedulesCommand, Result<ScheduleImportResultDTO>>
{
  public async Task<Result<ScheduleImportResultDTO>> Handle(ImportSchedulesCommand request, CancellationToken cancellationToken)
  {
    List<ScheduleImportEntryDTO> entries = request.Entries
      .Where(entry => !string.IsNullOrWhiteSpace(entry.Barcode))
      .GroupBy(entry => (Barcode: entry.Barcode.Trim().ToUpperInvariant(), entry.ScheduleDate))
      .Select(group => group.Last())
      .ToList();

    if (entries.Count == 0)
    {
      return Result.Success(new ScheduleImportResultDTO(0, 0, 0, 0, 0, 0, 0));
    }

    List<Employee> employees = await employeeRepository.ListAsync(
      new EmployeesByBarcodesAndTenantSpec(entries.Select(entry => entry.Barcode), request.Tenant),
      cancellationToken);

    Dictionary<string, Employee> employeesByBarcode = employees
      .Where(employee => employee.User is not null)
      .GroupBy(employee => employee.User!.Username.Trim(), StringComparer.OrdinalIgnoreCase)
      .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

    List<Shift> shifts = await shiftRepository.ListAsync(
      new ShiftsByNamesSpec(entries
        .Select(entry => entry.ShiftName)
        .Where(name => !string.IsNullOrWhiteSpace(name))
        .Select(name => name!)),
      cancellationToken);

    Dictionary<string, Shift> shiftsByName = shifts
      .GroupBy(shift => shift.Name.Trim(), StringComparer.OrdinalIgnoreCase)
      .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

    var resolvedEntries = new List<ResolvedScheduleImportEntry>();
    int skippedInvalid = 0;

    foreach (ScheduleImportEntryDTO entry in entries)
    {
      if (!employeesByBarcode.TryGetValue(entry.Barcode.Trim(), out Employee? employee))
      {
        skippedInvalid++;
        continue;
      }

      Guid? shiftId = null;
      if (!string.IsNullOrWhiteSpace(entry.ShiftName))
      {
        if (!shiftsByName.TryGetValue(entry.ShiftName.Trim(), out Shift? shift))
        {
          skippedInvalid++;
          continue;
        }

        shiftId = shift.Id;
      }

      resolvedEntries.Add(new ResolvedScheduleImportEntry(employee.Id, entry.ScheduleDate, shiftId, entry.IsDefault));
    }

    if (resolvedEntries.Count == 0)
    {
      return Result.Success(new ScheduleImportResultDTO(entries.Count, 0, 0, 0, 0, 0, skippedInvalid));
    }

    DateOnly startDate = resolvedEntries.Min(entry => entry.ScheduleDate);
    DateOnly endDate = resolvedEntries.Max(entry => entry.ScheduleDate);
    List<Guid> employeeIds = resolvedEntries.Select(entry => entry.EmployeeId).Distinct().ToList();
    List<DateOnly> payrollDates = resolvedEntries
      .Select(entry => GetPayrollPeriodEnd(entry.ScheduleDate))
      .Distinct()
      .ToList();

    List<Payroll> payrolls = await payrollRepository.ListAsync(
      new PayrollByEmployeeIdsAndDatesSpec(employeeIds, payrollDates),
      cancellationToken);

    HashSet<(Guid EmployeeId, DateOnly PayrollDate)> lockedPayrolls = payrolls
      .Where(payroll => payroll.EmployeeId.HasValue)
      .Select(payroll => (payroll.EmployeeId!.Value, payroll.Date))
      .ToHashSet();

    List<Schedule> existingSchedules = await scheduleRepository.ListAsync(
      new ScheduleByEmployeeIdsAndDatesSpec(employeeIds, startDate, endDate),
      cancellationToken);

    Dictionary<(Guid EmployeeId, DateOnly ScheduleDate), Schedule> existingByEmployeeAndDate = existingSchedules
      .GroupBy(schedule => (schedule.EmployeeId, schedule.ScheduleDate))
      .ToDictionary(group => group.Key, group => group.First());

    int created = 0;
    int updated = 0;
    int deleted = 0;
    int skippedPayrollLocked = 0;
    int skippedUnchanged = 0;
    foreach (ResolvedScheduleImportEntry entry in resolvedEntries)
    {
      DateOnly payrollDate = GetPayrollPeriodEnd(entry.ScheduleDate);
      if (lockedPayrolls.Contains((entry.EmployeeId, payrollDate)))
      {
        skippedPayrollLocked++;
        continue;
      }

      (Guid EmployeeId, DateOnly ScheduleDate) key = (entry.EmployeeId, entry.ScheduleDate);
      existingByEmployeeAndDate.TryGetValue(key, out Schedule? existingSchedule);
      bool isNoShift = entry.ShiftId is null || entry.ShiftId == Guid.Empty;

      if (isNoShift)
      {
        if (existingSchedule is null)
        {
          skippedUnchanged++;
          continue;
        }

        await scheduleRepository.DeleteAsync(existingSchedule, cancellationToken);
        deleted++;
        continue;
      }

      Guid shiftId = entry.ShiftId!.Value;

      if (existingSchedule is null)
      {
        await scheduleRepository.AddAsync(new Schedule(entry.EmployeeId, shiftId, entry.ScheduleDate, entry.IsDefault), cancellationToken);
        created++;
        continue;
      }

      if (existingSchedule.ShiftId == shiftId)
      {
        skippedUnchanged++;
        continue;
      }

      existingSchedule.UpdateSchedule(entry.EmployeeId, shiftId, entry.ScheduleDate, existingSchedule.IsDefault);
      await scheduleRepository.UpdateAsync(existingSchedule, cancellationToken);
      updated++;
    }

    return Result.Success(new ScheduleImportResultDTO(
      entries.Count,
      created,
      updated,
      deleted,
      skippedPayrollLocked,
      skippedUnchanged,
      skippedInvalid));
  }

  private sealed record ResolvedScheduleImportEntry(
    Guid EmployeeId,
    DateOnly ScheduleDate,
    Guid? ShiftId,
    bool IsDefault);

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
